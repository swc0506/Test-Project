using System;

namespace Core.FS
{
    internal class MixAsyncRequest : AbstractAssetRequest
    {
        private IChainLoadHandler loadHandler;
        private Action<IAssetAsyncRequest, AssetObject> requestEvent;
        private AssetAsyncHandler asyncHandler;

        public void Initial(string path, Type type, IChainLoadHandler loadHandler,
            Action<IAssetAsyncRequest, AssetObject> requestEvent)
        {
            Path = path;
            LoadType = type;
            this.loadHandler = loadHandler;
            this.requestEvent = requestEvent;
            CreateAsyncOperation();
        }

        public override void Request()
        {
            asyncHandler = loadHandler.RequestAsync(Path, LoadType, AsyncOperation.priority);
            ((IAsyncOperationHolder) asyncHandler).AsyncOperation.resultAction += OnAsyncResult;
        }

        private void OnAsyncResult(AssetObject asset, string path)
        {
            if (null == asset && null != loadHandler.GetNext())
            {
                loadHandler = loadHandler.GetNext();
                Request();
            }
            else
            {
                requestEvent?.Invoke(this, asset);
            }
        }
    }
}