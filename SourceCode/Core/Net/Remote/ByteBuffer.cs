namespace Core.Net
{
    using System;

    public class ByteBuffer
    {
        //字节缓存区
        private byte[] buf;

        //读取索引
        private int readIndex;

        //写入索引
        private int writeIndex;

        //读取索引标记
        private int markReadIndex;

        //写入索引标记
        private int markWriteIndex;

        //缓存区字节数组的长度
        private int capacity;

        //是否小端
        private readonly bool littleEndian;

        public ByteBuffer(int capacity, bool littleEndian)
        {
            buf = new byte[capacity];
            this.capacity = capacity;
            this.littleEndian = littleEndian;
        }
        
        public ByteBuffer(byte[] bytes, bool littleEndian)
        {
            buf = bytes;
            this.capacity = bytes.Length;
            this.littleEndian = littleEndian;
        }
        
        public ByteBuffer(bool littleEndian) : this(2048, littleEndian)
        {
        }

        public ByteBuffer(byte[] bytes) : this(bytes, false)
        {
        }
        
        public ByteBuffer(int capacity) : this(capacity, false)
        {
        }
        
        public ByteBuffer() : this(2048, false)
        {
        }
        
        /**
    * 翻转字节数组，如果本地字节序列为低字节序列，则进行翻转以转换为高字节序列
    */
        private byte[] Flip(byte[] bytes)
        {
            bool needFlip = littleEndian != BitConverter.IsLittleEndian;
            if (needFlip)
            {
                //Array.Reverse(bytes);  有GC
                int length = bytes.Length;
                int size = length / 2;
                for (int i = 0; i < size; i++)
                {
                    byte temp = bytes[i];
                    int end = length - 1 - i;
                    bytes[i] = bytes[end];
                    bytes[end] = temp;
                }
            }

            return bytes;
        }


        /**
       * 确定内部字节缓存数组的大小
       */
        private int FixSizeAndReset(int currLen, int futureLen)
        {
            if (futureLen > currLen)
            {
                //以原大小的2次方数的两倍确定内部字节缓存区大小
                int size = FixLength(currLen) * 2;
                if (futureLen > size)
                {
                    //以将来的大小的2次方的两倍确定内部字节缓存区大小
                    size = FixLength(futureLen) * 2;
                }

                byte[] newbuf = new byte[size];
                Array.Copy(buf, 0, newbuf, 0, currLen);
                buf = newbuf;
                capacity = newbuf.Length;
            }

            return futureLen;
        }

        /**
     * 根据length长度，确定大于此leng的最近的2次方数，如length=7，则返回值为8
     */
        private int FixLength(int length)
        {
            int n = 2;
            int b = 2;
            while (b < length)
            {
                b = 2 << n;
                n++;
            }

            return b;
        }


        /**
     * 将bytes字节数组从startIndex开始的length字节写入到此缓存区
     */
        public ByteBuffer WriteBytes(byte[] bytes, int startIndex, int length)
        {
            lock (this)
            {
                int offset = length - startIndex;
                if (offset <= 0) return this;
                int total = offset + writeIndex;
                int len = buf.Length;
                FixSizeAndReset(len, total);
                for (int i = writeIndex, j = startIndex; i < total; i++, j++)
                {
                    this.buf[i] = bytes[j];
                }

                writeIndex = total;
            }

            return this;
        }

        /**
     * 将字节数组中从0到length的元素写入缓存区
     */
        public ByteBuffer WriteBytes(byte[] bytes, int length)
        {
            return WriteBytes(bytes, 0, length);
        }

        /**
     * 将字节数组全部写入缓存区
     */
        public ByteBuffer WriteBytes(byte[] bytes)
        {
            return WriteBytes(bytes, bytes.Length);
        }

        /**
     * 将一个ByteBuffer的有效字节区写入此缓存区中
     */
        public ByteBuffer Write(ByteBuffer buffer)
        {
            if (buffer == null) return this;
            if (buffer.ReadableBytes() <= 0) return this;
            return WriteBytes(buffer.ToArray());
        }

        /**
     * 写入一个int16数据
     */
        public ByteBuffer WriteShort(short value)
        {
            return WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
     * 写入一个uint16数据
     */
        public ByteBuffer WriteUshort(ushort value)
        {
            return WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**写入字符串*/
        public ByteBuffer WriteString(string value)
        {
            int len = value.Length;
            WriteInt(len);
            WriteBytes(System.Text.Encoding.UTF8.GetBytes(value));
            return this;
        }

        /**读取字符串*/
        public String ReadString()
        {
            int len = ReadInt();
            byte[] bytes = new byte[len];
            ReadBytes(bytes, 0, len);

            return System.Text.Encoding.UTF8.GetString(bytes);
        }


        /**
     * 写入一个int32数据
     */
        public ByteBuffer WriteInt(int value)
        {
            return WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
     * 写入一个uint32数据
     */
        public ByteBuffer WriteUint(uint value)
        {
            return WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
     * 写入一个int64数据
     */
        public ByteBuffer WriteLong(long value)
        {
            return WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
     * 写入一个uint64数据
     */
        public ByteBuffer WriteUlong(ulong value)
        {
            return WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
     * 写入一个float数据
     */
        public ByteBuffer WriteFloat(float value)
        {
            return WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
     * 写入一个byte数据
     */
        public ByteBuffer WriteByte(byte value)
        {
            lock (this)
            {
                int afterLen = writeIndex + 1;
                int len = buf.Length;
                FixSizeAndReset(len, afterLen);
                buf[writeIndex] = value;
                writeIndex = afterLen;
            }

            return this;
        }

        /**
     * 写入一个double类型数据
     */
        public ByteBuffer WriteDouble(double value)
        {
            return WriteBytes(Flip(BitConverter.GetBytes(value)));
        }


        /**
     * 读取一个字节
     */
        public byte ReadByte()
        {
            byte b = buf[readIndex];
            readIndex++;
            return b;
        }

        /**
     * 从读取索引位置开始读取len长度的字节数组
     */
        private byte[] Read(int len)
        {
            byte[] bytes = new byte[len];
            Array.Copy(buf, readIndex, bytes, 0, len);
            bytes = Flip(bytes);
            readIndex += len;
            return bytes;
        }

        /**
     * 读取一个uint16数据
     */
        public ushort ReadUshort()
        {
            return BitConverter.ToUInt16(Read(2), 0);
        }

        /**
     * 读取一个int16数据
     */
        public short ReadShort()
        {
            return BitConverter.ToInt16(Read(2), 0);
        }

        /**
     * 读取一个uint32数据
     */
        public uint ReadUint()
        {
            return BitConverter.ToUInt32(Read(4), 0);
        }

        /**
     * 读取一个int32数据
     */
        public int ReadInt()
        {
            return BitConverter.ToInt32(Read(4), 0);
        }

        /**
     * 读取一个uint64数据
     */
        public ulong ReadUlong()
        {
            return BitConverter.ToUInt64(Read(8), 0);
        }

        /**
     * 读取一个long数据
     */
        public long ReadLong()
        {
            return BitConverter.ToInt64(Read(8), 0);
        }

        /**
     * 读取一个float数据
     */
        public float ReadFloat()
        {
            return BitConverter.ToSingle(Read(4), 0);
        }

        /**
     * 读取一个double数据
     */
        public double ReadDouble()
        {
            return BitConverter.ToDouble(Read(8), 0);
        }

        /**
     * 从读取索引位置开始读取len长度的字节到disbytes目标字节数组中
     * @params disstart 目标字节数组的写入索引
     */
        public void ReadBytes(byte[] disbytes, int disstart, int len)
        {
            int size = disstart + len;
            for (int i = disstart; i < size; i++)
            {
                disbytes[i] = this.ReadByte();
            }
        }

        /**
     * 清除已读字节并重建缓存区
     */
        public void DiscardReadBytes()
        {
            if (readIndex <= 0) return;
            int len = buf.Length - readIndex;
            byte[] newbuf = new byte[len];
            Array.Copy(buf, readIndex, newbuf, 0, len);
            buf = newbuf;
            writeIndex -= readIndex;
            markReadIndex -= readIndex;
            if (markReadIndex < 0)
            {
                markReadIndex = readIndex;
            }

            markWriteIndex -= readIndex;
            if (markWriteIndex < 0 || markWriteIndex < readIndex || markWriteIndex < markReadIndex)
            {
                markWriteIndex = writeIndex;
            }

            readIndex = 0;
        }

        /**
     * 清空此对象
     */
        public void Clear()
        {
            Array.Clear(buf, 0, buf.Length);
            readIndex = 0;
            writeIndex = 0;
            markReadIndex = 0;
            markWriteIndex = 0;
        }

        /**
     * 设置开始读取的索引
     */
        public void SetReaderIndex(int index)
        {
            if (index < 0) return;
            readIndex = index;
        }

        public int ReaderIndex()
        {
            return readIndex;
        }

        /**
     * 标记读取的索引位置
     */
        public int MarkReaderIndex()
        {
            markReadIndex = readIndex;
            return markReadIndex;
        }

        /**
     * 标记写入的索引位置
     */
        public void MarkWriterIndex()
        {
            markWriteIndex = writeIndex;
        }

        /**
     * 将读取的索引位置重置为标记的读取索引位置
     */
        public void ResetReaderIndex()
        {
            readIndex = markReadIndex;
        }

        /**
     * 将写入的索引位置重置为标记的写入索引位置
     */
        public void ResetWriterIndex()
        {
            writeIndex = markWriteIndex;
        }

        /**
     * 可读的有效字节数
     */
        public int ReadableBytes()
        {
            return writeIndex - readIndex;
        }

        /**
     * 获取可读的字节数组
     */
        public byte[] ToArray()
        {
            byte[] bytes = new byte[writeIndex];
            Array.Copy(buf, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /**
     * 获取缓存区大小
     */
        public int GetCapacity()
        {
            return this.capacity;
        }

        public byte[] GetBuffer()
        {
            return buf;
        }
    }
}