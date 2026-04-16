using System;

namespace Core.FS
{
    internal class ChainLoadHandler : IChainLoadHandler
    {
        private IAssetLoader loader;
        private IChainLoadHandler nextHandler;

        public ChainLoadHandler(IAssetLoader loader)
        {
            this.loader = loader;
        }

        public void SetNext(IChainLoadHandler handler)
        {
            nextHandler = handler;
        }

        public IChainLoadHandler GetNext()
        {
            return nextHandler;
        }

        public AssetObject Request(string path, Type type)
        {
            AssetObject asset = loader.Load(path, type);
            if (null == asset && null != nextHandler)
            {
                asset = nextHandler.Request(path, type);
            }

            return asset;
        }

        public  AssetAsyncHandler RequestAsync(string path, Type type, int priority)
        {
            AssetAsyncHandler asyncHandler = loader.LoadAsync(path, type,priority);
            return asyncHandler;
        }
    }
}