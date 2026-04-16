using System;
using Google.FlatBuffers;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Core.Config
{
    public sealed class ReadOnlyNativeArrayAllocator : ByteBufferAllocator
    {
        private readonly unsafe void* pointer;

        public unsafe ReadOnlyNativeArrayAllocator(NativeArray<byte> nativeArray)
        {
            pointer = nativeArray.GetUnsafePtr();
            Length = nativeArray.Length;
        }

        public override void GrowFront(int newSize)
        {
            throw new NotSupportedException();
        }

        public override Span<byte> Span
        {
            get
            {
                unsafe
                {
                    return new Span<byte>(pointer, Length);
                }
            }
        }
        public override ReadOnlySpan<byte> ReadOnlySpan
        {
            get
            {
                unsafe
                {
                    return new ReadOnlySpan<byte>(pointer, Length);
                }
            }
        }

        public override Memory<byte> Memory => throw new NotSupportedException();
        public override ReadOnlyMemory<byte> ReadOnlyMemory => throw new NotSupportedException();
    }
}