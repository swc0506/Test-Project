#if VIRTUAL

namespace Core.Net
{
    public interface IServerHandler : IUpdateable
    {
        VirtualServer Server { get; }
        IProtocolCoder ProtocolCoder { get; }

        IProtocolDispatcher ProtocolDispatcher { get; }

        IMessagePacker MessagePacker { get; }

        Connection Connection { get; }

        void Initial(VirtualServer server);

        void OnChannelActive(Connection connection);

        void OnChannelInactive(Connection connection);
    }
}

#endif