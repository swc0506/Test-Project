#if VIRTUAL

namespace Core.Net
{
    public interface IChannelHandler : IUpdateable
    {
        IProtocolCoder ProtocolCoder { get; }

        IProtocolDispatcher ProtocolDispatcher { get; }

        IMessagePacker MessagePacker { get; }

        void OnChannelActive(Connection connection);
        
        void OnChannelInactive(Connection connection);
    }
}

#endif