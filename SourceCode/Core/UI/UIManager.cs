using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Core.Event;
#if FAIRYGUI
using FairyGUI;

#endif

namespace Core.UI
{
    public enum UILayer
    {
        HUD = 0,
        Normal,
        Top,
        Guide,
        Dialog
    }

    public enum UIMode
    {
        InternalGUI,
        FairyGUI
    }

    public enum UIEvent
    {
        Resize,
        BeforeStart,
        Start,
        BeforeOpen,
        Open,
        Focus,
        Blur,
        BeforeClose,
        Close,
        Destroy,
    }

    public struct BindInfo : IEquatable<BindInfo>
    {
        private Type ctrlType;
        private Type viewType;
        private UILayer layer;
        private bool fullScreen;
        private bool disableFocus;
        private string path;

        public Type CtrlType
        {
            get { return ctrlType; }
        }

        public Type ViewType
        {
            get { return viewType; }
        }

        public UILayer Layer
        {
            get { return layer; }
        }

        public bool FullScreen
        {
            get { return fullScreen; }
        }

        public bool DisableFocus
        {
            get { return disableFocus; }
        }

        public string Path
        {
            get { return path; }
        }

        public BindInfo(Type ctrlType, Type viewType, UILayer layer, bool fullScreen, bool disableFocus, string path)
        {
            this.ctrlType = ctrlType;
            this.viewType = viewType;
            this.layer = layer;
            this.fullScreen = fullScreen;
            this.disableFocus = disableFocus;
            this.path = path;
        }

        public bool Equals(BindInfo other)
        {
            return ctrlType == other.ctrlType && viewType == other.viewType && layer == other.layer &&
                   fullScreen == other.fullScreen && disableFocus == other.disableFocus && path == other.path;
        }

        public override bool Equals(object obj)
        {
            return obj is BindInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ctrlType != null ? ctrlType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (viewType != null ? viewType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)layer;
                hashCode = (hashCode * 397) ^ fullScreen.GetHashCode();
                hashCode = (hashCode * 397) ^ disableFocus.GetHashCode();
                hashCode = (hashCode * 397) ^ (path != null ? path.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("ctrlType:{0}", ctrlType);
            str.AppendFormat(",viewType:{0}", viewType);
            str.AppendFormat(",layer:{0}", layer);
            str.AppendFormat(",fullScreen:{0}", fullScreen);
            str.AppendFormat(",disableFocus:{0}", disableFocus);
            str.AppendFormat(",path:{0}", path);
            return str.ToString();
        }
    }

    public class UIManager : Singleton<UIManager>
    {
        public UIMode UIMode { get; private set; }
        public int DesignWidth { get; private set; }
        public int DesignHeight { get; private set; }

        //默认异步加载
        public bool isAsync = false;

        private Dictionary<string, BindInfo> bindInfoMap = new Dictionary<string, BindInfo>();

        private Dictionary<UILayer, object> layerMap = new Dictionary<UILayer, object>();

        private Dictionary<string, UIProxy> allMap = new Dictionary<string, UIProxy>();
        private LinkedList<string> opens = new LinkedList<string>();

        private EventDispatcher<int> eventDispatcher = new EventDispatcher<int>();
        private HashSet<string> dontDestroySet = new HashSet<string>();

        private Dictionary<Type, string> typeNameMap = new Dictionary<Type, string>();

        private SafeAreaAdapter safeAdapter = new SafeAreaAdapter();
        private PopupChecker popup = new PopupChecker();

        private List<string> temp = new List<string>();

        public SafeAreaAdapter SafeAdapter
        {
            get { return safeAdapter; }
        }

        public PopupChecker Popup
        {
            get { return popup; }
        }

        public void Initial(UIMode uiMode, int designWidth, int designHeight)
        {
            this.UIMode = uiMode;
            this.DesignWidth = designWidth;
            this.DesignHeight = designHeight;
            if (uiMode == UIMode.InternalGUI)
            {
                Logger.WarnFormat(":{0} Mode don't realized", uiMode);
            }
            else if (uiMode == UIMode.FairyGUI)
            {
                CreateLayerComps();
            }

            safeAdapter.Initial();
            popup.Initial();
        }

        public void BindUI(string uiName, BindInfo info)
        {
            bindInfoMap[uiName] = info;
        }

        public void BindUIType(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                                BindingFlags.FlattenHierarchy);
            foreach (var item in fields)
            {
                BindUIAttribute attribute = item.GetCustomAttribute<BindUIAttribute>();
                if (null != attribute)
                {
                    string uiName = item.GetValue(null).ToString();
                    BindUI(uiName, attribute.ConvertBindInfo(uiName));
                }
            }
        }

