namespace Core.Net
{
    /// <summary>
    /// 带有包头长度的消息协议
    /// </summary>
    public class BinaryProtocolCoder : IProtocolCoder
    {
        private readonly ByteBuffer encodeBuffer;
        private readonly ByteBuffer decodeBuffer;
        private readonly IProtocolDispatcher protocolDispatcher;

        public BinaryProtocolCoder(IProtocolDispatcher protocolDispatcher, bool littleEndian)
        {
            encodeBuffer = new ByteBuffer(littleEndian);
            decodeBuffer = new ByteBuffer(littleEndian);
            this.protocolDispatcher = protocolDispatcher;
        }

        public BinaryProtocolCoder(IProtocolDispatcher protocolDispatcher) : this(protocolDispatcher, false)
        {
        }

        public byte[] Encode(IProtocol protocol)
        {
            BinaryProtocol proto = (BinaryProtocol)protocol;
            int length = proto.GetLength();
            if (length > Connection.MAX_BUFF_SIZE)
            {
                Logger.WarnFormat("Encode buff is out of range {0}", length);
                return null;
            }

            encodeBuffer.WriteInt(length);
            encodeBuffer.WriteByte(proto.ProtoValue);
            if (proto.ProtoValue == (byte)ProtoType.DATA)
            {
                encodeBuffer.WriteInt(proto.Opcode);
            }

            if (null != proto.Buffs)
            {
                encodeBuffer.WriteBytes(proto.Buffs);
            }

            byte[] buffs = encodeBuffer.ToArray();
            encodeBuffer.Clear();
            return buffs;
        }

        public void Decode(byte[] buffs, int offset, int length)
        {
            decodeBuffer.WriteBytes(buffs, offset, length);
            DecodeProtocol();
        }

        public IProtocol CreateProtocol(int opcode, byte[] buffs)
        {
            if (opcode == 0)
            {
                return new BinaryProtocol((byte)ProtoType.HEARTBEAT, buffs);
            }
            else
            {
                return new BinaryProtocol(opcode, buffs);
            }
        }

        public IProtocol CreateProtocol(long timestamp)
        {
            encodeBuffer.WriteLong(timestamp);
            byte[] buffs = encodeBuffer.ToArray();
            encodeBuffer.Clear();
            return CreateProtocol(0, buffs);
        }

        private void DecodeProtocol()
        {
            int byteSize = decodeBuffer.ReadableBytes();
            if (byteSize < BinaryProtocol.HEAD_LENGTH)
            {
                return;
            }

            //标记开始读取的index,防止后面还原
            decodeBuffer.MarkReaderIndex();
            int prevIndex = decodeBuffer.ReaderIndex();
            //包头标记的长度
            int length = decodeBuffer.ReadInt();
            if (prevIndex == decodeBuffer.ReaderIndex())
            {
                return;
            }

            //不正常的消息包
            if (length <= 0 && length > Connection.MAX_BUFF_SIZE)
            {
                decodeBuffer.Clear();
                return;
            }

            //包不完整,还原读取的index
            if (decodeBuffer.ReadableBytes() < length)
            {
                decodeBuffer.ResetReaderIndex();
                return;
            }

            byte protoType = decodeBuffer.ReadByte();
            BinaryProtocol protocol;
            if (protoType == (byte)ProtoType.DATA)
            {
                int opcode = decodeBuffer.ReadInt();
                //content的实际长度
                int bufLength = length - BinaryProtocol.PROTO_TYPE_LENGTH - BinaryProtocol.OPCODE_LENGTH;
                byte[] buffs = ReadBytes(bufLength);
                protocol = new BinaryProtocol(opcode, buffs);
            }
            else if (protoType == (byte)ProtoType.HEARTBEAT || protoType == (byte)ProtoType.HANDSHAKE)
            {
                int bufLength = length - BinaryProtocol.PROTO_TYPE_LENGTH;
                byte[] buffs = ReadBytes(bufLength);
                protocol = new BinaryProtocol(protoType, buffs);
            }
            else
            {
                Logger.WarnFormat("Protocol protoType error：{0}", protoType);
                return;
            }

            //派发协议
            protocolDispatcher.Dispatch(protocol);

            if (decodeBuffer.ReadableBytes() > 0)
            {
                DecodeProtocol();
            }
        }

        private byte[] ReadBytes(int bufLength)
        {
            if (bufLength > 0)
            {
                byte[] buffs = new byte[bufLength];
                decodeBuffer.ReadBytes(buffs, 0, buffs.Length);
                return buffs;
            }

            return null;
        }

        public void Clear()
        {
            encodeBuffer.Clear();
            decodeBuffer.Clear();
        }
    }
}