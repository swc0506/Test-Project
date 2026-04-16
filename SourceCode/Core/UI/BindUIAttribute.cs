using System;

namespace Core.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindUIAttribute : Attribute
    {
        private Type ctrlType;
        private Type viewType;
        private UILayer layer;
        private bool fullScreen;
        private bool disableFocus;
        private string path;

        public BindUIAttribute(Type ctrlType, Type viewType, UILayer layer = UILayer.Normal, 
            bool fullScreen = false, bool disableFocus = false, string path = null)
        {
            this.ctrlType = ctrlType;
            this.viewType = viewType;
            this.layer = layer;
            this.fullScreen = fullScreen;
            this.disableFocus = disableFocus;
            this.path = path;
        }

        public BindInfo ConvertBindInfo(string uiName)
        {
            string lPath = string.IsNullOrEmpty(path) ? uiName : path;
            return new BindInfo(ctrlType, viewType, layer, fullScreen, disableFocus, lPath);
        }
    }
}