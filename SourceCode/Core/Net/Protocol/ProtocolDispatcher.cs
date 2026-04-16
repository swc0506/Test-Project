using System.Collections.Concurrent;

namespace Core.Net
{
    public class ProtocolDispatcher : IProtocolDispatcher
    {
        private readonly ConcurrentQueue<MessagePacket> msgQueue;
        private readonly ByteBuffer byteBuffer;

        private readonly IMessageHandler messageHandler;
        private readonly IMessagePacker messagePacker;

        private readonly int MSG_MAX_COUNT = 10;
        private int currMsgCount;

        public ProtocolDispatcher(IMessageHandler messageHandler, IMessagePacker messagePacker, bool littleEndian)
        {
            msgQueue = new ConcurrentQueue<MessagePacket>();
            byteBuffer = new ByteBuffer(128, littleEndian);

            this.messageHandler = messageHandler;
            this.messagePacker = messagePacker;
        }

        public ProtocolDispatcher(IMessageHandler messageHandler, IMessagePacker messagePacker) : this(messageHandler,
            messagePacker, false)
        {
        }

        public void Dispatch(IProtocol protocol)
        {
            MessagePacket messagePacket = new MessagePacket(protocol, messagePacker);
            msgQueue.Enqueue(messagePacket);
        }

        public void Update(float deltaTime)
        {
            while (msgQueue.TryDequeue(out MessagePacket packet))
            {
                if (packet.ProtoType == (byte)ProtoType.DATA)
                {
                    messageHandler.OnReceive(packet);
                    if (++currMsgCount >= MSG_MAX_COUNT)
                    {
                        break;
                    }
                }
                else if (packet.ProtoType == (byte)ProtoType.HEARTBEAT)
                {
                    byteBuffer.WriteBytes(packet.Buffs);
                    long timestamp = byteBuffer.ReadLong();
                    byteBuffer.Clear();
                    messageHandler.OnReceiveHeartbeat(timestamp);
                }
            }

            currMsgCount = 0;
        }

        public void Clear()
        {
            while (msgQueue.TryDequeue(out MessagePacket packet))
            {
            }
        }
    }
}