        public void RegisterUIType(Type type)
        {
            RegisterUIAttribute attribute = type.GetCustomAttribute<RegisterUIAttribute>();
            if (null == attribute)
            {
                return;
            }

            Type baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(UIController<>))
                {
                    BindUI(type.Name, attribute.ConvertBindInfo(type, baseType.GetGenericArguments()[0]));
                    break;
                }

                baseType = baseType.BaseType;
            }
        }

        public void RegisterUITypes(Type[] types)
        {
            foreach (var type in types)
            {
                RegisterUIType(type);
            }
        }

        public void RegisterUIAssembly(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            RegisterUITypes(types);
        }

        public void RegisterUIAssembly()
        {
            RegisterUIAssembly(Assembly.GetCallingAssembly());
        }

        private void CreateLayerComps()
        {
#if FAIRYGUI
            GRoot.inst.SetContentScaleFactor(DesignWidth, DesignHeight);
            for (int i = 0; i < ((int)UILayer.Dialog + 1); i++)
            {
                var comp = new FairyGUI.GComponent();
                comp.name = Enum.GetName(typeof(UILayer), i);
                comp.gameObjectName = comp.name;
                comp.AddRelation(GRoot.inst, FairyGUI.RelationType.Size);
                comp.MakeFullScreen();
                GRoot.inst.AddChild(comp);
                layerMap[(UILayer)i] = comp;
            }
#endif
        }

        public T GetLayer<T>(UILayer layer)
        {
            return (T)layerMap[layer];
        }

        internal UIProxy InternalCreateProxy(string uiName, Action<bool, string> completed, object container,
            UIProxy parent)
        {
            if (!bindInfoMap.TryGetValue(uiName, out var bindInfo))
            {
                Logger.WarnFormat("[{0}] ui don't bind", uiName);
                return null;
            }

            UIProxy proxy = null;
            if (UIMode == UIMode.InternalGUI)
            {
                return null;
            }
            else if (UIMode == UIMode.FairyGUI)
            {
#if FAIRYGUI
                proxy = new FairyUIProxy(uiName, bindInfo);
#endif
            }

            proxy.SetCompleted(completed);
            if (null == container)
            {
                container = layerMap[bindInfo.Layer];
            }

            proxy.SetContainer(container);
            proxy.Initial(parent);
            return proxy;
        }

        private UIProxy CreateProxy(string uiName, Action<bool, string> completed)
        {
            UIProxy proxy = InternalCreateProxy(uiName, completed, null, null);
            allMap.Add(uiName, proxy);
            return proxy;
        }


        public void Preload(string uiName, Action<bool, string> completed)
        {
            if (!allMap.ContainsKey(uiName))
            {
                CreateProxy(uiName, completed);
            }
            else
            {
                completed?.Invoke(true, uiName);
            }
        }

        public void Preload(string uiName)
        {
            Preload(uiName, null);
        }

        public void Preloads(IEnumerable<string> uiNames, Action<bool> completed)
        {
            int totalCount = 0;
            int completedCount = 0;
            foreach (var item in uiNames)
            {
                totalCount++;
            }

            bool result = true;
            foreach (var item in uiNames)
            {
                Preload(item, (res, name) =>
                {
                    result &= res;
                    if (++completedCount >= totalCount)
                    {
                        completed?.Invoke(result);
                    }
                });
            }
        }

        public void Preloads(IEnumerable<string> uiNames)
        {
            Preloads(uiNames, null);
        }

        private void RefreshOrder(string uiName)
        {
            opens.Remove(uiName);
            opens.AddLast(uiName);
            SortAllPanels();
        }

        private void SortAllPanels()
        {
            int index = 0;
            foreach (var item in opens)
            {
                allMap[item].SetZOrder(index++);
            }
        }

        private bool IsDisableFocus(string uiName)
        {
            return bindInfoMap[uiName].DisableFocus;
        }

        private void CheckFocus()
        {
            LinkedListNode<string> curr = opens.Last;
            while (null != curr)
            {
                string uiName = curr.Value;
                if (!IsDisableFocus(uiName))
                {
                    allMap[uiName].Focus();
                    break;
                }

                curr = curr.Previous;
            }
        }

        private void CheckBlur()
        {
            LinkedListNode<string> curr = opens.Last.Previous;
            while (null != curr)
            {
                string uiName = curr.Value;
                if (!IsDisableFocus(uiName))
                {
                    allMap[uiName].Blur();
                    break;
                }

                curr = curr.Previous;
            }
        }

        public void Open(string uiName, params object[] args)
        {
            if (!allMap.TryGetValue(uiName, out var proxy))
            {
                proxy = CreateProxy(uiName, null);
            }

            if (null != proxy)
            {
                RefreshOrder(uiName);
                proxy.Open(args);
                if (!IsDisableFocus(uiName))
                {
                    CheckBlur();
                    proxy.Focus();
                }
            }
        }

        public void Close(string uiName)
        {
            if (allMap.TryGetValue(uiName, out var proxy))
            {
                if (proxy.IsOpen)
                {
                    opens.Remove(uiName);
                    proxy.Close();
                    if (proxy.Blur())
                    {
                        CheckFocus();
                    }
                }
            }
        }

        public void Destroy(string uiName)
        {
            if (allMap.TryGetValue(uiName, out var proxy))
            {
                proxy.Dispose();
                bool remove = opens.Remove(uiName);
                if (remove && !IsDisableFocus(uiName))
                {
                    CheckFocus();
                }

                allMap.Remove(uiName);
                dontDestroySet.Remove(uiName);
            }
        }

        public bool IsOpen(string uiName)
        {
            return opens.Contains(uiName);
        }

        public void Back()
        {
            if (null != opens.Last)
            {
                Close(opens.Last.Value);
            }
        }

        internal string GetUIName(Type type)
        {
            if (!typeNameMap.TryGetValue(type, out var name))
            {
                name = type.Name;
                typeNameMap.Add(type, name);
            }

            return name;
        }

        public void Preload<T>(Action<bool, string> completed) where T : IUIController
        {
            Preload(typeof(T), completed);
        }

        public void Preload<T>() where T : IUIController
        {
            Preload<T>(null);
        }

        public void Preload(Type type, Action<bool, string> completed)
        {
            Preload(GetUIName(type), completed);
        }

        public void Preload(Type type)
        {
            Preload(type, null);
        }

        public void Preloads(IEnumerable<Type> uiTypes, Action<bool> completed)
        {
            List<string> uiNames = new List<string>();
            foreach (var item in uiTypes)
            {
                uiNames.Add(GetUIName(item));
            }

            Preloads(uiNames, completed);
        }

        public void Preloads(IEnumerable<Type> uiTypes)
        {
            Preloads(uiTypes, null);
        }


        public void Open(Type type, params object[] args)
        {
            Open(GetUIName(type), args);
        }

        public void Close(Type type)
        {
            Close(GetUIName(type));
        }

        public void Destroy(Type type)
        {
            Destroy(GetUIName(type));
        }

        public bool IsOpen(Type type)
        {
            return IsOpen(GetUIName(type));
        }

        public void Open<T>(params object[] args) where T : IUIController
        {
            Open(typeof(T), args);
        }

        public void Close<T>() where T : IUIController
        {
            Close(typeof(T));
        }

        public void Destroy<T>() where T : IUIController
        {
            Destroy(typeof(T));
        }

        public bool IsOpen<T>() where T : IUIController
        {
            return IsOpen(typeof(T));
        }

        public void CloseAll()
        {
            foreach (var item in opens)
            {
                var proxy = allMap[item];
                proxy.Close();
                proxy.Blur();
            }

            opens.Clear();
        }

        public void DestroyAll()
        {
            HashSet<string> removes = new HashSet<string>();
            foreach (var item in allMap)
            {
                if (!dontDestroySet.Contains(item.Key))
                {
                    removes.Add(item.Key);
                }
            }

            foreach (var item in removes)
            {
                allMap[item].Dispose();
                allMap.Remove(item);
                opens.Remove(item);
            }
        }

        public List<string> GetVisiblePanels()
        {
            List<string> uiNames = new List<string>();
            foreach (var item in opens)
            {
                if (allMap.TryGetValue(item, out var proxy) && proxy.IsVisible)
                {
                    uiNames.Add(item);
                }
            }

            return uiNames;
        }

        public void GetVisiblePanels(ref List<string> res)
        {
            res.Clear();
            foreach (var item in opens)
            {
                if (allMap.TryGetValue(item, out var proxy) && proxy.IsVisible)
                {
                    res.Add(item);
                }
            }
        }

        public void SetPanelVisible(string uiName, bool visible)
        {
            if (allMap.TryGetValue(uiName, out var proxy))
            {
                if ((proxy.IsOpen))
                {
                    proxy.SetGUIVisible(visible);
                }
            }
        }

        public void SetPanelsVisible(IEnumerable<string> panels, bool visible)
        {
            foreach (var item in panels)
            {
                SetPanelVisible(item, visible);
            }
        }

        public void SetPanelVisible(Type type, bool visible)
        {
            string uiName = GetUIName(type);
            SetPanelVisible(uiName, visible);
        }

        public void SetPanelVisible<T>(bool visible) where T : IUIController
        {
            SetPanelVisible(typeof(T), visible);
        }

        public void SetPanelsVisible(IEnumerable<Type> types, bool visible)
        {
            foreach (var item in types)
            {
                SetPanelVisible(item, visible);
            }
        }

        public void SetDontDestroy(string uiName, bool dontDestroy)
        {
            if (dontDestroy)
            {
                dontDestroySet.Add(uiName);
            }
            else
            {
                dontDestroySet.Remove(uiName);
            }
        }

        public void SetDontDestroys(IEnumerable<string> uiNames, bool dontDestroy)
        {
            foreach (var item in uiNames)
            {
                SetDontDestroy(item, dontDestroy);
            }
        }

        public void SetDontDestroy(Type type, bool dontDestroy)
        {
            string uiName = GetUIName(type);
            SetDontDestroy(uiName, dontDestroy);
        }

        public void SetDontDestroy<T>(bool dontDestroy) where T : IUIController
        {
            SetDontDestroy(typeof(T), dontDestroy);
        }

        public void SetDontDestroys(bool dontDestroy, IEnumerable<Type> types)
        {
            foreach (var item in types)
            {
                SetDontDestroy(item, dontDestroy);
            }
        }

        public List<string> GetDontDestroys()
        {
            int count = dontDestroySet.Count;
            List<string> uiNames = new List<string>(count);
            int index = 0;
            foreach (var item in dontDestroySet)
            {
                uiNames[index++] = item;
            }

            return uiNames;
        }

        public void GetDontDestroys(ref List<string> res)
        {
            res.Clear();
            foreach (var item in dontDestroySet)
            {
                res.Add(item);
            }
        }

        public void ClearDontDestroys()
        {
            dontDestroySet.Clear();
        }

        public bool HasFullScreen(string beginPanel)
        {
            bool hasFullScreen = false;
            bool startFind = false;
            foreach (var item in opens)
            {
                if (!startFind)
                {
                    if (item == beginPanel)
                    {
                        startFind = true;
                    }

                    continue;
                }

                if (allMap.TryGetValue(item, out var proxy))
                {
                    if (proxy.IsFullScreen)
                    {
                        hasFullScreen = true;
                        break;
                    }
                }
            }

            return hasFullScreen;
        }

        public bool HasFullScreen(Type type)
        {
            return HasFullScreen(GetUIName(type));
        }

        public bool HasFullScreen<T>() where T : IUIController
        {
            return HasFullScreen(typeof(T));
        }

        public bool HasFullScreen()
        {
            return HasFullScreen(opens.First.Value);
        }

        public void Update(float deltaTime)
        {
            var item = opens.First;
            while (item != null)
            {
                allMap[item.Value].Update(deltaTime);
                item = item.Next;
            }
        }

        public void HotReload(Type type)
        {
            string uiName = GetUIName(type);
            if (!allMap.TryGetValue(uiName, out var proxy))
            {
                return;
            }

            if (!bindInfoMap.TryGetValue(uiName, out var bindInfo))
            {
                return;
            }

#if FAIRYGUI
            FairyGUIManager.Instance.SetBinderAssembly(type.Assembly);
            string[] values = bindInfo.Path.Split('/');
            string pkgName = values[0];
            FairyGUIManager.Instance.RemoveBinder(pkgName);
            FairyGUIManager.Instance.RegisterBinder(pkgName);
#endif

            RegisterUIType(type);
            var isOpen = proxy.IsOpen;
            var args = proxy.Args;
            Destroy(uiName);
            if (isOpen)
            {
                Open(uiName, args);
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            dontDestroySet.Clear();
            safeAdapter.Dispose();
            popup.Dispose();
            DestroyAll();
        }

        public void AddListener(UIEvent uiEvent, Action<IUIController> callback)
        {
            eventDispatcher.AddListener((int)uiEvent, callback);
        }

        public void RemoveListener(UIEvent uiEvent, Action<IUIController> callback)
        {
            eventDispatcher.RemoveListener((int)uiEvent, callback);
        }

        internal void DispatchListener(UIEvent uiEvent, IUIController ctrl)
        {
            eventDispatcher.DispatchListener((int)uiEvent, ctrl);
        }
    }
}