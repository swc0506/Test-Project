using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Core.FS
{
    public abstract class AbstractBundleLoader : IBundleLoader
    {
        protected AssetBundlePackage context;

        private Dictionary<string, IBundleAsyncRequest> requestMap;
        private HashSet<string> newRequestSet;
        private List<IBundleAsyncRequest> taskRequests;
        private HashSet<string> requestingSet;

        private int maxAsyncCount;

        protected AbstractBundleLoader(AssetBundlePackage context)
        {
            this.context = context;
            requestMap = new Dictionary<string, IBundleAsyncRequest>();
            newRequestSet = new HashSet<string>();
            taskRequests = new List<IBundleAsyncRequest>();
            requestingSet = new HashSet<string>();
            maxAsyncCount = -1;
        }

        private AssetBundleObject CreateBundle(AssetBundle result, string path)
        {
            AssetBundleObject bundle = null;
            if (null != result)
            {
                bundle = AssetBundleObject.RefPool.Pop<AssetBundleObject>();
                bundle.Initial(result, path, context);
            }

            return bundle;
        }

        protected string GetLoadPath(string path)
        {
            //优先从更新目录获取,再从内部目录
            if (!context.DisableUpdateBundle)
            {
                string updatePath = string.Format("{0}/{1}", context.UpdateBundlePath, path);
                if (File.Exists(updatePath))
                {
                    return updatePath;
                }
            }
            return string.Format("{0}/{1}", context.InternalBundlePath, path);
        }


        public AssetBundleObject Load(string path)
        {
            //只处理当前资源自身依赖
            if (context.TryGetDependencies(path, out var dependencies))
            {
                foreach (var item in dependencies)
                {
                    LoadBundle(item, null);
                }
            }

            var bundle = LoadBundle(path, dependencies);
            return bundle;
        }

        private AssetBundleObject LoadBundle(string path, string[] dependencies)
        {
            AssetBundleObject bundle = null;
            if (!context.TryGet(path, out bundle))
            {
                AssetBundle result = CreateLoadBundle(path);
                bundle = CreateBundle(result, path);
            }

            bundle?.RetainDependencies(dependencies);

            return bundle;
        }

        protected abstract AssetBundle CreateLoadBundle(string loadPath);


        public AssetBundleAsyncOperation LoadAsync(string path, int priority)
        {
            var request = LoadAsyncBundle(path, priority);
            if (context.TryGetDependencies(path, out var dependencies))
            {
                foreach (var item in dependencies)
                {
                    IBundleAsyncRequest depend = LoadAsyncBundle(item, priority);
                    request.AddDependent(depend, dependencies);
                }
            }

            return request.AsyncOperation;
        }

        private IBundleAsyncRequest LoadAsyncBundle(string path, int priority)
        {
            IBundleAsyncRequest request;
            if (!requestMap.TryGetValue(path, out request))
            {
                if (!context.TryGet(path, out var bundle))
                {
                    request = CreateAsyncRequest(path);
                }
                else
                {
                    request = CreateLoadedRequest(path, bundle);
                }

                requestMap.Add(path, request);
                newRequestSet.Add(request.Path);
            }

            request.AsyncOperation.priority = priority;
            return request;
        }

        protected abstract IBundleAsyncRequest CreateAsyncRequest(string path);

        protected void OnLoadAsyncCompleted(IBundleAsyncRequest request, AssetBundle result)
        {
            AssetBundleObject bundle = null;
            if (!context.TryGet(request.Path, out bundle))
            {
                bundle = CreateBundle(result, request.Path);
            }

            OnRequestCompleted(request, bundle);
        }

        private IBundleAsyncRequest CreateLoadedRequest(string path, AssetBundleObject bundle)
        {
            LoadedBundleRequest asyncRequest = AssetBundleObject.RefPool.Pop<LoadedBundleRequest>();
            asyncRequest.Initial(path, bundle, OnRequestCompleted);
            return asyncRequest;
        }

        protected void OnRequestCompleted(IBundleAsyncRequest request, AssetBundleObject bundle)
        {
            if (!request.IsCycleRequest)
            {
                bundle?.RetainDependencies(request.Dependencies);
                request.RetainCycleRequest();
            }

            requestingSet.Remove(request.Path);
            requestMap.Remove(request.Path);
            request.AsyncOperation.Completed(bundle);
            AssetBundleObject.RefPool.Push(request);
        }

        public void SetMaxAsyncCount(int value)
        {
            maxAsyncCount = value;
        }

        public void StopAllAsync()
        {
            requestMap.Clear();
            newRequestSet.Clear();
            taskRequests.Clear();
            requestingSet.Clear();
        }

        public void Update(float deltaTime)
        {
            CheckEnqueueRequests();
        }

        private void CheckEnqueueRequests()
        {
            if (newRequestSet.Count > 0)
            {
                foreach (var item in newRequestSet)
                {
                    var request = requestMap[item];
                    //保证依赖完成后再加载自己
                    if (request.IsReady)
                    {
                        taskRequests.Add(requestMap[item]);
                    }
                }

                if (taskRequests.Count > 0)
                {
                    foreach (var item in taskRequests)
                    {
                        newRequestSet.Remove(item.Path);
                    }

                    taskRequests.Sort();
                }
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
                    var request = taskRequests[0];
                    taskRequests.RemoveAt(0);
                    requestingSet.Add(request.Path);
                    request.Request();
                }
            }
        }
    }
}