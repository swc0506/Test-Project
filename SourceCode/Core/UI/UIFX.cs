using System;
using System.Reflection;
using Core.FS;
using UnityEngine;

namespace Core.UI
{
    public abstract class UIFX
    {
        public string Path { get; private set; }
        private AssetPackage assetPkg;

        private LoadState loadState;
        private AssetObject asset;
        public GameObject Skin { get; private set; }
        public bool IsPlaying { get; private set; }

        private float beginTime;
        public float Duration { get; private set; }

        internal bool IsFinished
        {
            get
            {
                if (Duration > 0)
                {
                    return Time.time - beginTime >= Duration;
                }

                return false;
            }
        }


        internal object userData;

        public bool stencil;

        protected object parent;
        protected Vector3 scale;
        protected Vector3 pos;
        protected Vector3 angle;
        protected Vector3 pos3d;

        protected Animator animator;
        protected string stateName;

        protected object touchArea;
        public bool disableDrag;

        public Action<UIFX> activeEvent;
        public Action<UIFX> releaseEvent;

        internal virtual void Initial(string path, AssetPackage assetPkg)
        {
            this.Path = path;
            this.assetPkg = assetPkg;
            loadState = LoadState.None;
            IsPlaying = true;
            scale = Vector3.one;
            LoadAssets();
        }

        protected void LoadAssets()
        {
            if (loadState == LoadState.None || loadState == LoadState.Fail)
            {
                loadState = LoadState.Loading;
                var asyncHandler = assetPkg.LoadAsync(Path);
                asyncHandler.CompletedEvent += OnLoadFXCompleted;
            }
        }

        private void OnLoadFXCompleted(AssetObject asset, string path)
        {
            if (null == asset)
            {
                loadState = LoadState.Fail;
            }
            else
            {
                if (loadState == LoadState.Loading)
                {
                    loadState = LoadState.Success;
                    this.asset = asset;
                    Skin = asset.Get<GameObject>();
                    if (null == Skin)
                    {
                        return;
                    }
                    Skin.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    Skin.transform.localScale = Vector3.one;
                    OnLoadSuccess();

                    RefreshParent();
                    RefreshScale();
                    RefreshPosition();
                    Refresh3DPosition();
                    RefreshAngle();
                    RefreshActive();
                    RefreshAnimator();
                }
            }
        }

        protected abstract void OnLoadSuccess();

        public void Play()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
                RefreshActive();
            }
        }

        public void PlayAnimator(string stateName)
        {
            this.stateName = stateName;
            RefreshAnimator();
        }

        public void Stop()
        {
            if (IsPlaying)
            {
                IsPlaying = false;
                RefreshActive();
            }
        }

        protected virtual void RefreshActive()
        {
            activeEvent?.Invoke(this);
        }

        protected abstract void RefreshAnimator();

        public void SetParent(object parent)
        {
            if (this.parent != parent)
            {
                this.parent = parent;
                RefreshParent();
            }
        }

        protected abstract void RefreshParent();

        public void SetTouchArea(object touchArea)
        {
            if (this.touchArea != touchArea)
            {
                this.touchArea = touchArea;
                RefreshTouchArea();
            }
        }

        public virtual void AddClick<T>(T callback) where T : Delegate
        {
        }

        public virtual void RemoveClick<T>(T callback) where T : Delegate
        {
        }

        protected abstract void RefreshTouchArea();

        #region Scale

        public void SetScale(float scale)
        {
            if (this.scale.x != scale)
            {
                this.scale = new Vector3(scale, scale, scale);
                RefreshScale();
            }
        }

        protected abstract void RefreshScale();

        #endregion

        #region Pos

        public void SetPosition(Vector3 pos)
        {
            if (this.pos != pos)
            {
                this.pos = pos;
                RefreshPosition();
            }
        }

        public void SetPosition(float x, float y, float z)
        {
            SetPosition(new Vector3(x, y, z));
        }

        public void SetPosition(float x, float y)
        {
            SetPosition(x, y, pos.z);
        }

        public void Set3DPosition(Vector3 pos)
        {
            if (this.pos3d != pos)
            {
                this.pos3d = pos;
                Refresh3DPosition();
            }
        }

        public void Set3DPosition(float x, float y, float z)
        {
            Set3DPosition(new Vector3(x, y, z));
        }

        protected abstract void RefreshPosition();

        protected abstract void Refresh3DPosition();

        #endregion

        #region Angle

        public void SetAngle(Vector3 angle)
        {
            if (this.angle != angle)
            {
                this.angle = angle;
                RefreshAngle();
            }
        }

        public void SetRotation(Quaternion rot)
        {
            SetAngle(rot.eulerAngles);
        }

        public void SetAngle(float x, float y, float z)
        {
            SetAngle(new Vector3(x, y, z));
        }

        public void SetAngle(float y)
        {
            SetAngle(new Vector3(angle.x, y, angle.z));
        }

        protected abstract void RefreshAngle();

        public void SetDuration(float duration)
        {
            beginTime = Time.time;
            Duration = duration;
        }

        #endregion

        private void TickReleaseEvent()
        {
            if (null != releaseEvent)
            {
                releaseEvent.Invoke(this);
                releaseEvent = null;
            }
        }

        internal virtual void Clear()
        {
            Stop();
            SetParent(null);
            SetScale(1);
            SetPosition(Vector3.zero);
            SetAngle(Vector3.zero);
            TickReleaseEvent();
            Duration = 0;
            userData = null;
            activeEvent = null;
        }

        internal virtual void Dispose()
        {
            TickReleaseEvent();
            assetPkg = null;
            loadState = LoadState.None;
            if (null != asset)
            {
                asset.Release();
                asset = null;
            }

            if (null != Skin)
            {
                GameObject.Destroy(Skin);
                Skin = null;
            }

            userData = null;
            parent = null;
            releaseEvent = null;
            activeEvent = null;
        }
    }
}