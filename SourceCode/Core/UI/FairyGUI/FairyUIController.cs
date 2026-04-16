#if FAIRYGUI
using System;
using System.Collections.Generic;
using System.Reflection;
using FairyGUI;

namespace Core.UI
{
    public abstract class FairyUIController<T> : UIController<T> where T : GComponent
    {
        private enum EventType
        {
            None,
            Button0,
            Button1,
            List,
        }

        private const string CLOSE_BTN_NAME = "closeBtn";
        private const string BUTTON_KEY = "OnClick";
        private const string LIST_KEY = "OnRenderer";
        private const string REA_LIST_KEY = "Rea";

        private Dictionary<GObject, UIFX> activeFxMap;
        private Action<UIFX> onUIFxRelease;

        internal override void OnBeforeStart()
        {
            base.OnBeforeStart();
            RegisterUIEvent();
        }

        private void RegisterUIEvent()
        {
            GComponent comp = view as GComponent;
            RegisterCompChildsEvent(comp);
        }

        private void RegisterCompChildsEvent(GComponent comp)
        {
            if (null == comp)
            {
                return;
            }

            for (int i = 0; i < comp.numChildren; i++)
            {
                GObject child = comp.GetChildAt(i);
                if (child is GButton button)
                {
                    RegisterCompChildsEvent(button);
                    if (child.name == CLOSE_BTN_NAME)
                    {
                        button.onClick.Add(CloseSelf);
                    }
                    else
                    {
                        if (ConvertToFuncName(BUTTON_KEY, child.name, out var fName))
                        {
                            if (TryGetMethod(fName, BUTTON_KEY, out Delegate del, out EventType evtType))
                            {
                                if (evtType == EventType.Button0)
                                {
                                    OnClick(button, (EventCallback0)del);
                                }
                                else if (evtType == EventType.Button1)
                                {
                                    OnClick(button, (EventCallback1)del);
                                }
                            }
                        }
                    }
                }
                else if (child is GList list)
                {
                    if (ConvertToFuncName(LIST_KEY, child.name, out var fName))
                    {
                        if (TryGetMethod(fName, LIST_KEY, out Delegate del, out EventType evtType))
                        {
                            if (evtType == EventType.List)
                            {
                                bool isVirtual = !child.name.Contains(REA_LIST_KEY);
                                OnRenderer(list, (ListItemRenderer)del, isVirtual);
                            }
                        }
                    }
                }
                else if (child is GComponent com)
                {
                    RegisterCompChildsEvent(com);
                }
            }
        }

        private bool ConvertToFuncName(string prefix, string wName, out string fName)
        {
            if (!string.IsNullOrEmpty(wName) && wName.Length > 0)
            {
                if (char.IsLower(wName[0]))
                {
                    fName = string.Format("{0}{1}{2}", prefix, char.ToUpper(wName[0]), wName.Substring(1));
                    return true;
                }
            }

            fName = null;
            return false;
        }

        private bool TryGetMethod(string fieldName, string type, out Delegate del, out EventType evtType)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo methodInfo = GetType().GetMethod(fieldName, flags);
            if (null != methodInfo)
            {
                if (type == BUTTON_KEY)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (null == parameters || parameters.Length == 0)
                    {
                        del = Delegate.CreateDelegate(typeof(EventCallback0), this, methodInfo);
                        evtType = EventType.Button0;
                        return true;
                    }

                    if (null != parameters && parameters.Length == 1)
                    {
                        del = Delegate.CreateDelegate(typeof(EventCallback1), this, methodInfo);
                        evtType = EventType.Button1;
                        return true;
                    }
                }
                else if (type == LIST_KEY)
                {
                    del = Delegate.CreateDelegate(typeof(ListItemRenderer), this, methodInfo);
                    evtType = EventType.List;
                    return true;
                }
            }

