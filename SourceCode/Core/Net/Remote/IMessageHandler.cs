namespace Core.Net
{
    public interface IMessageHandler
    {
        void OnReceive(MessagePacket packet);

        void OnReceiveHeartbeat(long timestamp);
    }
}