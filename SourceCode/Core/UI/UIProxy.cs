using System;
using Core.FS;
using UnityEngine;

namespace Core.UI
{
    internal enum LoadState
    {
        None,
        Loading,
        Success,
        Fail
    }

    public abstract class UIProxy
    {
        protected string uiName;
        protected BindInfo bindInfo;

        protected object container;
        private IUIController ctrl;
        protected object target;
        protected Vector3 position;

        private LoadState loadState;
        protected object gui;
        protected object view;
        protected UIProxy parent;

        private bool isStart;
        protected bool isOpen;
        private bool isFocus;
        protected bool isVisible;
        protected int zOrder;
        private Action<bool, string> completed;
        private object[] args;

        public string UIName
        {
            get { return uiName; }
        }

        internal IUIController Ctrl
        {
            get { return ctrl; }
        }

        internal object Gui
        {
            get { return gui; }
        }

        internal object View
        {
            get { return view; }
        }

        internal UIProxy Parent
        {
            get { return parent; }
        }

        internal bool IsFullScreen
        {
            get { return bindInfo.FullScreen; }
        }

        internal bool IsOpen
        {
            get { return isOpen; }
        }

        internal bool IsFocus
        {
            get { return isFocus; }
        }

        internal bool IsVisible
        {
            get { return isVisible; }
        }

        internal int ZOrder
        {
            get { return zOrder; }
        }

        internal object[] Args
        {
            get { return args; }
        }

        protected UIProxy(string uiName, BindInfo bindInfo)
        {
            this.uiName = uiName;
            this.bindInfo = bindInfo;
        }

        public void Initial(UIProxy parent)
        {
            ctrl = (IUIController)Activator.CreateInstance(bindInfo.CtrlType);
            ctrl.Initial(this);
            loadState = LoadState.None;
            zOrder = -1;
            isVisible = true;
            this.parent = parent;
            LoadAssets();
        }

        protected void LoadAssets()
        {
            if (loadState == LoadState.None || loadState == LoadState.Fail)
            {
                loadState = LoadState.Loading;
                StartLoad();
            }
        }

        protected void OnLoadCompleted(bool res, AssetObject asset)
        {
            if (!res)
            {
                loadState = LoadState.Fail;
                isOpen = false;
            }
            else
            {
                if (loadState == LoadState.Loading)
                {
                    loadState = LoadState.Success;
                    OnLoadSuccess(asset);
                }
            }

            completed?.Invoke(res, uiName);
        }

        public void SetCompleted(Action<bool, string> completed)
        {
            this.completed = completed;
        }

        public void SetContainer(object container)
        {
            this.container = container;
            if (null != gui)
            {
                AddToContainer();
            }
        }

        public void SetRelative(object target)
        {
            this.target = target;
            if (null != gui)
            {
                RefreshRelativeTarget();
            }
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
            if (null != gui)
            {
                RefreshPosition();
            }
        }

        protected abstract void StartLoad();

        protected abstract void OnLoadSuccess(AssetObject asset);

        protected abstract void AddToContainer();
        protected abstract void SetGUIName();

        public abstract int GetChildCount();

        protected abstract void RefreshRelativeTarget();

        protected abstract void RefreshPosition();

        public void SetGUIVisible(bool isVisible)
        {
            if (this.isVisible != isVisible)
            {
                this.isVisible = isVisible;
                RefreshGUIVisible();
            }
        }

        protected abstract void RefreshGUIVisible();

        protected abstract void RefreshGUIZOrder();

        protected abstract void DestroyGUI();

        public void SetZOrder(int value)
        {
            if (value >= 0 && zOrder != value)
            {
                zOrder = value;
                if (isStart && isOpen)
                {
                    RefreshGUIZOrder();
                }
            }
        }

        protected void StartController(object gui, object view)
        {
            this.gui = gui;
            this.view = view;
            AddToContainer();
            if (Application.isEditor)
            {
                SetGUIName();
            }

            RefreshRelativeTarget();
            RefreshPosition();

            isStart = true;
            UIManager.Instance.DispatchListener(UIEvent.BeforeStart, ctrl);
            try
            {
                ctrl.Start();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("UI Exception:{0}\n{1}", e.Message, e.StackTrace);
            }

            UIManager.Instance.DispatchListener(UIEvent.Start, ctrl);
            if (isOpen)
            {
                OpenController();
            }
            else
            {
                SetGUIVisible(isOpen);
            }

            if (isFocus)
            {
                FocusController();
            }
        }

        private void OpenController()
        {
            SetGUIVisible(isOpen);
            RefreshGUIZOrder();
            UIManager.Instance.DispatchListener(UIEvent.BeforeOpen, ctrl);
            try
            {
                ctrl.Open(args);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("UI Exception:{0}\n{1}", e.Message, e.StackTrace);
            }

            UIManager.Instance.DispatchListener(UIEvent.Open, ctrl);
        }

        private void CloseController()
        {
            UIManager.Instance.DispatchListener(UIEvent.BeforeClose, ctrl);
            try
            {
                ctrl.Close();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("UI Exception:{0}\n{1}", e.Message, e.StackTrace);
            }

            UIManager.Instance.DispatchListener(UIEvent.Close, ctrl);
        }

        public void Open(params object[] args)
        {
            this.args = args;
            if (!isOpen)
            {
                isOpen = true;
                if (isStart)
                {
                    OpenController();
                }
                else
                {
                    LoadAssets();
                }
            }
            else
            {
                if (isStart)
                {
                    SetGUIVisible(isOpen);
                    RefreshGUIZOrder();
                }
            }
        }
        
        private void FocusController()
        {
            try
            {
                ctrl.Focus(args);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("UI Exception:{0}\n{1}", e.Message, e.StackTrace);
            }

            UIManager.Instance.DispatchListener(UIEvent.Focus, ctrl);
        }

        private void BlurController()
        {
            try
            {
                ctrl.Blur();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("UI Exception:{0}\n{1}", e.Message, e.StackTrace);
            }

            UIManager.Instance.DispatchListener(UIEvent.Blur, ctrl);
        }

        public void Focus()
        {
            if (!isFocus)
            {
                isFocus = true;
                if (isStart)
                {
                    FocusController();
                }
            }
        }

        public bool Blur()
        {
            if (isFocus)
            {
                isFocus = false;
                if (isStart)
                {
                    BlurController();
                }

                return true;
            }

            return false;
        }

        public void Update(float deltaTime)
        {
            if (isStart && isOpen)
            {
                ctrl.Update(deltaTime);
            }
        }

        public bool Close()
        {
            if (isOpen)
            {
                isOpen = false;
                if (isStart)
                {
                    SetGUIVisible(isOpen);
                    CloseController();
                }

                return true;
            }

            return false;
        }
        
        public void Dispose()
        {
            if (isStart)
            {
                if (isOpen)
                {
                    CloseController();
                }

                if (isFocus)
                {
                    BlurController();
                }

                UIManager.Instance.DispatchListener(UIEvent.Destroy, ctrl);
                try
                {
                    ctrl.Destroy();
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("UI Exception:{0}\n{1}", e.Message, e.StackTrace);
                }

                DestroyGUI();
            }

            loadState = LoadState.None;
            OnDispose();
            ctrl = null;
            gui = null;
            view = null;
        }

        protected abstract void OnDispose();
    }
}