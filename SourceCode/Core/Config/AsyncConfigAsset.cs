using System;
using Core.FS;

namespace Core.Config
{
    public delegate void LoadConfigAction(string name, Type type, bool result);

    struct AsyncConfigAsset : IEquatable<AsyncConfigAsset>
    {
        public readonly string path;
        public readonly Type type;
        public readonly AssetAsyncHandler handler;
        public event LoadConfigAction completedEvent;

        public AsyncConfigAsset(string path, Type type, AssetAsyncHandler handler)
        {
            this.path = path;
            this.type = type;
            this.handler = handler;
            completedEvent = null;
        }

        public AsyncConfigAsset(string path, Type type)
        {
            this.path = path;
            this.type = type;
            this.handler = new AssetAsyncHandler();
            completedEvent = null;
        }

        public void Invoke(bool result)
        {
            if (!result)
            {
                Logger.WarnFormat("Config load fail:name:{0},type:{1}", path, type);
            }

            completedEvent?.Invoke(path, type, result);
        }

        public bool Equals(AsyncConfigAsset other)
        {
            return path == other.path && Equals(type, other.type);
        }

        public override bool Equals(object obj)
        {
            return obj is AsyncConfigAsset other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (path != null ? path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (type != null ? type.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}