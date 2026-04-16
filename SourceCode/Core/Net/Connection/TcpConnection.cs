#if TCP
using System;
using System.Net;
using System.Net.Sockets;

namespace Core.Net
{
    public class TcpConnection : Connection
    {
        private Socket socket;

        //接收缓存
        private readonly byte[] recvBuff;
        private SocketAsyncEventArgs recvEventArg;
        private SocketAsyncEventArgs sendEventArg;

        public TcpConnection()
        {
            recvBuff = new byte[MAX_BUFF_SIZE];
        }

        protected override void StartConnect(string host, int port)
        {
            if (!NetUtils.TryParseHost(host, out IPAddress address))
            {
                return;
            }

            Array.Clear(recvBuff, 0, recvBuff.Length);
            recvEventArg = new SocketAsyncEventArgs();
            recvEventArg.Completed += OnReceiveCompleted;
            recvEventArg.SetBuffer(recvBuff, 0, recvBuff.Length);

            socket = new Socket(address.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            socket.NoDelay = true;
            SocketAsyncEventArgs connectEventArg = new SocketAsyncEventArgs();
            connectEventArg.Completed += OnConnectCompleted;
            connectEventArg.RemoteEndPoint = new IPEndPoint(address, port);
            //连接服务器
            bool willRaiseEvent = socket.ConnectAsync(connectEventArg);
            if (!willRaiseEvent)
            {
                ProcessConnect(connectEventArg);
            }
        }

        protected override void StartConnect(string url)
        {
            throw new NotImplementedException();
        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            ProcessConnect(args);
        }

        private void ProcessConnect(SocketAsyncEventArgs args)
        {
            state = SocketState.None;
            //链接成功开始接收数据
            if (args.SocketError == SocketError.Success)
            {
                state = SocketState.Connected;
                StartReceive();
            }

            OnConnectEvent?.Invoke((int) args.SocketError);
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            ProcessReceive(args);
        }

        private void ProcessReceive(SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                if (args.BytesTransferred > 0)
                {
                    OnReceivedEvent?.Invoke(args.Buffer, args.Offset, args.BytesTransferred);
                    StartReceive();
                }
                else
                {
                    CloseConnect();
                }
            }
            else
            {
                //链接被异常断开了
                ProcessException(args.SocketError.ToString());
            }
        }

        private void StartReceive()
        {
            try
            {
                bool willRaiseEvent = socket.ReceiveAsync(recvEventArg);
                if (!willRaiseEvent)
                {
                    ProcessReceive(recvEventArg);
                }
            }
            catch (Exception e)
            {
                ProcessException(e.ToString());
            }
        }

        private void ProcessException(string error)
        {
            if (null != socket)
            {
                OnError(error);
                CloseConnect();
            }
        }

        private void CloseConnect()
        {
            Disconnect();
            OnDisconnectEvent?.Invoke();
        }

        // public override bool IsConnected()
        // {
        //     return socket.Connected || !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
        // }

        protected override void OnSend(byte[] buffs, int offset, int length)
        {
            if (null != socket)
            {
                try
                {
                    socket.BeginSend(buffs, offset, length, SocketFlags.None, null, socket);
                }
                catch (Exception e)
                {
                    ProcessException(e.ToString());
                }
            }
        }

        protected override void OnConnectTimeOut()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SocketError = SocketError.TimedOut;
            ProcessConnect(args);
        }

        public override void Disconnect()
        {
            if (null != socket)
            {
                try
                {
                    state = SocketState.Disconnect;
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Dispose();
                    socket = null;

                    recvEventArg.Dispose();
                    recvEventArg = null;
                    sendEventArg.Dispose();
                    sendEventArg = null;
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
#endif