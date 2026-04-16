#if VIRTUAL

namespace Core.Net
{
    public class AbstractServerHandler : IServerHandler
    {
        private IProtocolCoder protocolCoder;
        private IProtocolDispatcher protocolDispatcher;
        private IMessagePacker messagePacker;
        private Connection connection;
        private VirtualServer server;

        public IProtocolCoder ProtocolCoder
        {
            get { return protocolCoder; }
        }

        public IProtocolDispatcher ProtocolDispatcher
        {
            get { return protocolDispatcher; }
        }

        public IMessagePacker MessagePacker
        {
            get { return messagePacker; }
        }

        public Connection Connection
        {
            get { return connection; }
        }

        public VirtualServer Server
        {
            get { return server; }
        }


        public void Build(IProtocolCoder protocolCoder, IProtocolDispatcher protocolDispatcher,
            IMessagePacker messagePacker)
        {
            this.protocolCoder = protocolCoder;
            this.protocolDispatcher = protocolDispatcher;
            this.messagePacker = messagePacker;
        }

        public void Initial(VirtualServer server)
        {
            this.server = server;
        }

        public virtual void OnChannelActive(Connection connection)
        {
            this.connection = connection;
        }

        public virtual void OnChannelInactive(Connection connection)
        {
            this.connection = null;
        }

        protected virtual void SendBuff(byte[] buffs, int offset, int length)
        {
            if (connection is VirtualConnection virtualConnection)
            {
                virtualConnection.OnReceiveCompleted(buffs, 0, buffs.Length);
            }
        }

        public void SendMessage(int opcode, object data)
        {
            byte[] dataBuffs = messagePacker.Serialize(data);
            IProtocol protocol = protocolCoder.CreateProtocol(opcode, dataBuffs);
            byte[] buffs = protocolCoder.Encode(protocol);
            SendBuff(buffs, 0, buffs.Length);
        }

        public void Update(float deltaTime)
        {
            protocolDispatcher.Update(deltaTime);
        }
    }
}

#endif