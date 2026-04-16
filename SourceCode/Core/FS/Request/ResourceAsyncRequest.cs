using UnityEngine;

namespace Core.FS
{
    internal class ResourceAsyncRequest : AbstractAssetRequest
    {
        public override void Request()
        {
            if (LoadType == typeof(SceneAsset))
            {
                SceneAsset sceneAsset = SceneAsset.CreateFromBuildSettings(LoadPath);
                OnLoadCompleted(sceneAsset);
            }
            else
            {
                ResourceRequest request = Resources.LoadAsync(LoadPath, LoadType);
                request.completed += OnResourcesCompleted;
            }
        }

        private void OnResourcesCompleted(AsyncOperation operation)
        {
            ResourceRequest request = (ResourceRequest) operation;
            OnLoadCompleted(request.asset);
        }
    }
}