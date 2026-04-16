namespace Core.Net
{
    public interface IProtocolCoder : IClearable
    {
        /// <summary>
        /// 协议编码
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        byte[] Encode(IProtocol protocol);

        /// <summary>
        /// 协议解码
        /// </summary>
        /// <param name="buffs">内容字节</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        void Decode(byte[] buffs, int offset, int length);

        /// <summary>
        /// 创建protocol
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="buffs"></param>
        /// <returns></returns>
        IProtocol CreateProtocol(int opcode, byte[] buffs);

        /// <summary>
        /// 创建心跳 protocol
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        IProtocol CreateProtocol(long timestamp);
    }
}