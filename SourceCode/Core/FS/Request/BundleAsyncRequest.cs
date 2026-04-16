using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.FS
{
    internal class BundleAsyncRequest : AbstractAssetRequest
    {
        private AssetBundlePackage bundles;

        public void Initial(string path, Type type, string loadPath,
            Action<IAssetAsyncRequest, Object> loadCallback, AssetBundlePackage bundles)
        {
            base.Initial(path, type, loadPath, loadCallback);
            this.bundles = bundles;
        }

        public override void Request()
        {
            if (string.IsNullOrEmpty(LoadPath) || null == bundles)
            {
                OnLoadCompleted(null);
                return;
            }

            var asyncOperation = bundles.LoadAsync(LoadPath, AsyncOperation.priority);
            asyncOperation.CompletedEvent += OnBundleCompleted;
        }

        private void OnBundleCompleted(AssetBundleObject bundle)
        {
            if (null != bundle)
            {
                if (LoadType == typeof(SceneAsset))
                {
                    Object asset = SceneAsset.Create(Path);
                    OnLoadCompleted(asset);
                }
                else
                {
                    // Object asset = bundle.Result.LoadAsset(Path, LoadType);
                    // OnLoadCompleted(asset);
                    AssetBundleRequest abRequest = bundle.Result.LoadAssetAsync(Path, LoadType);
                    abRequest.completed += OnAssetCompleted;
                }
            }
            else
            {
                OnLoadCompleted(null);
            }
        }

        private void OnAssetCompleted(AsyncOperation operation)
        {
            Object asset = ((AssetBundleRequest)operation).asset;
            OnLoadCompleted(asset);
        }
    }
}