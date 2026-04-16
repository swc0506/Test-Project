using System;
using System.Collections.Generic;
using FairyGUI;

namespace Core.UI
{
    public class PopupChecker
    {
        private HashSet<string> popups = new HashSet<string>();
        private List<IUIController> opened = new List<IUIController>();

        internal void Initial()
        {
            UIManager.Instance.AddListener(UIEvent.Open, OnOpenPopup);
            UIManager.Instance.AddListener(UIEvent.Close, OnClosePopup);

#if FAIRYGUI
            FairyGUI.Stage.inst.onTouchBegin.Add(CheckPopupPanel);
#endif
        }

        private void OnOpenPopup(IUIController ctrl)
        {
            string panel = ctrl.UIName;
            if (popups.Contains(panel))
            {
                if (!opened.Contains(ctrl))
                {
                    opened.Add(ctrl);
                }
            }
        }

        private void OnClosePopup(IUIController ctrl)
        {
            string panel = ctrl.UIName;
            if (popups.Contains(panel))
            {
                opened.Remove(ctrl);
            }
        }

        private void CheckPopupPanel()
        {
#if FAIRYGUI
            var touchTarget = FairyGUI.Stage.inst.touchTarget;
            int count = opened.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var item = opened[i];
                if (null == touchTarget || !Contains((GComponent)item.GUI, touchTarget))
                {
                    UIManager.Instance.Close(item.UIName);
                }
            }
        }


        private bool Contains(GComponent comp, DisplayObject target)
        {
            if (target is DisplayObject displayObject)
            {
                var comObj = comp.displayObject;
                while (null != displayObject)
                {
                    if (displayObject == comObj)
                    {
                        return true;
                    }

                    displayObject = displayObject.parent;
                }
            }

            return false;
        }
#endif

        public void AddPopup(string uiName)
        {
            popups.Add(uiName);
        }

        public void AddPopups(IEnumerable<string> uiNames)
        {
            foreach (var item in uiNames)
            {
                AddPopup(item);
            }
        }

        public void AddPopup(Type type)
        {
            AddPopup(UIManager.Instance.GetUIName(type));
        }

        public void AddPopups(IEnumerable<Type> types)
        {
            foreach (var item in types)
            {
                AddPopup(item);
            }
        }

        public void AddPopup<T>() where T : IUIController
        {
            AddPopup(typeof(T));
        }


        public void RemovePopup(string uiName)
        {
            popups.Remove(uiName);
        }

        public void RemovePopups(IEnumerable<string> uiNames)
        {
            foreach (var item in uiNames)
            {
                RemovePopup(item);
            }
        }

        public void RemovePopup(Type type)
        {
            RemovePopup(UIManager.Instance.GetUIName(type));
        }

        public void RemovePopups(IEnumerable<Type> types)
        {
            foreach (var item in types)
            {
                RemovePopup(item);
            }
        }

        public void RemovePopup<T>() where T : IUIController
        {
            RemovePopup(typeof(T));
        }

        internal void Dispose()
        {
            UIManager.Instance.RemoveListener(UIEvent.Open, OnOpenPopup);
            UIManager.Instance.RemoveListener(UIEvent.Close, OnClosePopup);

#if FAIRYGUI
            FairyGUI.Stage.inst.onTouchBegin.Remove(CheckPopupPanel);
#endif
        }
    }
}