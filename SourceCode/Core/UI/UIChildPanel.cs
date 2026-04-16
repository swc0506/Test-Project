using UnityEngine;

namespace Core.UI
{
    public class UIChildPanel
    {
        private UIProxy proxy;

        internal UIProxy Proxy
        {
            get { return proxy; }
        }

        public bool IsFullScreen
        {
            get { return proxy.IsFullScreen; }
        }

        public bool IsFocus
        {
            get { return proxy.IsFocus; }
        }
        
        public UIChildPanel(UIProxy proxy)
        {
            this.proxy = proxy;
        }

        public void SetRelative(object target)
        {
            proxy.SetRelative(target);
        }

        public void SetZOrder(int zOrder)
        {
            int lastZOrder = proxy.GetChildCount() - 1;
            if (zOrder >= 0 && zOrder <= lastZOrder)
            {
                proxy.SetZOrder(zOrder);
            }
        }

        public void SetZOrderToLast()
        {
            int zOrder = proxy.GetChildCount() - 1;
            if (zOrder >= 0)
            {
                proxy.SetZOrder(zOrder);
            }
        }

        public void SetPosition(Vector3 pos)
        {
            proxy.SetPosition(pos);
        }
    }
}