            del = null;
            evtType = EventType.None;
            return false;
        }

        #region FairyGUI事件

        /// <summary>
        /// 给按钮添加点击事件
        /// </summary>
        /// <param name="button"></param>
        /// <param name="callback"></param>
        protected void OnClick(GButton button, EventCallback1 callback)
        {
            if (null != callback)
            {
                button.onClick.Add(callback);
            }
        }

        protected void OnClick(GButton button, EventCallback0 callback)
        {
            if (null != callback)
            {
                button.onClick.Add(callback);
            }
        }

        /// <summary>
        /// 给按钮移除点击事件
        /// </summary>
        /// <param name="button"></param>
        /// <param name="callback"></param>
        protected void OffClick(GButton button, EventCallback1 callback)
        {
            if (null != callback)
            {
                button.onClick.Remove(callback);
            }
        }

        protected void OffClick(GButton button, EventCallback0 callback)
        {
            if (null != callback)
            {
                button.onClick.Remove(callback);
            }
        }

        /// <summary>
        /// 给List增加渲染事件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        /// <param name="isVirtual"></param>
        protected void OnRenderer(GList list, ListItemRenderer callback, bool isVirtual)
        {
            if (null != callback)
            {
                if (isVirtual)
                {
                    list.SetVirtual();
                }

                list.itemRenderer = callback;
            }
        }

        /// <summary>
        /// 给List增加渲染事件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        protected void OnRenderer(GList list, ListItemRenderer callback)
        {
            OnRenderer(list, callback, true);
        }

        /// <summary>
        /// 给List移除渲染事件
        /// </summary>
        /// <param name="list"></param>
        protected void OffRenderer(GList list)
        {
            list.itemRenderer = null;
        }

        #endregion

        #region FX

        /// <summary>
        /// 在UI控件上播放光效
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <param name="duration"></param>
        public UIFX PlayUIFX(GObject obj, string path, float duration, string pkgName)
        {
            if (null == obj)
            {
                return null;
            }

            if (null == activeFxMap)
            {
                activeFxMap = new Dictionary<GObject, UIFX>();
            }

            if (!activeFxMap.TryGetValue(obj, out var fx))
            {
                if (string.IsNullOrEmpty(pkgName))
                {
                    fx = UIFXManager.Instance.Create(path);
                }
                else
                {
                    fx = UIFXManager.Instance.Create(path, pkgName);
                }

                fx.userData = obj;
                fx.SetParent(obj);

                fx.SetDuration(duration);
                if (duration > 0)
                {
                    if (null == onUIFxRelease)
                    {
                        onUIFxRelease = new Action<UIFX>(OnUIFxRelease);
                    }

                    fx.releaseEvent += onUIFxRelease;
                }

                activeFxMap.Add(obj, fx);
            }

            fx.Play();
            return fx;
        }


        private void OnUIFxRelease(UIFX fx)
        {
            if (null != activeFxMap)
            {
                GObject obj = (GObject)fx.userData;
                if (null != obj)
                {
                    activeFxMap.Remove(obj);
                }
            }
        }

        public UIFX PlayUIFX(GObject obj, string path, float duration)
        {
            return PlayUIFX(obj, path, duration, null);
        }

        public UIFX PlayUIFX(GObject obj, string path, string pkgName)
        {
            return PlayUIFX(obj, path, 0, pkgName);
        }

        /// <summary>
        /// 在UI控件上播放光效
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        public UIFX PlayUIFX(GObject obj, string path)
        {
            return PlayUIFX(obj, path, null);
        }

        /// <summary>
        /// 停止播放UI控件上的光效
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void StopUIFX(GObject obj)
        {
            if (null != obj)
            {
                return;
            }

            if (activeFxMap.TryGetValue(obj, out UIFX fx))
            {
                fx.Stop();
            }
        }

        /// <summary>
        /// 释放UI控件上的光效
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void ReleaseUIFX(GObject obj)
        {
            if (null == obj)
            {
                return;
            }

            if (null != activeFxMap && activeFxMap.TryGetValue(obj, out UIFX fx))
            {
                activeFxMap.Remove(obj);
                UIFXManager.Instance.Release(fx);
            }
        }

        /// <summary>
        /// 删除UI控件上的光效
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void DestroyUIFX(GObject obj)
        {
            if (null == obj)
            {
                return;
            }

            if (null != activeFxMap && activeFxMap.TryGetValue(obj, out UIFX fx))
            {
                activeFxMap.Remove(obj);
                UIFXManager.Instance.Destroy(fx);
            }
        }

        #endregion

        internal override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            if (null != activeFxMap)
            {
                foreach (var item in activeFxMap)
                {
                    UIFXManager.Instance.Destroy(item.Value);
                }

                activeFxMap = null;
            }
        }
    }
}

#endif