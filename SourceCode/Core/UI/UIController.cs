using System;
using System.Collections.Generic;

namespace Core.UI
{
    public abstract class UIController<T> : IUIController
    {
        private UIProxy proxy;
        protected float elapseTime { get; private set; }
        private float perTime;

        private Dictionary<string, UIChildPanel> childMap;

        protected T view
        {
            get { return (T)proxy.View; }
        }

        public string UIName
        {
            get { return proxy.UIName; }
        }

        public object GUI
        {
            get { return proxy.Gui; }
        }

        public IUIController Parent
        {
            get
            {
                if (null != proxy.Parent)
                {
                    return proxy.Parent.Ctrl;
                }

                return null;
            }
        }

        public bool IsFullScreen
        {
            get { return proxy.IsFullScreen; }
        }

        public bool IsFocus
        {
            get { return proxy.IsFocus; }
        }

        public void Initial(UIProxy uiProxy)
        {
            this.proxy = uiProxy;
        }

        public void Start()
        {
            OnBeforeStart();
            OnStart();
        }

        public void Open(params object[] args)
        {
            elapseTime = 0;
            perTime = 0;
            OnOpen(args);
        }

        public void Focus(params object[] args)
        {
            OnFocus(args);
            FocusAllChild();
        }

        public void Update(float deltaTime)
        {
            elapseTime += deltaTime;
            perTime += deltaTime;
            OnUpdate(deltaTime);
            if (perTime >= 1)
            {
                perTime -= 1;
                OnPerSecond();
            }

            UpdateAllChild(deltaTime);
        }

        public void Blur()
        {
            OnBlur();
            BlurAllChild();
        }

        public void Close()
        {
            OnClose();
            CloseAllChild();
        }

        public void Destroy()
        {
            OnDestroy();
            DestroyAllChild();
            this.proxy = null;
        }

        public void CloseSelf()
        {
            if (null != Parent)
            {
                Parent.CloseChild(UIName);
            }
            else
            {
                UIManager.Instance.Close(UIName);
            }
        }

        internal virtual void OnBeforeStart()
        {
        }

        /// <summary>
        /// 创建时候执行
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// 被打开时候执行
        /// </summary>
        /// <param name="uiArgs"></param>
        protected virtual void OnOpen(params object[] args)
        {
        }

        /// <summary>
        /// 获取焦点时执行
        /// </summary>
        /// <param name="uiArgs"></param>
        protected virtual void OnFocus(params object[] args)
        {
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        /// <param name="deltaTime"></param>
        protected virtual void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 每秒执行1次
        /// </summary>
        protected virtual void OnPerSecond()
        {
        }

        /// <summary>
        /// 失去焦点时执行
        /// </summary>
        protected virtual void OnBlur()
        {
        }

        /// <summary>
        /// 隐藏时候执行
        /// </summary>
        protected virtual void OnClose()
        {
        }

        internal virtual void OnBeforeDestroy()
        {
        }

        /// <summary>
        /// 销毁时候执行
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        #region 子面版

        public UIChildPanel OpenChild(string uiName, params object[] args)
        {
            if (null == childMap)
            {
                childMap = new Dictionary<string, UIChildPanel>();
            }

            if (!childMap.TryGetValue(uiName, out var child))
            {
                UIProxy proxy = UIManager.Instance.InternalCreateProxy(uiName, null, this.proxy.Gui, this.proxy);
                child = new UIChildPanel(proxy);
                childMap.Add(uiName, child);
            }

            if (null != child)
            {
                child.Proxy.Open(args);
                if (proxy.IsFocus)
                {
                    child.Proxy.Focus();
                }
            }

            return child;
        }

        public void CloseChild(string uiName)
        {
            if (null != childMap && childMap.TryGetValue(uiName, out var child))
            {
                child.Proxy.Close();
                if (proxy.IsFocus)
                {
                    child.Proxy.Blur();
                }
            }
        }

        public UIChildPanel GetChild(string uiName)
        {
            if (null != childMap && childMap.TryGetValue(uiName, out var child))
            {
                return child;
            }

            return null;
        }

        public bool IsOpenChild(string uiName)
        {
            var child = GetChild(uiName);
            if (null != child)
            {
                return child.Proxy.IsOpen;
            }

            return false;
        }


        public UIChildPanel OpenChild(Type type, params object[] args)
        {
            return OpenChild(UIManager.Instance.GetUIName(type), args);
        }

        public void CloseChild(Type type)
        {
            CloseChild(UIManager.Instance.GetUIName(type));
        }

        public UIChildPanel GetChild(Type type)
        {
            return GetChild(UIManager.Instance.GetUIName(type));
        }

        public bool IsOpenChild(Type type)
        {
            return IsOpenChild(UIManager.Instance.GetUIName(type));
        }

        public UIChildPanel OpenChild<C>(params object[] args) where C : IUIController
        {
            return OpenChild(typeof(C), args);
        }

        public void CloseChild<C>() where C : IUIController
        {
            CloseChild(typeof(C));
        }

        public UIChildPanel GetChild<C>() where C : IUIController
        {
            return GetChild(typeof(C));
        }

        public bool IsOpenChild<C>() where C : IUIController
        {
            return IsOpenChild(typeof(C));
        }

        private void FocusAllChild()
        {
            if (null != childMap)
            {
                foreach (var item in childMap)
                {
                    if (item.Value.Proxy.IsOpen)
                    {
                        item.Value.Proxy.Focus();
                    }
                }
            }
        }

        private void BlurAllChild()
        {
            if (null != childMap)
            {
                foreach (var item in childMap)
                {
                    if (item.Value.Proxy.IsOpen)
                    {
                        item.Value.Proxy.Blur();
                    }
                }
            }
        }

        private void UpdateAllChild(float deltaTime)
        {
            if (null != childMap)
            {
                foreach (var item in childMap)
                {
                    item.Value.Proxy.Update(deltaTime);
                }
            }
        }

        public void CloseAllChild()
        {
            if (null != childMap)
            {
                foreach (var item in childMap)
                {
                    item.Value.Proxy.Close();
                    if (proxy.IsFocus)
                    {
                        item.Value.Proxy.Blur();
                    }
                }
            }
        }

        public void DestroyAllChild()
        {
            if (null != childMap)
            {
                foreach (var item in childMap)
                {
                    item.Value.Proxy.Dispose();
                }

                childMap = null;
            }
        }

        #endregion
    }
}