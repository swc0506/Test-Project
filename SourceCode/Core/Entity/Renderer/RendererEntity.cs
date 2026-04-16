using UnityEngine;

namespace Core.Entity
{
    public abstract class RendererEntity : Entity
    {
        public new IRendererEntityManager Context
        {
            get { return (IRendererEntityManager)base.Context; }
        }

        public EntityRendererComponent renderer;

        public GameObject Skin { get; internal set; }
        public Transform Parent { get; private set; }
        public int Layer { get; internal set; }
        public Vector3 Pos { get; private set; }
        public Quaternion Rot { get; private set; }
        public Vector3 Scale { get; private set; }

        public bool Visible { get; private set; }


        protected override void OnInitial()
        {
            base.OnInitial();
            Layer = -1;
            Scale = Vector3.one;
            Visible = true;
        }

        protected override void OnAfterStart(CreateArgs args)
        {
            base.OnAfterStart(args);
            renderer?.LoadSkin();
        }

        public abstract string GetSkinPath();

        public virtual void OnSetSkin()
        {
        }
        
        public virtual void OnRemoveSkin()
        {
        }

        public void SetParent(Transform parent)
        {
            if (this.Parent == parent)
            {
                return;
            }

            this.Parent = parent;
            renderer?.RefreshParent();
        }

        public void SetLayer(int layer)
        {
            if (this.Layer == layer)
            {
                return;
            }

            this.Layer = layer;
            renderer?.RefreshLayer();
        }

        public void SetPosition(Vector3 pos)
        {
            if (this.Pos == pos)
            {
                return;
            }

            this.Pos = pos;
            renderer?.RefreshPosition();
        }

        public void SetRotate(Quaternion rot)
        {
            if (this.Rot == rot)
            {
                return;
            }

            this.Rot = rot;
            renderer?.RefreshRotate();
        }

        public void SetScale(Vector3 scale)
        {
            if (this.Scale == scale)
            {
                return;
            }

            this.Scale = scale;
            renderer?.RefreshScale();
        }

        public void SetScale(float scale)
        {
            Vector3 scaleVec = new Vector3(scale, scale, scale);
            SetScale(scaleVec);
        }

        public void SetVisible(bool visible)
        {
            if (this.Visible == visible)
            {
                return;
            }

            this.Visible = visible;
            renderer?.RefreshVisible();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
        }
    }
}