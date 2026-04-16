using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.FS
{
    public sealed class AssetPackage : IAssetCachePool<AssetObject>, IEnumerable<KeyValuePair<string, AssetObject>>
    {
        private static Func<AssetPackage, List<IAssetLoader>> assetLoaderFunc;

        public static void SetAssetLoaderFunc(Func<AssetPackage, List<IAssetLoader>> func)
        {
            assetLoaderFunc = func;
        }

        public enum WorkState
        {
            Idle,
            Loading,
        }

        private const int DEFAULT_PRIORITY = 1000;
        private const float MIN_UNLOAD_INTERVAL = 60;

        private static bool unloadUnusedAssets;
        private static float prevUnloadTime;

        public string Name { get; private set; }

        //所有asset的集合
        private Dictionary<string, AssetObject> assetMap = new Dictionary<string, AssetObject>();

        //延迟减少引用对集合
        private Dictionary<AssetObject, DelayDecReference> delayDecMap =
            new Dictionary<AssetObject, DelayDecReference>();

        private List<AssetObject> delayRemoves = new List<AssetObject>();

        private Dictionary<string, AssetAsyncOperation> loadedMap = new Dictionary<string, AssetAsyncOperation>();
        private HashSet<string> newLoadedSet = new HashSet<string>();
        private List<AssetAsyncOperation> taskLoaded = new List<AssetAsyncOperation>();

        private int maxAsyncCount;
        public WorkState State { get; private set; }

        private bool isAsyncLoading;
        public bool Pause { get; set; }

        public AssetBundlePackage Bundles { get; private set; }
        private bool isNeedUnloadUnused;
        private List<string> unloadAssets = new List<string>();
        private List<IAssetLoader> loaders = new List<IAssetLoader>();
        private IAssetLoader mixLoader;
        private AssetAsyncHandler invalidAsyncHandler;

        public int Count
        {
            get { return assetMap.Count; }
        }

        public AssetPackage(string pkgName)
        {
            Initial(pkgName);
            Bundles.LoadManifest();
        }

        public AssetPackage(string pkgName, Action<AssetPackage, bool> completedAction)
        {
            Initial(pkgName);
            Bundles.SetCompletedAction(completedAction);
            Bundles.LoadManifestAsync();
        }

        private void Initial(string pkgName)
        {
            Name = pkgName;
            maxAsyncCount = -1;
            State = WorkState.Idle;
            unloadUnusedAssets = false;
            prevUnloadTime = -MIN_UNLOAD_INTERVAL;
            invalidAsyncHandler = new AssetAsyncHandler();

            Bundles = new AssetBundlePackage(this);
            CreateLoaders();
        }

        private void CreateLoaders()
        {
            if (Application.isEditor && null != assetLoaderFunc)
            {
                List<IAssetLoader> assetLoaders = assetLoaderFunc.Invoke(this);
                if (null != assetLoaders)
                {
                    loaders.AddRange(assetLoaders);
                }
            }
            else
            {
                loaders.Add(new BundleAssetLoader(this, Bundles));
            }

            loaders.Add(new ResourceAssetLoader(this));
            mixLoader = new MixAssetLoader(this, loaders);
        }

        private IAssetLoader GetLoader()
        {
            return mixLoader;
        }

        void IAssetCachePool<AssetObject>.Push(AssetObject asset)
        {
            assetMap.Add(asset.Path, asset);
        }

        void IAssetCachePool<AssetObject>.Pop(AssetObject asset)
        {
            if (delayDecMap.TryGetValue(asset, out var decRef))
            {
                DelayDecReference.RefPool.Push(decRef);
                delayDecMap.Remove(asset);
            }

            assetMap.Remove(asset.Path);
        }

        void IAssetCachePool<AssetObject>.DelayDecRef(AssetObject asset, float delayTime)
        {
            if (!delayDecMap.TryGetValue(asset, out var decRef))
            {
                decRef = DelayDecReference.RefPool.Pop<DelayDecReference>();
                delayDecMap.Add(asset, decRef);
            }

            decRef.SetDelayTime(delayTime);
        }

        public bool TryGet(string path, out AssetObject asset)
        {
            if (!string.IsNullOrEmpty(path) && assetMap.TryGetValue(path, out asset))
            {
                return true;
            }

            asset = null;
            return false;
        }

        private bool CheckValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (Application.isEditor)
                {
                    throw new NullReferenceException("LoadAsset path is null");
                }

                return false;
            }

            return true;
        }

        //当前任务数
        public int TaskCount
        {
            get
            {
                int count = taskLoaded.Count + newLoadedSet.Count;
                foreach (var item in loaders)
                {
                    count += item.TaskCount;
                }

                return count;
            }
        }

        public AssetObject Load(string path, Type type)
        {
            if (!CheckValidPath(path))
            {
                return null;
            }

            return GetLoader().Load(path, type);
        }

        public AssetObject Load<T>(string path) where T : Object
        {
            return Load(path, typeof(T));
        }

        public AssetObject Load(string path)
        {
            return Load(path, typeof(Object));
        }

        public AssetAsyncHandler LoadAsync(string path, Type type, int priority)
        {
            if (!CheckValidPath(path))
            {
                return invalidAsyncHandler;
            }

            if (State == WorkState.Idle)
            {
                State = WorkState.Loading;
            }

            AssetAsyncHandler asyncHandler;
            //已经加载过的，就不走loader去处理速度会更快
            if (assetMap.TryGetValue(path, out var asset))
            {
                asyncHandler = AsyncAlreadyLoaded(path, priority, asset);
            }
            else
            {
                asyncHandler = GetLoader().LoadAsync(path, type, priority);
            }

            return asyncHandler;
        }

        private AssetAsyncHandler AsyncAlreadyLoaded(string path, int priority, AssetObject asset)
        {
            if (!loadedMap.TryGetValue(path, out AssetAsyncOperation asyncOperation))
            {
                asyncOperation = AssetObject.RefPool.Pop<AssetAsyncOperation>();
                asyncOperation.Initial(path);
                loadedMap.Add(path, asyncOperation);
                newLoadedSet.Add(path);
                //保持一下，防止在处理过程被卸载了
                ((IReferenceCount)asset).AddRef();
            }

            asyncOperation.priority = priority;
            return new AssetAsyncHandler(asyncOperation);
        }

        public AssetAsyncHandler LoadAsync<T>(string path, int priority) where T : Object
        {
            return LoadAsync(path, typeof(T), priority);
        }

        public AssetAsyncHandler LoadAsync<T>(string path)
        {
            return LoadAsync(path, typeof(T));
        }

        public AssetAsyncHandler LoadAsync(string path, Type type)
        {
            return LoadAsync(path, type, DEFAULT_PRIORITY);
        }

        public AssetAsyncHandler LoadAsync(string path)
        {
            return LoadAsync(path, typeof(Object));
        }

        public AssetAsyncHandler LoadAsync(string path, int priority)
        {
            return LoadAsync(path, typeof(Object), priority);
        }

        /// <summary>
        /// 设置最大异步请求数量
        /// </summary>
        /// <param name="value">数量 -1:无限大 0:暂停</param>
        public void SetMaxAsyncCount(int value)
        {
            maxAsyncCount = value;
            foreach (var item in loaders)
            {
                item.SetMaxAsyncCount(value);
            }
        }

        /// <summary>
        /// 卸载资源 不会进行引用计数检查,可以确保资源本身一定会被释放
        /// </summary>
        /// <param name="path"></param>
        public void Unload(string path)
        {
            if (assetMap.TryGetValue(path, out var asset))
            {
                Unload(asset);
            }
        }

        /// <summary>
        /// 卸载资源 不会进行引用计数检查,可以确保资源本身一定会被释放
        /// </summary>
        /// <param name="asset">资源对象</param>
        /// <returns></returns>
        public void Unload(AssetObject asset)
        {
            if (!asset.DontUnload)
            {
                ((IDisposable)asset)?.Dispose();
            }
        }

        public void Update(float deltaTime)
        {
            if (Pause || unloadUnusedAssets)
            {
                return;
            }

            Bundles?.Update(deltaTime);
            if (State == WorkState.Loading)
            {
                CheckEnqueueLoaded();
                isAsyncLoading = taskLoaded.Count > 0;
                foreach (var item in loaders)
                {
                    item.Update(deltaTime);
                    isAsyncLoading |= item.TaskCount > 0;
                }

                if (!isAsyncLoading)
                {
                    State = WorkState.Idle;
                }
            }
            else if (State == WorkState.Idle)
            {
                CheckDecRefAssets();
                if (isNeedUnloadUnused)
                {
                    isNeedUnloadUnused = false;
                    CheckUnusedAssets();
                }
            }
        }

        private void CheckEnqueueLoaded()
        {
            if (newLoadedSet.Count > 0)
            {
                foreach (var item in newLoadedSet)
                {
                    taskLoaded.Add(loadedMap[item]);
                }

                newLoadedSet.Clear();
                taskLoaded.Sort();
            }

            int taskCount = taskLoaded.Count;
            if (taskCount > 0)
            {
                if (maxAsyncCount >= 0)
                {
                    taskCount = taskCount > maxAsyncCount ? maxAsyncCount : taskCount;
                }

                while (taskCount > 0)
                {
                    taskCount--;
                    var asyncOperation = taskLoaded[0];
                    taskLoaded.RemoveAt(0);
                    loadedMap.Remove(asyncOperation.Path);
                    if (assetMap.TryGetValue(asyncOperation.Path, out var asset))
                    {
                        asyncOperation.Completed(asset);
                        //释放之前保持的
                        ((IReferenceCount)asset).DecRef();
                    }
                }
            }
        }

        private void CheckDecRefAssets()
        {
            if (delayDecMap.Count == 0)
            {
                return;
            }

            foreach (var item in delayDecMap)
            {
                if (Time.unscaledTime > item.Value.DecRefTime)
                {
                    if (item.Value.Count >= item.Key.RefCount)
                    {
                        unloadAssets.Add(item.Key.Path);
                    }
                    else
                    {
                        ((IReferenceCount)item.Key).DecRefCount(item.Value.Count);
                    }

                    delayRemoves.Add(item.Key);
                }
            }

            if (delayRemoves.Count > 0)
            {
                foreach (var item in delayRemoves)
                {
                    delayDecMap.Remove(item);
                }

                delayRemoves.Clear();

                UnloadAssets();
            }
        }

        private void CheckUnusedAssets()
        {
            //check unused assets
            foreach (var item in assetMap)
            {
                if (item.Value.IsUnused || item.Value.CheckInvalidOwners())
                {
                    unloadAssets.Add(item.Key);
                }
            }

            UnloadAssets();
            ResourcesUnloadUnused();
        }

        private void UnloadAssets()
        {
            if (unloadAssets.Count > 0)
            {
                foreach (var item in unloadAssets)
                {
                    Unload(assetMap[item]);
                }

                unloadAssets.Clear();
            }
        }

        /// <summary>
        /// 卸载所有未使用的资源
        /// </summary>
        public void UnloadUnused()
        {
            isNeedUnloadUnused = true;
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        public void UnloadAll(bool containsDontUnload)
        {
            //unload all assets
            foreach (var item in assetMap)
            {
                if (containsDontUnload && item.Value.DontUnload)
                {
                    item.Value.SetDontUnload(false);
                }

                unloadAssets.Add(item.Key);
            }

            UnloadAssets();
        }

        public void UnloadAll()
        {
            UnloadAll(false);
        }

        /// <summary>
        /// 停止所有异步加载
        /// </summary>
        public void StopAsync()
        {
            //stop async load
            loadedMap.Clear();
            newLoadedSet.Clear();
            taskLoaded.Clear();
            foreach (var item in loaders)
            {
                item.StopAsync();
            }
        }

        private void ClearLoaded()
        {
            foreach (var item in assetMap)
            {
                item.Value.UnloadBundle();
            }

            UnloadAll(true);
            foreach (var item in loaders)
            {
                item.Clear();
            }
        }

        public void Reload()
        {
            ClearLoaded();
            Bundles.LoadManifest();
        }

        public void ReloadAsync(Action<AssetPackage, bool> completedAction)
        {
            ClearLoaded();
            Bundles.SetCompletedAction(completedAction);
            Bundles.LoadManifestAsync();
        }

        /// <summary>
        /// 停止异步加载并且清除所有资源
        /// </summary>
        public void Clear()
        {
            StopAsync();
            UnloadAll();
            ResourcesUnloadUnused();
        }

        private static void ResourcesUnloadUnused()
        {
            if (!unloadUnusedAssets && Time.unscaledTime - prevUnloadTime >= MIN_UNLOAD_INTERVAL)
            {
                unloadUnusedAssets = true;
                prevUnloadTime = Time.unscaledTime;

                AsyncOperation unloadOperation = Resources.UnloadUnusedAssets();
                unloadOperation.completed += OnResourcesUnloadUnused;
            }
        }

        private static void OnResourcesUnloadUnused(AsyncOperation asyncOperation)
        {
            Logger.DebugFormat("Resources.UnloadUnusedAssets End:{0}", Time.unscaledTime);
            unloadUnusedAssets = false;
        }

        public IEnumerator<KeyValuePair<string, AssetObject>> GetEnumerator()
        {
            return assetMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}