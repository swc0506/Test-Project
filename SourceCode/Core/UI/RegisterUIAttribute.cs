using System;

namespace Core.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterUIAttribute : Attribute
    {
        private string path;
        private UILayer layer;
        private bool fullScreen;
        private bool disableFocus;

        public RegisterUIAttribute(string path, UILayer layer, bool fullScreen, bool disableFocus)
        {
            this.path = path;
            this.layer = layer;
            this.fullScreen = fullScreen;
            this.disableFocus = disableFocus;
        }

        public RegisterUIAttribute(string path) : this(path, UILayer.Normal, false, false)
        {
        }

        public RegisterUIAttribute(string path, UILayer layer) : this(path, layer, false, false)
        {
        }
        
        public RegisterUIAttribute(string path, UILayer layer,bool fullScreen) : this(path, layer, fullScreen, false)
        {
        }

        public BindInfo ConvertBindInfo(Type ctrlType, Type viewType)
        {
            return new BindInfo(ctrlType, viewType, layer,fullScreen, disableFocus, path);
        }
    }
}