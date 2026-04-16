using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.FS
{
    public class AssetBundleObject : IReferenceCount, IClearable
    {
        //因为assetbundle全局只能load一份,又不能停止异步加载,防止上层加载结束后没有保存到,所以静态保存
        private static readonly ReferencePool refPool = new ReferencePool();

        public static ReferencePool RefPool
        {
            get { return refPool; }
        }


        public AssetBundle Result { get; private set; }
        public string Path { get; private set; }
        private bool dontUnload;
        internal int RefCount { get; private set; }
        private HashSet<string> dependencySet;
        private float loadedTime;
        private bool unloadAllLoadedObjects;
        private bool isDispose;
        private HashSet<string> cycleRefSet;
        private IAssetCachePool<AssetBundleObject> cachePool;

        public void Initial(AssetBundle result, string path, IAssetCachePool<AssetBundleObject> cachePool)
        {
            Result = result;
            Path = path;
            this.cachePool = cachePool;
            this.cachePool.Push(this);

            RefCount = 0;
            loadedTime = Time.time;
            isDispose = false;
        }

        /// <summary>
        /// 保持依赖引用计数
        /// </summary>
        /// <param name="dependencies">依赖路径</param>
        public void RetainDependencies(string[] dependencies)
        {
            if (null != dependencies && (null == dependencySet || dependencySet.Count != dependencies.Length))
            {
                if (null == dependencySet)
                {
                    dependencySet = new HashSet<string>();
                }
                else
                {
                    dependencySet.Clear();
                }

                foreach (var item in dependencies)
                {
                    dependencySet.Add(item);
                    if (!cachePool.TryGet(item, out AssetBundleObject dependency))
                    {
                        Logger.WarnFormat("{0} RetainDependencies error,dependency is null:{1}", Path, item);
                        continue;
                    }

                    dependency.Retain();
                    //CheckCycleRef
                    CheckCycleRef(dependency);
                }
            }
        }

        /// <summary>
        /// 是否已经没有使用了
        /// </summary>
        public bool IsUnused
        {
            get { return !dontUnload && RefCount - GetCycleRefCount() == 0; }
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

        /// <summary>
        /// 增加引用计数
        /// </summary>
        public void Retain()
        {
            ((IReferenceCount)this).AddRef();
        }

        /// <summary>
        /// 减少引用计数
        /// </summary>
        public void Release()
        {
            if (RefCount > 0)
            {
                ((IReferenceCount)this).DecRef();
                if (IsUnused)
                {
                    Dispose();
                }
            }
        }

        private int GetCycleRefCount()
        {
            return null == cycleRefSet ? 0 : cycleRefSet.Count;
        }


        private void ReleaseDependencies()
        {
            foreach (var item in dependencySet)
            {
                if (!cachePool.TryGet(item, out AssetBundleObject dependency))
                {
                    if (null == cycleRefSet || !cycleRefSet.Contains(item))
                    {
                        Logger.WarnFormat("{0} ReleaseDependencies error, dependency is null:{1}", Path, item);
                    }

                    continue;
                }

                dependency.Release();
            }
        }

        private void CheckCycleRef(AssetBundleObject dependency)
        {
            if (null != dependency.dependencySet && dependency.dependencySet.Contains(Path))
            {
                dependency.LinkCycleRef(Path);
                LinkCycleRef(dependency.Path);
            }
        }

        private void LinkCycleRef(string dependent)
        {
            if (null == cycleRefSet)
            {
                cycleRefSet = new HashSet<string>();
            }

            cycleRefSet.Add(dependent);
        }


        void IClearable.Clear()
        {
            if (null != dependencySet)
            {
                ReleaseDependencies();
                dependencySet.Clear();
            }

            cachePool.Pop(this);

            Result.Unload(unloadAllLoadedObjects);
            Result = null;
            Path = null;
            cachePool = null;

            dontUnload = false;
            RefCount = 0;
            cycleRefSet?.Clear();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否卸载包内所有资源</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Dispose(bool unloadAllLoadedObjects)
        {
            if (!isDispose)
            {
                isDispose = true;
                this.unloadAllLoadedObjects = unloadAllLoadedObjects;
                refPool.Push(this);
                return;
            }

            throw new InvalidOperationException("AssetBundle already dispose");
        }

        /// <summary>
        /// 释放资源 包含包内所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public override string ToString()
        {
            return string.Format("Path:{0},LoadedTime:{1},RefCount:{2},CycleRefCount:{3}", Path, loadedTime, RefCount,
                GetCycleRefCount());
        }
    }
}