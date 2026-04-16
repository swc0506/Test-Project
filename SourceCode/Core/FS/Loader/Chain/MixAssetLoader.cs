using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Core.FS
{
    /// <summary>
    /// 链式混合资源加载器
    /// </summary>
    internal class MixAssetLoader : AbstractAssetLoader
    {
        private IChainLoadHandler headHandler;

        public MixAssetLoader(AssetPackage cachePool, List<IAssetLoader> loaders) : base(cachePool)
        {
            immediatelyRequest = true;
            IChainLoadHandler tailHandler = null;
            for (int i = 0; i < loaders.Count; i++)
            {
                IChainLoadHandler loadHandler = new ChainLoadHandler(loaders[i]);
                if (i == 0)
                {
                    headHandler = loadHandler;
                }
                else
                {
                    tailHandler.SetNext(loadHandler);
                }

                tailHandler = loadHandler;
            }

            tailHandler.SetNext(null);
        }

        protected override string ParseLoadPath(string path)
        {
            return path;
        }

        protected override AssetType GetAssetType()
        {
            return AssetType.Unknown;
        }

        public override AssetObject Load(string path, Type type)
        {
            return headHandler.Request(path, type);
        }

        protected override void LoadAsset(string path, Type type, out Object result, out string loadSign)
        {
            result = null;
            loadSign = null;
        }

        protected override IAssetAsyncRequest CreateAsyncRequest(string path, Type type)
        {
            MixAsyncRequest asyncRequest = AssetObject.RefPool.Pop<MixAsyncRequest>();
            asyncRequest.Initial(path, type, headHandler, OnRequestCompleted);
            return asyncRequest;
        }

        public override void Update(float deltaTime)
        {
        }
    }
}