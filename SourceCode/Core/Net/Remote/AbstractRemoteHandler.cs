namespace Core.Net
{
    public abstract class AbstractRemoteHandler : IRemoteHandler
    {
        protected Remote remote { get; private set; }

        public AbstractRemoteHandler(Remote remote)
        {
            this.remote = remote;
        }

        public void OnConnect(int error)
        {
            if (error == 0)
            {
                OnConnectSuccess();
            }
            else
            {
                OnConnectFail(error);
            }
        }

        protected abstract void OnConnectSuccess();

        protected abstract void OnConnectFail(int error);

        public virtual void OnDisconnect()
        {
            Logger.DebugFormat("{0}:OnDisconnect", remote);
        }

        public virtual void OnError(string errorMsg)
        {
            Logger.WarnFormat("{0}:OnError:{1}", remote, errorMsg);
        }

        public virtual void OnReconnect(int cur, int totalCount)
        {
            Logger.DebugFormat("{0}:OnReconnect:{1}/{2}", remote, cur, totalCount);
        }

        public virtual void OnReceive(MessagePacket packet)
        {
            Logger.DebugFormat("{0}:OnReceive:{1}", remote, packet);
        }

        public virtual void OnReceiveHeartbeat(long timestamp)
        {
            this.remote.OnReceiveHeartbeat(timestamp);
        }

        public virtual void Dispose()
        {
            remote = null;
        }
    }
}