using System;

namespace Core.FS
{
    public class LoadedBundleRequest : AbstractBundleRequest
    {
        private AssetBundleObject bundle;
        private Action<IBundleAsyncRequest, AssetBundleObject> requestCallback;

        public void Initial(string path, AssetBundleObject bundle,
            Action<IBundleAsyncRequest, AssetBundleObject> requestCallback)
        {
            this.Path = path;
            this.bundle = bundle;
            this.requestCallback = requestCallback;
            //先保持一下，防止在处理过程被卸载了
            ((IReferenceCount) bundle).AddRef();
            CreateAsyncOperation();
        }

        public override void Request()
        {
            //释放之前保持的
            ((IReferenceCount) bundle).DecRef();
            requestCallback.Invoke(this, bundle);
        }

        public override void Clear()
        {
            base.Clear();
            bundle = null;
            requestCallback = null;
        }
    }
}