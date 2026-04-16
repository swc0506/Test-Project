#if VIRTUAL
using System.Net.Sockets;

namespace Core.Net
{
    public class VirtualConnection : Connection
    {
        private VirtualServer server;

        protected override void StartConnect(string host, int port)
        {
            string id = host + port;
            StartConnect(id);
        }

        protected override void StartConnect(string url)
        {
            server = VirtualServerManager.Instance.GetVirtualServer(url);
            if (null != server)
            {
                server.OnChannelActive(this);
                OnConnectCompleted(SocketError.Success);
            }
            else
            {
                OnConnectCompleted(SocketError.ConnectionRefused);
            }
        }

        private void OnConnectCompleted(SocketError error)
        {
            state = SocketState.None;
            if (error == SocketError.Success)
            {
                state = SocketState.Connected;
            }

            OnConnectEvent?.Invoke((int)error);
        }

        protected override void OnSend(byte[] buffs, int offset, int length)
        {
            server.OnReceive(this, buffs, offset, length);
        }

        protected override void OnConnectTimeOut()
        {
            OnConnectEvent?.Invoke((int)SocketError.TimedOut);
        }

        public void OnReceiveCompleted(byte[] buffs, int offset, int length)
        {
            OnReceivedEvent?.Invoke(buffs, offset, length);
        }

        public override void Disconnect()
        {
            if (null != server)
            {
                state = SocketState.Disconnect;
                server.OnChannelInactive(this);
                server = null;
            }
        }
    }
}

#endif