using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.FS
{
    public abstract class AbstractBundleRequest : IBundleAsyncRequest
    {
        public string Path { get; protected set; }
        public AssetBundleAsyncOperation AsyncOperation { get; private set; }
        protected string loadPath;
        private Action<IBundleAsyncRequest, AssetBundle> loadCallback;
        protected AssetBundlePackage context;

        private HashSet<string> dependencySet;
        private Dictionary<string, string[]> cycleRequestMap;

        public string[] Dependencies { get; private set; }
        public bool IsCycleRequest { get; private set; }

        //依赖剩余数量
        private int dependencyRemain;

        public bool IsReady
        {
            //子依赖剩余数
            get { return dependencyRemain == 0; }
        }

        public void Initial(string path, string loadPath,
            Action<IBundleAsyncRequest, AssetBundle> loadCallback, AssetBundlePackage context)
        {
            this.Path = path;
            this.loadPath = loadPath;
            this.loadCallback = loadCallback;
            this.context = context;
            CreateAsyncOperation();
        }

        protected void CreateAsyncOperation()
        {
            AsyncOperation = AssetBundleObject.RefPool.Pop<AssetBundleAsyncOperation>();
            AsyncOperation.Initial(Path, this);
        }

        public bool ContainsDependent(string path)
        {
            return null != dependencySet && dependencySet.Contains(path);
        }

        public void AddDependent(IBundleAsyncRequest dependent, string[] dependencies)
        {
            Dependencies = dependencies;
            if (null == dependencySet)
            {
                dependencySet = new HashSet<string>();
            }

            if (dependencySet.Add(dependent.Path))
            {
                //出现循环依赖加载
                if (dependent.ContainsDependent(Path))
                {
                    IsCycleRequest = true;
                    dependent.MarkCycleRequest(Path, Dependencies);
                }
                else
                {
                    dependencyRemain++;
                    dependent.AsyncOperation.ResultEvent += OnDependentCompleted;
                }
            }
        }

        private void OnDependentCompleted(string path, AssetBundleObject bundle)
        {
            dependencyRemain--;
        }

        public void MarkCycleRequest(string path, string[] dependencies)
        {
            if (null == cycleRequestMap)
            {
                cycleRequestMap = new Dictionary<string, string[]>();
            }

            if (!cycleRequestMap.ContainsKey(path))
            {
                cycleRequestMap.Add(path, dependencies);
            }
        }

        public void RetainCycleRequest()
        {
            if (null != cycleRequestMap)
            {
                foreach (var item in cycleRequestMap)
                {
                    if (context.TryGet(item.Key, out var bundle))
                    {
                        bundle.RetainDependencies(item.Value);
                    }
                    else
                    {
                        Logger.WarnFormat("RetainCycleRequest AssetBundleObject is null,path:{0}", item.Key);
                    }
                }
            }
        }

        public abstract void Request();

        protected void OnLoadCompleted(AssetBundle obj)
        {
            loadCallback.Invoke(this, obj);
        }

        public virtual void Clear()
        {
            dependencyRemain = 0;
            loadPath = null;
            loadCallback = null;
            context = null;
            AsyncOperation = null;
            Dependencies = null;
            IsCycleRequest = false;
            if (null != dependencySet)
            {
                dependencySet.Clear();
            }

            if (null != cycleRequestMap)
            {
                cycleRequestMap.Clear();
            }
        }

        public int CompareTo(IBundleAsyncRequest other)
        {
            return AsyncOperation.CompareTo(other.AsyncOperation);
        }

        public bool Equals(IBundleAsyncRequest other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Path == other.Path;
        }
    }
}