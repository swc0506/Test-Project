using System;

namespace Core.Net
{
    public abstract class NetChannel : IDisposable
    {
        protected readonly Remote remote;

        public string Name
        {
            get { return remote.Name; }
        }

        public NetChannel(Remote remote)
        {
            this.remote = remote;
        }

        public abstract void OnReceiveMessage(MessagePacket packet);

        public void Connect(string host, int port)
        {
            remote.Connect(host, port);
        }

        public void Connect(string url)
        {
            remote.Connect(url);
        }

        public void Reconnect()
        {
            remote.Reconnect();
        }

        public void Disconnect()
        {
            remote.Disconnect();
        }

        public virtual void Dispose()
        {
            if (null != remote)
            {
                remote.Dispose();
            }
        }
    }
}