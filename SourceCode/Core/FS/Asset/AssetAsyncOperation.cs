using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.FS
{
    /// <summary>
    /// 资源异步实际操作对象
    /// </summary>
    public sealed class AssetAsyncOperation : IComparable<AssetAsyncOperation>, IEquatable<AssetAsyncOperation>,
        IEnumerator, IClearable
    {
        private static readonly AtomicInt atomicId = new AtomicInt();

        private static readonly Dictionary<int, AssetAsyncOperation> aliveMap =
            new Dictionary<int, AssetAsyncOperation>();

        public static int AliveCount
        {
            get { return aliveMap.Count; }
        }

        public static Dictionary<int, AssetAsyncOperation>.Enumerator GetAliveIter()
        {
            return aliveMap.GetEnumerator();
        }

        public int Version { get; private set; }
        public string Path { get; private set; }
        public int priority;

        private LoadAssetAction completeAction;

        public event LoadAssetAction CompletedEvent
        {
            add { completeAction += value; }
            remove { completeAction -= value; }
        }

        internal LoadAssetAction resultAction;
        public AssetObject Result { get; private set; }
        public bool IsFocusReference { get; private set; }

        private readonly HashSet<int> refSet;
        private bool isDone;
        private EventWaitHandle waitHandle;

        public AssetAsyncOperation()
        {
            refSet = new HashSet<int>();
        }

        public Task<AssetObject> Task
        {
            get
            {
                IsFocusReference = true;
                if (isDone)
                {
                    return System.Threading.Tasks.Task.FromResult(Result);
                }

                if (null == waitHandle)
                {
                    waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                }

                var handle = waitHandle;
                handle.Reset();
                return System.Threading.Tasks.Task.Factory.StartNew((arg) =>
                {
                    handle.WaitOne();
                    return ((AssetAsyncOperation) arg).Result;
                }, this);
            }
        }

        public void Initial(string path)
        {
            Version = atomicId.IncrementAndGet();
            aliveMap.Add(Version, this);
            Path = path;
        }

        public void Completed(AssetObject asset)
        {
            Result = asset;
            isDone = true;
            completeAction?.Invoke(asset, Path);
            resultAction?.Invoke(asset, Path);
            waitHandle?.Set();
        }

        public void Retain(AssetAsyncHandler handler)
        {
            refSet.Add(handler.Id);
        }

        public bool Release(AssetAsyncHandler handler)
        {
            bool result = refSet.Remove(handler.Id);
            if (result)
            {
                if (refSet.Count == 0)
                {
                    AssetObject.RefPool.Push(this);
                }

                return true;
            }

            return false;
        }

        public int CompareTo(AssetAsyncOperation other)
        {
            if (priority == other.priority)
            {
                return Version.CompareTo(other.Version);
            }

            return priority.CompareTo(other.priority);
        }

        public bool MoveNext()
        {
            IsFocusReference = true;
            return !isDone;
        }

        public object Current
        {
            get { return null; }
        }

        public void Reset()
        {
        }

        public bool Equals(AssetAsyncOperation other)
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

        public void Clear()
        {
            aliveMap.Remove(Version);
            Version = -1;
            Path = null;
            priority = 0;
            completeAction = null;
            resultAction = null;
            Result = null;
            IsFocusReference = false;
            refSet.Clear();
            isDone = false;
        }

        public void ClearCompleteEvent()
        {
            completeAction = null;
            IsFocusReference = false;
        }
    }
}