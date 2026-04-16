using System;
using System.Collections;
using System.Threading.Tasks;

namespace Core.FS
{
    /// <summary>
    /// 资源异步处理接口
    /// </summary>
    public struct AssetAsyncHandler : IEnumerator, IDisposable, IEquatable<AssetAsyncHandler>, IAsyncOperationHolder
    {
        private static AtomicInt atomicId = new AtomicInt();

        private AssetAsyncOperation asyncOperation;

        private AssetAsyncOperation weakRefAsync
        {
            get
            {
                if (null != asyncOperation)
                {
                    return asyncVersion == asyncOperation.Version ? asyncOperation : null;
                }

                return null;
            }
        }

        private int asyncVersion;
        public int Id { get; private set; }
        public string Path { get; private set; }
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event LoadAssetAction CompletedEvent
        {
            add
            {
                if (null != weakRefAsync)
                {
                    weakRefAsync.CompletedEvent += value;
                }
            }
            remove
            {
                if (null != weakRefAsync)
                {
                    weakRefAsync.CompletedEvent -= value;
                }
            }
        }

        /// <summary>
        /// 资源对象
        /// </summary>
        public AssetObject Asset
        {
            get { return null != weakRefAsync ? weakRefAsync.Result : null; }
        }

        AssetAsyncOperation IAsyncOperationHolder.AsyncOperation
        {
            get { return weakRefAsync; }
        }

        public AssetAsyncHandler(AssetAsyncOperation asyncOperation) : this()
        {
            this.asyncOperation = asyncOperation;
            asyncVersion = asyncOperation.Version;
            Id = atomicId.IncrementAndGet();
            Path = asyncOperation.Path;
            IsCompleted = false;
            asyncOperation.Retain(this);
            asyncOperation.resultAction += OnAsyncResult;
        }

        /// <summary>
        /// async操作的task
        /// </summary>
        public Task<AssetObject> Task
        {
            get { return null != asyncOperation ? asyncOperation.Task : null; }
        }

        /// <summary>
        /// 增加加载完成回调
        /// </summary>
        /// <param name="callback">回调方法</param>
        public void AddCompleted(LoadAssetAction callback)
        {
            CompletedEvent += callback;
        }

        /// <summary>
        /// 移除加载完成回调
        /// </summary>
        /// <param name="callback">回调方法</param>
        public void RemoveCompleted(LoadAssetAction callback)
        {
            CompletedEvent -= callback;
        }

        /// <summary>
        /// 异步加载完成后尝试主动释放
        /// </summary>
        /// <param name="path"></param>
        /// <param name="asset"></param>
        private void OnAsyncResult(AssetObject asset, string path)
        {
            if (!weakRefAsync.IsFocusReference)
            {
                Dispose();
            }
        }

        /// <summary>
        /// 协程接口
        /// </summary>
        /// <returns></returns>
        bool IEnumerator.MoveNext()
        {
            if (null != weakRefAsync)
            {
                return weakRefAsync.MoveNext();
            }

            return false;
        }

        object IEnumerator.Current
        {
            get { return null; }
        }

        void IEnumerator.Reset()
        {
        }

        public void Dispose()
        {
            if (null != weakRefAsync && weakRefAsync.Release(this))
            {
                IsCompleted = true;
                return;
            }

            throw new InvalidOperationException("AssetAsyncHandler already release " + ToString());
        }

        public bool Equals(AssetAsyncHandler other)
        {
            return Id == other.Id && Path == other.Path;
        }

        public override bool Equals(object obj)
        {
            return obj is AssetAsyncHandler other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("Path:{0},Url:{1}", Path, Id);
        }
    }
}