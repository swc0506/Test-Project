using System;
using UnityEngine;

namespace Core.Entity
{
    public class EntityRendererComponent : EntityComponent
    {
        public RendererEntity Owner
        {
            get { return (RendererEntity)owner; }
        }

        private Action<string, GameObject> onLoadedSkin;

        private bool isLoading;
        private int skinLoadId;

        protected override void OnInitial()
        {
            base.OnInitial();
            onLoadedSkin = OnLoadedSkin;
        }

        public void LoadSkin()
        {
            if (null == Owner.Skin && !isLoading)
            {
                isLoading = true;
                skinLoadId = Owner.Context.GoPool.PopAsync(Owner.GetSkinPath(), onLoadedSkin);
            }
        }

        public void ClearSkin()
        {
            if (null != Owner.Skin)
            {
                Owner.Context.GoPool.Push(Owner.Skin, Owner.GetSkinPath());
                Owner.Skin = null;
                Owner.OnRemoveSkin();
            }

            if (skinLoadId != 0)
            {
                Owner.Context.GoPool.CancelAsync(skinLoadId);
                skinLoadId = 0;
                Owner.OnRemoveSkin();
            }

            isLoading = false;
        }

        private void OnLoadedSkin(string path, GameObject skin)
        {
            isLoading = false;
            skinLoadId = 0;
            if (null == skin)
            {
                Logger.WarnFormat("Skin is null:{0}", Owner.GetSkinPath());
                return;
            }

            Owner.Skin = skin;
            RefreshAll();
            OnSetSkin();
            Owner.OnSetSkin();
            Owner.Context.SetSkinEvent?.Invoke(Owner);
        }

        protected virtual void OnSetSkin()
        {
        }

        public void RefreshAll()
        {
            if (Owner.Layer < 0 && null != Owner.Skin)
            {
                Owner.Layer = Owner.Skin.layer;
            }

            RefreshParent();
            RefreshLayer();
            RefreshPosition();
            RefreshRotate();
            RefreshScale();
            RefreshVisible();
        }

        public void RefreshParent()
        {
            GameObjectUtils.SetParent(Owner.Skin, Owner.Parent, false);
        }

        public void RefreshLayer()
        {
            GameObjectUtils.SetLayer(Owner.Skin, Owner.Layer);
        }

        public virtual void RefreshPosition()
        {
            if (null != Owner.Skin)
            {
                Owner.Skin.transform.position = Owner.Pos;
            }
        }

        public virtual void RefreshRotate()
        {
            if (null != Owner.Skin)
            {
                Owner.Skin.transform.rotation = Owner.Rot;
            }
        }

        public virtual void RefreshScale()
        {
            if (null != Owner.Skin)
            {
                Owner.Skin.transform.localScale = Owner.Scale;
            }
        }

        public virtual void RefreshVisible()
        {
            if (null != Owner.Skin)
            {
                GameObjectUtils.SetActive(Owner.Skin, Owner.Visible);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameObjectUtils.SetActive(Owner.Skin, true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameObjectUtils.SetActive(Owner.Skin, false);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            ClearSkin();
        }
    }
}