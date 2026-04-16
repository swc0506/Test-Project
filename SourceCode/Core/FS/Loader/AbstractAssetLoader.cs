using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Core.FS
{
    public abstract class AbstractAssetLoader : IAssetLoader
    {
        private readonly AssetPackage cachePool;
        private readonly Dictionary<string, string> loadPathMap = new Dictionary<string, string>();

        private readonly Dictionary<string, IAssetAsyncRequest> requestMap =
            new Dictionary<string, IAssetAsyncRequest>();

        private readonly HashSet<string> newRequestSet = new HashSet<string>();
        private readonly List<IAssetAsyncRequest> taskRequests = new List<IAssetAsyncRequest>();
        private readonly HashSet<string> requestingSet = new HashSet<string>();

        protected bool immediatelyRequest;
        private int maxAsyncCount;

        public AbstractAssetLoader(AssetPackage cachePool)
        {
            this.cachePool = cachePool;
            immediatelyRequest = false;
            maxAsyncCount = -1;
        }

        private AssetObject CreateAssetObject(string path, Object result, string abName)
        {
            AssetObject asset = null;
            if (null != result)
            {
                asset = AssetObject.RefPool.Pop<AssetObject>();
                asset.Initial(result, path, GetAssetType(), abName, cachePool);
            }

            return asset;
        }

        protected string GetLoadPath(string path)
        {
            if (!loadPathMap.TryGetValue(path, out string loadPath))
            {
                loadPath = ParseLoadPath(path);
                loadPathMap.Add(path, loadPath);
            }

            return loadPath;
        }

        protected abstract string ParseLoadPath(string path);
        
        protected abstract AssetType GetAssetType();

        public virtual AssetObject Load(string path, Type type)
        {
            if (!cachePool.TryGet(path, out AssetObject asset))
            {
                LoadAsset(path, type, out Object result, out string abName);
                asset = CreateAssetObject(path, result, abName);
            }

            return asset;
        }

        protected abstract void LoadAsset(string path, Type type, out Object result, out string loadSign);


        public AssetAsyncHandler LoadAsync(string path, Type type, int priority)
        {
            if (!requestMap.TryGetValue(path, out var request))
            {
                request = CreateAsyncRequest(path, type);
                requestMap.Add(path, request);
                if (immediatelyRequest)
                {
                    request.AsyncOperation.priority = priority;
                    request.Request();
                }
                else
                {
                    newRequestSet.Add(path);
                }
            }

            request.AsyncOperation.priority = priority;
            AssetAsyncHandler asyncHandler = new AssetAsyncHandler(request.AsyncOperation);
            return asyncHandler;
        }

        /// <summary>
        /// 构建异步请求
        /// </summary>
        /// <returns></returns>
        protected abstract IAssetAsyncRequest CreateAsyncRequest(string path, Type type);

        protected void OnLoadAsyncCompleted(IAssetAsyncRequest request, Object result)
        {
            if (!cachePool.TryGet(request.Path, out AssetObject asset))
            {
                asset = CreateAssetObject(request.Path, result, request.LoadPath);
            }

            OnRequestCompleted(request, asset);
        }

        protected void OnRequestCompleted(IAssetAsyncRequest request, AssetObject asset)
        {
            request.AsyncOperation.Completed(asset);
            requestingSet.Remove(request.Path);
            requestMap.Remove(request.Path);
            AssetObject.RefPool.Push(request);
        }

        public void SetMaxAsyncCount(int value)
        {
            maxAsyncCount = value;
        }

        public void StopAsync()
        {
            foreach (var item in requestingSet)
            {
                //主动清除加载完成的业务监听事件
                requestMap[item].AsyncOperation.ClearCompleteEvent();
            }

            requestMap.Clear();
            newRequestSet.Clear();
            taskRequests.Clear();
            requestingSet.Clear();
        }

        public int TaskCount
        {
            get { return requestMap.Count; }
        }

        public virtual void Update(float deltaTime)
        {
            if (newRequestSet.Count > 0)
            {
                foreach (var item in newRequestSet)
                {
                    taskRequests.Add(requestMap[item]);
                }

                newRequestSet.Clear();
                taskRequests.Sort();
            }

            int taskCount = taskRequests.Count;
            if (taskCount > 0)
            {
                if (maxAsyncCount >= 0)
                {
                    taskCount = maxAsyncCount - requestingSet.Count;
                }

                while (taskCount > 0)
                {
                    taskCount--;
                    var task = taskRequests[0];
                    taskRequests.RemoveAt(0);
                    requestingSet.Add(task.Path);
                    task.Request();
                }
            }
        }

        public void Clear()
        {
            loadPathMap.Clear();
        }
    }
}