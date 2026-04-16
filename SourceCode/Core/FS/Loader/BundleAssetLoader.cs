using System;
using Object = UnityEngine.Object;

namespace Core.FS
{
    public class BundleAssetLoader : AbstractAssetLoader
    {
        private readonly AssetBundlePackage bundles;

        public BundleAssetLoader(AssetPackage cachePool, AssetBundlePackage bundles) : base(cachePool)
        {
            this.bundles = bundles;
        }

        protected override string ParseLoadPath(string path)
        {
            if (null != bundles)
            {
                return bundles.GetAddressPath(path);
            }

            return null;
        }

        protected override AssetType GetAssetType()
        {
            return AssetType.AssetBundle;
        }

        protected override void LoadAsset(string path, Type type, out Object result, out string loadSign)
        {
            string loadPath = GetLoadPath(path);
            loadSign = loadPath;
            if (string.IsNullOrEmpty(loadPath))
            {
                result = null;
                return;
            }

            AssetBundleObject bundle = bundles.Load(loadPath);
            if (null != bundle)
            {
                if (type == typeof(SceneAsset))
                {
                    result = SceneAsset.Create(path);
                }
                else
                {
                    result = bundle.Result.LoadAsset(path, type);
                }
            }
            else
            {
                result = null;
            }
        }

        protected override IAssetAsyncRequest CreateAsyncRequest(string path, Type type)
        {
            BundleAsyncRequest asyncRequest = AssetObject.RefPool.Pop<BundleAsyncRequest>();
            string loadPath = GetLoadPath(path);
            asyncRequest.Initial(path, type, loadPath, OnLoadAsyncCompleted, bundles);
            return asyncRequest;
        }
    }
}