#if FAIRYGUI
using Core.FS;
using FairyGUI;

namespace Core.UI
{
    internal class FairyUIProxy : UIProxy
    {
        private string pkgName;
        private string compName;

        public FairyUIProxy(string uiName, BindInfo bindInfo) : base(uiName, bindInfo)
        {
            string[] values = bindInfo.Path.Split('/');
            pkgName = values[0];
            compName = values[1];
        }

        protected override void StartLoad()
        {
            if (UIManager.Instance.isAsync)
            {
                FairyGUIManager.Instance.LoadPackageAsync(pkgName,
                    (string pkgName, bool res) => { OnLoadCompleted(res, null); });
            }
            else
            {
                FairyGUIManager.Instance.LoadPackage(pkgName);
                OnLoadCompleted(true, null);
            }
        }

        protected override void OnLoadSuccess(AssetObject asset)
        {
            GComponent gui = (GComponent)FairyGUIManager.Instance.CreateObject(pkgName, compName, bindInfo.ViewType);
            GComponent parent = (GComponent)container;
            gui.AddRelation(parent, RelationType.Size);
            gui.SetSize(parent.width, parent.height);
            StartController(gui, gui);
        }

        protected override void AddToContainer()
        {
            ((GComponent)container).AddChild((GObject)gui);
        }

        protected override void SetGUIName()
        {
            ((GObject)gui).gameObjectName = uiName;
        }

        public override int GetChildCount()
        {
            return ((GComponent)container).numChildren;
        }

        protected override void RefreshRelativeTarget()
        {
            if (null != target)
            {
                GObject guiObj = (GObject)gui;
                GObject targetObj = (GObject)target;
                guiObj.position = targetObj.position;
                if (targetObj != GRoot.inst)
                {
                    guiObj.relations.CopyFrom(targetObj.relations);
                }
                else
                {
                    guiObj.AddRelation(targetObj, RelationType.Size);
                    guiObj.SetSize(targetObj.width, targetObj.height);
                }

                GComponent parent = targetObj.parent;
                if (null != parent && targetObj != GRoot.inst)
                {
                    int index = targetObj.parent.GetChildIndex(targetObj);
                    SetZOrder(index);
                }
            }
        }

        protected override void RefreshPosition()
        {
            ((GObject)gui).position = position;
        }

        protected override void RefreshGUIVisible()
        {
            ((GObject)gui).visible = isVisible;
        }

        protected override void RefreshGUIZOrder()
        {
            if (zOrder >= 0)
            {
                ((GComponent)container).SetChildIndex((GObject)gui, zOrder);
            }
        }

        protected override void DestroyGUI()
        {
            ((GObject)gui).Dispose();
        }

        protected override void OnDispose()
        {
        }
    }
}
#endif