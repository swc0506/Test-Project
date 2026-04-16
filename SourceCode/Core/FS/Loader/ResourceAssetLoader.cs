using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.FS
{
    internal class ResourceAssetLoader : AbstractAssetLoader
    {
        private const string PATH_KEYWORDS = "/Resources/";

        public ResourceAssetLoader(AssetPackage cachePool) : base(cachePool)
        {
        }

        protected override string ParseLoadPath(string path)
        {
            string loadPath = path;
            //Regex GC很大
//          loadPath = Regex.Replace(path, @"(Assets/Resources/)|(\.\w+)", string.Empty);
            int beginIndex = path.IndexOf(PATH_KEYWORDS);
            int endIndex = path.IndexOf(".");
            if (beginIndex >= 0 && endIndex > 0)
            {
                beginIndex += PATH_KEYWORDS.Length;
                loadPath = path.Substring(beginIndex, endIndex - beginIndex);
            }

            return loadPath;
        }

        protected override AssetType GetAssetType()
        {
            return AssetType.Resources;
        }

        protected override void LoadAsset(string path, Type type, out Object result, out string loadSign)
        {
            string loadPath = GetLoadPath(path);
            if (type == typeof(SceneAsset))
            {
                result = SceneAsset.CreateFromBuildSettings(loadPath);
            }
            else
            {
                result = Resources.Load(loadPath, type);
            }

            loadSign = null;
        }
        
        protected override IAssetAsyncRequest CreateAsyncRequest(string path, Type type)
        {
            ResourceAsyncRequest asyncRequest = AssetObject.RefPool.Pop<ResourceAsyncRequest>();
            string loadPath = GetLoadPath(path);
            asyncRequest.Initial(path, type, loadPath, OnLoadAsyncCompleted);
            return asyncRequest;
        }
    }
}