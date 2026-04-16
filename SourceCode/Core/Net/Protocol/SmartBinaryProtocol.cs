using System;

namespace Core.Net
{
    /**
     * 带有包头长度的消息协议
     * 包长(ushort)+消息编号(ushort)+内容(byte[])
     */
    public class SmartBinaryProtocol : IEquatable<SmartBinaryProtocol>, IProtocol
    {
        //包头占用4个字节
        public const int HEAD_LENGTH = 2;
        public const int OPCODE_LENGTH = 2;

        //协议号
        private readonly int opcode;

        //buff数据
        private readonly byte[] buffs;

        public SmartBinaryProtocol(int opcode, byte[] buffs)
        {
            this.opcode = opcode;
            this.buffs = buffs;
        }

        public SmartBinaryProtocol(int opcode)
        {
            this.opcode = opcode;
            this.buffs = null;
        }

        public byte ProtoValue
        {
            get { return (byte)(opcode == (int)ProtoType.HEARTBEAT ? ProtoType.HEARTBEAT : ProtoType.DATA); }
        }

        public int Opcode
        {
            get { return opcode; }
        }

        public byte[] Buffs
        {
            get { return buffs; }
        }

        /**
         * 获取包的长度
         */
        public int GetLength()
        {
            int bufLength = buffs == null ? 0 : buffs.Length;
            return HEAD_LENGTH + OPCODE_LENGTH + bufLength;
        }

        public bool Equals(SmartBinaryProtocol other)
        {
            return opcode == other.opcode && Equals(buffs, other.buffs);
        }

        public override bool Equals(object obj)
        {
            return obj is BinaryProtocol other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = opcode;
                hashCode = (hashCode * 397) ^ (buffs != null ? buffs.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}