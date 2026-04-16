using System;
using System.Collections.Generic;

namespace Core.FS
{
    public class AssetBundleAsyncOperation : IComparable<AssetBundleAsyncOperation>,
        IEquatable<AssetBundleAsyncOperation>, IClearable
    {
        private static readonly AtomicInt atomicId = new AtomicInt();

        private static readonly Dictionary<int, AssetBundleAsyncOperation> aliveMap =
            new Dictionary<int, AssetBundleAsyncOperation>();
        
        
        private int version;
        public string Path { get; private set; }
        public IBundleAsyncRequest Request { get; private set; }

        internal int priority;

        
        private Action<AssetBundleObject> completeAction;
        public event Action<AssetBundleObject> CompletedEvent
        {
            add { completeAction += value; }
            remove { completeAction -= value; }
        }

        private Action<string, AssetBundleObject> resultAction;
        public event Action<string, AssetBundleObject> ResultEvent
        {
            add { resultAction += value; }
            remove { resultAction -= value; }
        }
        

        public void Initial(string path)
        {
            Initial(path, null);
        }

        public void Initial(string path, IBundleAsyncRequest request)
        {
            version = atomicId.IncrementAndGet();
            aliveMap.Add(version, this);
            Path = path;
            Request = request;
        }

        public void Completed(AssetBundleObject bundle)
        {
            completeAction?.Invoke(bundle);
            resultAction?.Invoke(Path, bundle);
            AssetBundleObject.RefPool.Push(this);
        }

        public int CompareTo(AssetBundleAsyncOperation other)
        {
            if (priority == other.priority)
            {
                return version.CompareTo(other.version);
            }

            return priority.CompareTo(other.priority);
        }

        public bool Equals(AssetBundleAsyncOperation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Path == other.Path;
        }

        public void Clear()
        {
            aliveMap.Remove(version);
            version = -1;
            Path = null;
            priority = 0;
            completeAction = null;
            resultAction = null;
        }
    }
}