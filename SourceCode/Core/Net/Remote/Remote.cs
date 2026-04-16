using System;

namespace Core.Net
{
    public enum RemoteState
    {
        None,
        Connecting,
        Connected,
        Reconnecting,
        Disconnect,
        Close
    }

    public class Remote : IDisposable
    {
        enum ConnectMode
        {
            None,
            Host,
            Url,
        }

        enum HeartState
        {
            Ready,
            Started,
            WaitReply,
            Stop
        }

        private static readonly int MAX_RETRY_COUNT = 3;
        private static readonly int RETRY_INTERVAL = 6;
        public static readonly int HEARTBEAT_TIMEOUT = 3;

        public string Name { get; private set; }
        public RemoteState State { get; private set; }
        public long Timestamp { get; private set; }
        public string ProtocolTypeName { get; private set; }

        private Connection connection;
        private IProtocolCoder protocolCoder;
        private IMessagePacker messagePacker;
        private IProtocolDispatcher protocolDispatcher;
        private IRemoteHandler remoteHandler;
        private IHeartbeat heartbeat;

        private string host;
        private int port;
        private ConnectMode connectMode;

        private int curRetryCount;
        private float prevRetryTime;
        private bool retryReconnect;

        private HeartState heartState;
        private float sendHeartTime;

        public Remote(string name)
        {
            connectMode = ConnectMode.None;
            this.Name = name;
            this.heartState = HeartState.Stop;
            MonoEventProxy.Instance.UpdateEvent += Update;
        }

        /// <summary>
        /// 初始远端链接器
        /// </summary>
        /// <param name="connection">链接器</param>
        /// <param name="remoteHandler">远端处理器</param>
        /// <param name="protocolCoder">协议解码编码器</param>
        /// <param name="protocolDispatcher">协议派发器</param>
        /// <param name="messagePacker">消息解包拆包器</param>
        public void Build(Connection connection, IRemoteHandler remoteHandler, IProtocolCoder protocolCoder,
            IProtocolDispatcher protocolDispatcher, IMessagePacker messagePacker, IHeartbeat heartbeat)
        {
            this.connection = connection;
            this.remoteHandler = remoteHandler;
            this.protocolCoder = protocolCoder;
            this.protocolDispatcher = protocolDispatcher;
            this.messagePacker = messagePacker;
            this.heartbeat = heartbeat;

            connection.OnConnectEvent = OnConnect;
            connection.OnReceivedEvent = OnReceive;
            connection.OnErrorEvent = OnError;
            connection.OnDisconnectEvent = OnDisconnect;
        }

        public void Build(Connection connection, IRemoteHandler remoteHandler, IProtocolCoder protocolCoder,
            IProtocolDispatcher protocolDispatcher, IMessagePacker messagePacker, float heartbeatInterval)
        {
            Build(connection, remoteHandler, protocolCoder, protocolDispatcher, messagePacker,
                new Heartbeat(heartbeatInterval, protocolCoder));
        }

        public void Build(Connection connection, IRemoteHandler remoteHandler, IProtocolCoder protocolCoder,
            IProtocolDispatcher protocolDispatcher, IMessagePacker messagePacker)
        {
            Build(connection, remoteHandler, protocolCoder, protocolDispatcher, messagePacker,
                new Heartbeat(30, protocolCoder));
        }

        public void SetProtocolTypeName(string protocolTypeName)
        {
            this.ProtocolTypeName = protocolTypeName;
        }

        private bool CheckAddress()
        {
            if (string.IsNullOrEmpty(host) || port == 0)
            {
                Logger.WarnFormat("Connect address error:{0},{1}", host, port);
                return false;
            }

            return true;
        }

        private bool CheckUrl()
        {
            if (string.IsNullOrEmpty(host))
            {
                Logger.WarnFormat("Connect address error:{0}", host);
                return false;
            }

            return true;
        }

        public void Connect(string host, int port)
        {
            if (State == RemoteState.Connecting || State == RemoteState.Connected)
            {
                return;
            }

            this.host = host;
            this.port = port;
            if (CheckAddress())
            {
                connectMode = ConnectMode.Host;
                State = RemoteState.Connecting;
                protocolCoder.Clear();
                connection.Connect(host, port);
            }
        }

        public void Connect(string url)
        {
            if (State == RemoteState.Connecting || State == RemoteState.Connected)
            {
                return;
            }

            this.host = url;
            if (CheckUrl())
            {
                connectMode = ConnectMode.Url;
                State = RemoteState.Connecting;
                protocolCoder.Clear();
                connection.Connect(host);
            }
        }


        private void StartReconnect()
        {
            State = RemoteState.Reconnecting;
            curRetryCount++;
            prevRetryTime = TimeUtils.GetUnityNow();
            retryReconnect = false;
            protocolCoder.Clear();
            remoteHandler.OnReconnect(curRetryCount, MAX_RETRY_COUNT);
            if (connectMode == ConnectMode.Host)
            {
                connection.Connect(host, port);
            }
            else if (connectMode == ConnectMode.Url)
            {
                connection.Connect(host);
            }
        }

