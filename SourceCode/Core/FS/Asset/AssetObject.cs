using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.FS
{
    public enum AssetType
    {
        Unknown,
        Resources,
        Editor,
        AssetBundle
    }

    public sealed class AssetObject : IReferenceCount, IClearable, IDisposable
    {
        private static readonly ReferencePool refPool = new ReferencePool();

        public static ReferencePool RefPool
        {
            get { return refPool; }
        }

        public static float DelayTime = 5;

        /// <summary>
        /// 加载的原始资源对象
        /// </summary>
        public Object Result { get; private set; }

        public string Path { get; private set; }
        public AssetType AssetType { get; private set; }
        private string abName;
        private IAssetCachePool<AssetObject> cachePool;

        private bool needInstantiate;
        internal bool DontUnload { get; private set; }
        internal int RefCount { get; private set; }
        private Dictionary<int, WeakReference> requireMap;
        private List<int> removeRequires;
        private bool isDispose;
        private bool isUnloadBundle;

        public void Initial(Object result, string path, AssetType assetType, string abName,
            IAssetCachePool<AssetObject> cachePool)
        {
            Result = result;
            Path = path;
            this.AssetType = assetType;
            if (assetType == AssetType.AssetBundle)
            {
                this.abName = abName;
                if (((AssetPackage)cachePool).Bundles.TryGet(abName, out var bundle))
                {
                    bundle.Retain();
                }
            }
            else
            {
                this.abName = null;
            }

            this.cachePool = cachePool;
            this.cachePool.Push(this);

            needInstantiate = Result is GameObject;
            DontUnload = false;
            RefCount = 0;
            isDispose = false;
        }

        /// <summary>
        /// 是否已经没有使用了
        /// </summary>
        public bool IsUnused
        {
            get { return RefCount <= 0 && !DontUnload; }
        }

        void IReferenceCount.AddRef()
        {
            RefCount++;
        }

        void IReferenceCount.DecRef()
        {
            RefCount--;
        }

        void IReferenceCount.DecRefCount(int count)
        {
            RefCount -= count;
        }

        public void SetDontUnload(bool dontUnload)
        {
            this.DontUnload = dontUnload;
        }

        /// <summary>
        /// 释放引用计数 并且尝试卸载
        /// </summary>
        public void Release()
        {
            if (RefCount > 0)
            {
                ((IReferenceCount)this).DecRef();
                if (IsUnused)
                {
                    ((IDisposable)this).Dispose();
                }
            }
        }

        /// <summary>
        /// 延迟释放引用计数 并且尝试卸载
        /// </summary>
        /// <param name="delayTime"></param>
        public void DelayRelease(float delayTime)
        {
            if (RefCount > 0)
            {
                cachePool.DelayDecRef(this, delayTime);
            }
        }

        public void DelayRelease()
        {
            DelayRelease(DelayTime);
        }

        /// <summary>
        /// 获取资源对象 并且引用计数+1 注：实例化类型的返回的是实例化之后的对象
        /// </summary>
        /// <returns></returns>
        public Object Get()
        {
            Object result = null;
            if (needInstantiate)
            {
                if (null != Result)
                {
                    result = GameObject.Instantiate(Result);
                }
            }
            else
            {
                result = Result;
            }

            if (null == result)
            {
                Logger.WarnFormat("result is null:{0}", Path);
            }

            ((IReferenceCount)this).AddRef();
            return result;
        }

        /// <summary>
        /// 获取资源对象 并且引用计数+1 注：实例化类型的返回的是实例化之后的对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>资源对象</returns>
        public T Get<T>() where T : Object
        {
            return Get() as T;
        }

        /// <summary>
        /// 获取资源对象结果 根据owner自动处理引用计数 注：实例化类型的返回的是实例化之后的对象
        /// </summary>
        /// <param name="owner">拥有者</param>
        /// <returns>资源对象</returns>
        public Object Require(object owner)
        {
            if (!CheckValidOwner(owner))
            {
                return null;
            }

            Object result = Get();
            if (needInstantiate)
            {
                owner = result;
            }

            RetainOwner(owner);
            return result;
        }

        private bool CheckValidOwner(object owner)
        {
            if (null == owner)
            {
                if (Application.isEditor)
                {
                    throw new NullReferenceException("asset require owner is null");
                }

                return false;
            }

            return true;
        }

        private void RetainOwner(object owner)
        {
            if (null == requireMap)
            {
                requireMap = new Dictionary<int, WeakReference>();
                removeRequires = new List<int>();
            }

            int instId = owner.GetHashCode();
            if (!requireMap.ContainsKey(instId))
            {
                WeakReference weakRef = new WeakReference(owner);
                requireMap.Add(instId, weakRef);
                ((IReferenceCount)this).AddRef();
            }
        }

        /// <summary>
        /// 获取资源对象 根据owner自动处理引用计数 注：实例化类型的返回的是实例化之后的对象
        /// </summary>
        /// <param name="owner">拥有者</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>资源对象</returns>
        public T Require<T>(object owner) where T : Object
        {
            return Require(owner) as T;
        }

        /// <summary>
        /// 释放资源对象 根据owner自动处理引用计数 注：实例化类型的返回的是实例化之后的对象
        /// </summary>
        /// <param name="owner">拥有者</param>
        public void Unrequire(object owner)
        {
            if (null == requireMap || !CheckValidOwner(owner))
            {
                return;
            }

            int instId = owner.GetHashCode();
            if (requireMap.Remove(instId))
            {
                ((IReferenceCount)this).DecRef();
            }
        }

        public bool CheckInvalidOwners()
        {
            if (null != requireMap)
            {
                foreach (var item in requireMap)
                {
                    if (!item.Value.IsAlive)
                    {
                        removeRequires.Add(item.Key);
                    }
                }

                if (removeRequires.Count > 0)
                {
                    foreach (var item in removeRequires)
                    {
                        requireMap.Remove(item);
                        ((IReferenceCount)this).DecRef();
                    }

                    removeRequires.Clear();
                }
            }

            return IsUnused;
        }

        public Object[] LoadBundleAllAssets()
        {
            if (AssetType == AssetType.AssetBundle && !string.IsNullOrEmpty(abName))
            {
                if (((AssetPackage)cachePool).Bundles.TryGet(abName, out var bundleObject))
                {
                    return bundleObject.Result.LoadAllAssets();
                }
            }

            return null;
        }

        public void LoadAsyncBundleAllAssets(Type type, Action<Object[]> callback)
        {
            if (this.AssetType == AssetType.AssetBundle && !string.IsNullOrEmpty(abName))
            {
                if (((AssetPackage)cachePool).Bundles.TryGet(abName, out var bundleObject))
                {
                    var abRequest = bundleObject.Result.LoadAllAssetsAsync(type);
                    abRequest.completed += (req) => { callback?.Invoke(((AssetBundleRequest)req).allAssets); };
                    return;
                }
            }

            callback?.Invoke(null);
        }

        public void LoadAsyncBundleAllAssets<T>(Action<Object[]> callback)
        {
            LoadAsyncBundleAllAssets(typeof(T), callback);
        }

        /// <summary>
        /// 卸载assetBundle 明确该资源没有被其他引用才调用,否则会造成其他资源依赖冗余
        /// </summary>
        public void UnloadBundle()
        {
            isUnloadBundle = true;
            if (AssetType == AssetType.AssetBundle && !string.IsNullOrEmpty(abName))
            {
                if (((AssetPackage)cachePool).Bundles.TryGet(abName, out var bundle))
                {
                    bundle.Dispose(false);
                }

                abName = null;
            }
        }

        void IClearable.Clear()
        {
            if (AssetType == AssetType.AssetBundle)
            {
                if (!string.IsNullOrEmpty(abName))
                {
                    if (((AssetPackage)cachePool).Bundles.TryGet(abName, out var bundle))
                    {
                        bundle.Release();
                    }
                }
            }
            else if (AssetType == AssetType.Editor || AssetType == AssetType.Resources)
            {
                if (!needInstantiate && !isUnloadBundle)
                {
                    Resources.UnloadAsset(Result);
                }
            }

            cachePool.Pop(this);
            if (Result is IDisposable dispose)
            {
                dispose.Dispose();
            }

            Result = null;
            Path = null;
            AssetType = AssetType.Unknown;
            abName = null;
            cachePool = null;

            needInstantiate = false;
            DontUnload = false;
            RefCount = 0;
            requireMap?.Clear();
            removeRequires?.Clear();
            isUnloadBundle = false;
        }

        void IDisposable.Dispose()
        {
            if (!isDispose)
            {
                isDispose = true;
                refPool.Push(this);
                return;
            }

            throw new InvalidOperationException("Asset already dispose");
        }

        public override string ToString()
        {
            int requireCount = null == requireMap ? 0 : requireMap.Count;
            return string.Format("Path:{0},AssetMode:{1},RefCount:{2},RequireCount:{3}", Path, AssetType, RefCount,
                requireCount);
        }
    }
}