using System;

namespace Core.Net
{
    public struct MessagePacket : IEquatable<MessagePacket>
    {
        private IProtocol protocol;
        private IMessagePacker messagePacker;

        /**
         * 获取数据包类型
         */
        public byte ProtoType
        {
            get { return protocol.ProtoValue; }
        }

        /**
         * 获取协议号
         */
        public int Opcode
        {
            get { return protocol.Opcode; }
        }

        /**
         * 获取数据buff
         */
        public byte[] Buffs
        {
            get { return protocol.Buffs; }
        }

        public MessagePacket(IProtocol protocol, IMessagePacker messagePacker)
        {
            this.protocol = protocol;
            this.messagePacker = messagePacker;
        }

        public object Parse(Type type)
        {
            return messagePacker.Deserialize(protocol.Buffs, type);
        }

        public T Parse<T>()
        {
            return messagePacker.Deserialize<T>(protocol.Buffs);
        }

        public bool Equals(MessagePacket other)
        {
            return protocol.Equals(other.protocol);
        }

        public override bool Equals(object obj)
        {
            return obj is MessagePacket other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return protocol.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("Opcode:{0}", protocol.Opcode);
        }
    }
}