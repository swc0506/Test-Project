using System;
#if FLATBUFFERS
using Google.FlatBuffers;

#endif

namespace Core.I18N
{
#if FLATBUFFERS
    struct LanguagePackage : IFlatbufferObject, IEquatable<LanguagePackage>
#else
    struct LanguagePackage :IEquatable<LanguagePackage>
#endif
    {
#if FLATBUFFERS
        private Table p;

        public ByteBuffer ByteBuffer
        {
            get { return this.p.bb; }
        }

        public void __init(int _i, ByteBuffer _bb)
        {
            this.p = new Table(_i, _bb);
        }
#endif

        public void ForEach(Action<string, string> callback)
        {
#if FLATBUFFERS
            int length = p.__offset(4);
            length = length == 0 ? 0 : p.__vector_len(length);
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    //unit
                    int offset = p.__offset(4);
                    if (offset == 0)
                    {
                        continue;
                    }

                    Table up = new Table(p.__indirect(p.__vector(offset) + i * 4), p.bb);
                    int num = up.__offset(6);
                    string iKey = num == 0 ? null : up.__string(num + up.bb_pos);
                    num = up.__offset(8);
                    string iValue = num == 0 ? null : up.__string(num + up.bb_pos);

                    callback.Invoke(iKey, iValue);
                }
            }
#endif
        }

        public bool Equals(LanguagePackage other)
        {
            return p.Equals(other.p);
        }

        public override bool Equals(object obj)
        {
            return obj is LanguagePackage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return p.GetHashCode();
        }
    }
}