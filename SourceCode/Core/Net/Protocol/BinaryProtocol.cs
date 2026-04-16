using System;

namespace Core.Net
{
    /**
     * 带有包头长度的消息协议
     * 包长(int)+协议类型(byte)+消息编号(int)+内容(byte[])
     */
    public readonly struct BinaryProtocol : IEquatable<BinaryProtocol>, IProtocol
    {
        //包头占用4个字节
        public const int HEAD_LENGTH = 4;
        public const int PROTO_TYPE_LENGTH = 1;
        public const int OPCODE_LENGTH = 4;

        //数据包类型
        private readonly byte protoValue;

        //协议号
        private readonly int opcode;

        //buff数据
        private readonly byte[] buffs;

        public BinaryProtocol(byte protoValue, int opcode, byte[] buffs)
        {
            this.protoValue = protoValue;
            this.opcode = opcode;
            this.buffs = buffs;
        }

        public BinaryProtocol(byte protoValue, byte[] buffs) : this(protoValue, 0, buffs)
        {
        }

        public BinaryProtocol(byte protoValue) : this(protoValue, 0, null)
        {
        }

        public BinaryProtocol(int opcode, byte[] buffs) : this((byte) Net.ProtoType.DATA, opcode, buffs)
        {
        }

        /**
         * 获取数据包类型
         */
        public byte ProtoValue
        {
            get { return protoValue; }
        }

        /**
         * 获取协议号
         */
        public int Opcode
        {
            get { return opcode; }
        }

        /**
         * 获取数据buff
         */
        public byte[] Buffs
        {
            get { return buffs; }
        }

        /**
         * 获取包的长度
         */
        public int GetLength()
        {
            int opcodeLength = protoValue == (byte) Net.ProtoType.DATA ? OPCODE_LENGTH : 0;
            int bufLength = buffs == null ? 0 : buffs.Length;
            return PROTO_TYPE_LENGTH + opcodeLength + bufLength;
        }

        public bool Equals(BinaryProtocol other)
        {
            return protoValue == other.protoValue && opcode == other.opcode && Equals(buffs, other.buffs);
        }

        public override bool Equals(object obj)
        {
            return obj is BinaryProtocol other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = protoValue.GetHashCode();
                hashCode = (hashCode * 397) ^ opcode;
                hashCode = (hashCode * 397) ^ (buffs != null ? buffs.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}