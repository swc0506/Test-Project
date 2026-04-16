using System;
using Object = UnityEngine.Object;

namespace Core.FS
{
    public abstract class AbstractAssetRequest : IAssetAsyncRequest
    {
        public string Path { get; protected set; }
        public Type LoadType { get; protected set; }
        public string LoadPath { get; protected set; }
        private Action<IAssetAsyncRequest, Object> loadCallback;

        public AssetAsyncOperation AsyncOperation { get; private set; }

        public void Initial(string path, Type type, string loadPath,
            Action<IAssetAsyncRequest, Object> loadCallback)
        {
            Path = path;
            LoadType = type;
            LoadPath = loadPath;
            this.loadCallback = loadCallback;
            CreateAsyncOperation();
        }

        protected void CreateAsyncOperation()
        {
            AsyncOperation = AssetObject.RefPool.Pop<AssetAsyncOperation>();
            AsyncOperation.Initial(Path);
        }

        public abstract void Request();

        protected void OnLoadCompleted(Object obj)
        {
            loadCallback.Invoke(this, obj);
        }

        public virtual void Clear()
        {
            AsyncOperation = null;
            loadCallback = null;
        }

        public int CompareTo(IAssetAsyncRequest other)
        {
            return AsyncOperation.CompareTo(other.AsyncOperation);
        }

        public bool Equals(IAssetAsyncRequest other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Path == other.Path && LoadType == other.LoadType;
        }
    }
}