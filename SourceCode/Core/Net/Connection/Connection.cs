using System;

namespace Core.Net
{
    internal enum ProtoType
    {
        //数据包
        DATA = 0,

        //心跳
        HEARTBEAT = 1,

        //握手
        HANDSHAKE = 2,
    }

    public abstract class Connection
    {
        protected enum SocketState
        {
            None,
            Connecting,
            Connected,
            Disconnect
        }

        //最大数据
        public const int MAX_BUFF_SIZE = 1024 * 1024 * 1;

        //超时时间 单位秒
        protected const int TIMEOUT = 6;

        protected SocketState state;
        private float startConnectTime;

        public Action<string> OnErrorEvent;
        public Action<int> OnConnectEvent;
        public Action<byte[], int, int> OnReceivedEvent;
        public Action OnDisconnectEvent;

        public void Connect(string host, int port)
        {
            if (state == SocketState.Connecting || state == SocketState.Connected)
            {
                return;
            }

            state = SocketState.Connecting;
            startConnectTime = TimeUtils.GetUnityNow();
            try
            {
                StartConnect(host, port);
            }
            catch (Exception e)
            {
                state = SocketState.None;
                OnError(e.ToString());
            }
        }

        public void Connect(string url)
        {
            if (state == SocketState.Connecting || state == SocketState.Connected)
            {
                return;
            }

            state = SocketState.Connecting;
            startConnectTime = TimeUtils.GetUnityNow();
            try
            {
                StartConnect(url);
            }
            catch (Exception e)
            {
                state = SocketState.None;
                OnError(e.ToString());
            }
        }

        protected abstract void StartConnect(string host, int port);

        protected abstract void StartConnect(string url);

        public bool Send(byte[] buffs)
        {
            if (null != buffs && buffs.Length > 0 && state == SocketState.Connected)
            {
                OnSend(buffs, 0, buffs.Length);
                return true;
            }

            return false;
        }

        protected abstract void OnSend(byte[] buffs, int offset, int length);

        protected void OnError(string error)
        {
            OnErrorEvent?.Invoke(error);
        }

        public void Update(float deltaTime)
        {
            if (state == SocketState.Connecting)
            {
                if (TimeUtils.GetUnityElapse(startConnectTime) >= TIMEOUT)
                {
                    OnConnectTimeOut();
                    Disconnect();
                    state = SocketState.None;
                }
            }

            this.OnUpdate(deltaTime);
        }

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        protected abstract void OnConnectTimeOut();

        public bool IsConnected()
        {
            return state == SocketState.Connected;
        }

        public abstract void Disconnect();
    }
}