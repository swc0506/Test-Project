using UnityEngine;

namespace Core.Net
{
    /// <summary>
    /// 带有包头长度的消息协议
    /// </summary>
    public class SmartBinaryProtocolCoder : IProtocolCoder
    {
        private readonly ByteBuffer encodeBuffer;
        private readonly ByteBuffer decodeBuffer;
        private readonly IProtocolDispatcher protocolDispatcher;
        private readonly bool littleEndian;

        private SmartBinaryProtocolCoder subProtocolCoder;

        public SmartBinaryProtocolCoder(IProtocolDispatcher protocolDispatcher, bool littleEndian, bool onlyDecode)
        {
            if (!onlyDecode)
            {
                encodeBuffer = new ByteBuffer(littleEndian);
            }

            decodeBuffer = new ByteBuffer(littleEndian);

            this.protocolDispatcher = protocolDispatcher;
            this.littleEndian = littleEndian;
        }

        public SmartBinaryProtocolCoder(IProtocolDispatcher protocolDispatcher, bool littleEndian) : this(
            protocolDispatcher, littleEndian, false)
        {
        }

        public SmartBinaryProtocolCoder(IProtocolDispatcher protocolDispatcher) : this(protocolDispatcher, false)
        {
        }

        public byte[] Encode(IProtocol protocol)
        {
            SmartBinaryProtocol proto = (SmartBinaryProtocol)protocol;
            int length = proto.GetLength();
            if (length > Connection.MAX_BUFF_SIZE)
            {
                Logger.WarnFormat("Encode buff is out of range {0}", length);
                return null;
            }

            encodeBuffer.WriteUshort((ushort)length);
            encodeBuffer.WriteUshort((ushort)proto.Opcode);
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
            return new SmartBinaryProtocol(opcode, buffs);
        }

        public IProtocol CreateProtocol(long timestamp)
        {
            encodeBuffer.WriteLong(timestamp);
            byte[] buffs = encodeBuffer.ToArray();
            encodeBuffer.Clear();
            return CreateProtocol((int)ProtoType.HEARTBEAT, buffs);
        }

        private void DecodeProtocol()
        {
            int byteSize = decodeBuffer.ReadableBytes();
            if (byteSize < SmartBinaryProtocol.HEAD_LENGTH)
            {
                return;
            }

            //标记开始读取的index,防止后面还原
            decodeBuffer.MarkReaderIndex();
            int prevIndex = decodeBuffer.ReaderIndex();
            //包头标记的长度
            int length = decodeBuffer.ReadUshort();
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
            if (decodeBuffer.ReadableBytes() < length - SmartBinaryProtocol.HEAD_LENGTH)
            {
                decodeBuffer.ResetReaderIndex();
                return;
            }

            IProtocol protocol;
            int opcode = decodeBuffer.ReadUshort();
            //content的实际长度
            int bufLength = length - SmartBinaryProtocol.HEAD_LENGTH - SmartBinaryProtocol.OPCODE_LENGTH;
            byte[] buffs = ReadBytes(bufLength);

            //分包消息
            if (opcode == 2 || opcode == 3)
            {
                if (null == subProtocolCoder)
                {
                    subProtocolCoder = new SmartBinaryProtocolCoder(protocolDispatcher, littleEndian, true);
                }

                subProtocolCoder.Decode(buffs, 0, bufLength);
            }
            else
            {
                protocol = new SmartBinaryProtocol(opcode, buffs);
                //派发协议
                protocolDispatcher.Dispatch(protocol);
            }

            if (decodeBuffer.ReadableBytes() > 0)
            {
                DecodeProtocol();
            }
        }

        private byte[] ReadBytes(int bufLength)
        {
            if (bufLength >= 0)
            {
                byte[] buffs = new byte[bufLength];
                decodeBuffer.ReadBytes(buffs, 0, buffs.Length);
                return buffs;
            }

            return null;
        }

        public void Clear()
        {
            encodeBuffer?.Clear();
            decodeBuffer?.Clear();
            subProtocolCoder?.Clear();
        }
    }
}