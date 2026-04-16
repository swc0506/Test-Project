using UnityEngine;

namespace Core.FS
{
    public class FileBundleRequest : AbstractBundleRequest
    {
        public override void Request()
        {
            ulong offset = AssetBundlePackage.GetOffset(loadPath);
            AsyncOperation request = AssetBundle.LoadFromFileAsync(loadPath, 0, offset);
            request.completed += OnRequestCompleted;
        }

        private void OnRequestCompleted(AsyncOperation operation)
        {
            AssetBundleCreateRequest request = (AssetBundleCreateRequest) operation;
            OnLoadCompleted(request.assetBundle);
        }
    }
}