        public void Reconnect()
        {
            if (State == RemoteState.Reconnecting || State == RemoteState.Connected)
            {
                return;
            }

            if (connectMode != ConnectMode.None)
            {
                curRetryCount = 0;
                prevRetryTime = 0;
                retryReconnect = true;
            }
        }

        public void Disconnect()
        {
            retryReconnect = false;
            State = RemoteState.Close;
            StopHeartbeat();
            connection.Disconnect();
            protocolCoder.Clear();
            protocolDispatcher.Clear();
        }

        public bool SendBuffs(byte[] buffs)
        {
            if (null != buffs)
            {
                bool res = connection.Send(buffs);
                if (res)
                {
                    // sendHeartTime = TimeUtils.GetUnityNow();
                    return true;
                }
            }

            return false;
        }

        public bool SendProtocol(IProtocol protocol)
        {
            byte[] buffs = protocolCoder.Encode(protocol);
            return SendBuffs(buffs);
        }

        public void SendMessageBuffs(int opcode, byte[] dataBuffs)
        {
            IProtocol protocol = protocolCoder.CreateProtocol(opcode, dataBuffs);
            bool res = SendProtocol(protocol);
            if (!res)
            {
                Logger.WarnFormat("{0} Send Fail,Opcode:{1}", Name, opcode);
            }
        }

        public void SendMessage(int opcode, object data)
        {
            byte[] dataBuffs = messagePacker.Serialize(data);
            SendMessageBuffs(opcode, dataBuffs);
        }

        internal string DataToString(object data)
        {
            return messagePacker.SerializeToString(data);
        }

        private void SetStateByConnectError(int error)
        {
            if (error == 0)
            {
                State = RemoteState.Connected;
                StartHeartbeat();
            }
            else
            {
                State = RemoteState.None;
            }
        }

        private void OnConnect(int error)
        {
            MainThreadCaller.Call(MainThreadConnect, error);
        }

        private void MainThreadConnect(int error)
        {
            if (State == RemoteState.Connecting)
            {
                SetStateByConnectError(error);
                remoteHandler.OnConnect(error);
            }
            else if (State == RemoteState.Reconnecting)
            {
                if (error == 0 || curRetryCount >= MAX_RETRY_COUNT)
                {
                    SetStateByConnectError(error);
                    remoteHandler.OnConnect(error);
                }
                else
                {
                    retryReconnect = true;
                }
            }
        }

        private void OnError(String errorMsg)
        {
            MainThreadCaller.Call(MainThreadError, errorMsg);
        }

        private void MainThreadError(String errorMsg)
        {
            remoteHandler.OnError(errorMsg);
        }

        private void OnReceive(byte[] buffs, int offset, int length)
        {
            protocolCoder.Decode(buffs, offset, length);
            if (heartState == HeartState.WaitReply)
            {
                heartState = HeartState.Started;
            }
        }

        private void OnDisconnect()
        {
            MainThreadCaller.Call(MainThreadDisconnect);
        }

        private void MainThreadDisconnect()
        {
            if (State != RemoteState.Close)
            {
                State = RemoteState.Disconnect;
                StopHeartbeat();
                remoteHandler.OnDisconnect();
            }
        }

        private void Update(float deltaTime)
        {
            connection.Update(deltaTime);
            protocolDispatcher.Update(deltaTime);
            UpdateHeartbeat();
            if (retryReconnect && TimeUtils.GetUnityElapse(prevRetryTime) > RETRY_INTERVAL)
            {
                StartReconnect();
            }
        }

        internal void OnReceiveHeartbeat(long timestamp)
        {
            this.Timestamp = timestamp;
            heartState = HeartState.Started;
        }

        private void StartHeartbeat()
        {
            if (null != heartbeat && heartState != HeartState.Started)
            {
                heartState = HeartState.Ready;
            }
        }

        private void StopHeartbeat()
        {
            heartState = HeartState.Stop;
        }

        private void UpdateHeartbeat()
        {
            if (heartState == HeartState.Ready)
            {
                heartState = HeartState.Started;
                sendHeartTime = TimeUtils.GetUnityNow();
            }
            else if (heartState == HeartState.Started)
            {
                if (TimeUtils.GetUnityElapse(sendHeartTime) >= heartbeat.Interval)
                {
                    heartState = HeartState.WaitReply;
                    sendHeartTime = TimeUtils.GetUnityNow();
                    heartbeat.Tick(this);
                }
            }
            else if (heartState == HeartState.WaitReply)
            {
                //还没回复心跳消息,认为断线了
                if (TimeUtils.GetUnityElapse(sendHeartTime) >= HEARTBEAT_TIMEOUT)
                {
                    Logger.WarnFormat("Heartbeat timeout,disconnect socket");
                    connection.Disconnect();
                    MainThreadDisconnect();
                }
            }
        }

        public void Dispose()
        {
            MonoEventProxy.Instance.UpdateEvent -= Update;
            connection.OnConnectEvent = null;
            connection.OnReceivedEvent = null;
            connection.OnErrorEvent = null;
            connection.OnDisconnectEvent = null;
            connection.Disconnect();
            remoteHandler.Dispose();
        }

        public override string ToString()
        {
            return string.Format("Remote[{0}]", Name);
        }
    }
}