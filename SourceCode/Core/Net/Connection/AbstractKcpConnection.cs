#if KCP
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Core.Net
{
    public abstract class AbstractKcpConnection : Connection
    {
        private readonly IProtocolCoder protocolCoder;
        protected readonly uint conv;

        //接受缓存数组
        private byte[] recvBytes;
        protected MemoryStream memoryStream;
        protected ByteBuffer recvBuffer;
        private float startConnectTime;

        private SocketAsyncEventArgs recvEventArg;
        protected Socket socket;

        protected uint elapseTime;

        public AbstractKcpConnection(IProtocolCoder protocolCoder, uint conv)
        {
            this.protocolCoder = protocolCoder;
            this.conv = conv;

            recvBytes = new byte[MAX_BUFF_SIZE];
            memoryStream = new MemoryStream();
            recvBuffer = new ByteBuffer();
        }

        protected override void StartConnect(string host, int port)
        {
            if (!NetUtils.TryParseHost(host, out IPAddress address))
            {
                return;
            }

            memoryStream.Flush();
            recvBuffer.Clear();
            startConnectTime = TimeUtils.GetUnityNow();

            Array.Clear(recvBytes, 0, recvBytes.Length);
            recvEventArg = new SocketAsyncEventArgs();
            recvEventArg.Completed += OnReceiveCompleted;
            recvEventArg.SetBuffer(recvBytes, 0, recvBytes.Length);

            CreateKcp();

            socket = new Socket(address.AddressFamily, SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
            socket.Connect(address,port);

            StartReceive();
            SendHandShake();
        }

        protected override void StartConnect(string url)
        {
            throw new NotImplementedException();
        }

        protected abstract void CreateKcp();

        protected abstract void ReleaseKcp();

        protected void StartReceive()
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

        private void SendHandShake()
        {
            BinaryProtocol heartProtocol = new BinaryProtocol((byte) ProtoType.HANDSHAKE);
            byte[] buffs = protocolCoder.Encode(heartProtocol);
            OnSend(buffs, 0, buffs.Length);
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            ProcessReceive(args);
        }

        protected void ProcessConnect(SocketAsyncEventArgs args)
        {
            state = SocketState.None;
            //链接成功开始接收数据
            if (args.SocketError == SocketError.Success)
            {
                state = SocketState.Connected;
            }
            else
            {
                recvBuffer.Clear();
                ReleaseKcp();
            }

            OnConnectEvent?.Invoke((int) args.SocketError);
        }

        private void ProcessReceive(SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                recvBuffer.WriteBytes(args.Buffer, args.Offset, args.BytesTransferred);
                StartReceive();
            }
            else
            {
                //链接被异常断开了
                ProcessException(args.SocketError.ToString());
            }
        }

        protected void ProcessException(string error)
        {
            if (null != socket)
            {
                OnError(error);
                Disconnect();
                OnDisconnectEvent?.Invoke();
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            TickRecvBuff();

            if (state == SocketState.Connecting || state == SocketState.Connected)
            {
                elapseTime += (uint) (deltaTime * 1000);
                UpdateKcp();
            }
        }

        private void TickRecvBuff()
        {
            int buffSize = recvBuffer.ReadableBytes();
            if (buffSize <= 0)
            {
                return;
            }

            KcpInput();
            recvBuffer.Clear();

            while (true)
            {
                int length = KcpPeekSize();
                if (length <= 0)
                {
                    break;
                }

                byte[] buffer = memoryStream.GetBuffer();
                memoryStream.SetLength(length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                int size = KcpRecv(buffer, 0, length);
                if (length != size)
                {
                    break;
                }

                if (state == SocketState.Connecting)
                {
                    //首次回包,链接成功
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.SocketError = SocketError.Success;
                    ProcessConnect(args);
                }
                else if (state == SocketState.Connected)
                {
                    OnReceivedEvent?.Invoke(buffer, 0, length);
                }
            }
        }

        protected abstract void KcpInput();

        protected abstract int KcpPeekSize();

        protected abstract int KcpRecv(byte[] buffer, int offset, int length);


        protected abstract void UpdateKcp();


        protected override void OnConnectTimeOut()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SocketError = SocketError.TimedOut;
            ProcessConnect(args);
        }

        public override void Disconnect()
        {
            if (null == socket)
            {
                return;
            }

            try
            {
                state = SocketState.Disconnect;
                socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
                socket = null;

                recvEventArg.Dispose();
                recvEventArg = null;

                memoryStream.Dispose();
                memoryStream = null;
                recvBuffer.Clear();
                recvBuffer = null;
                recvBytes = null;

                ReleaseKcp();
            }
            catch (Exception e)
            {
                OnError(e.ToString());
            }
        }
    }
}
#endif