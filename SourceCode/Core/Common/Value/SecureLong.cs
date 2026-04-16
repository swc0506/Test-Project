using System;

namespace Core
{
    public struct SecureLong : IEquatable<SecureLong>
    {
        private long key;
        private long encryVal;
        private long dummyVal;

        public SecureLong(long value, long key)
        {
            if (key == 0)
            {
                key = new Random().Next();
            }

            this.key = key;
            this.encryVal = value ^ key;
            this.dummyVal = value;
        }

        public SecureLong(long value) : this(value, 0)
        {
        }

        public void Initial(long value, long key)
        {
            this.key = key;
            this.encryVal = value ^ key;
            this.dummyVal = value;
        }

        public long Value
        {
            get { return encryVal ^ key; }
            set
            {
                dummyVal = value;
                encryVal = value ^ key;
            }
        }


        public static SecureLong operator +(SecureLong a, SecureLong b)
        {
            return new SecureLong(a.Value + b.Value);
        }

        public static SecureLong operator -(SecureLong a, SecureLong b)
        {
            return new SecureLong(a.Value - b.Value);
        }

        public static SecureLong operator *(SecureLong a, SecureLong b)
        {
            return new SecureLong(a.Value * b.Value);
        }

        public static SecureLong operator /(SecureLong a, SecureLong b)
        {
            return new SecureLong(a.Value / b.Value);
        }

        public static SecureLong operator %(SecureLong a, SecureLong b)
        {
            return new SecureLong(a.Value % b.Value);
        }

        public static bool operator ==(SecureLong a, SecureLong b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(SecureLong a, SecureLong b)
        {
            return !(a == b);
        }

        public static bool operator >(SecureLong a, SecureLong b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(SecureLong a, SecureLong b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(SecureLong a, SecureLong b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(SecureLong a, SecureLong b)
        {
            return a.Value <= b.Value;
        }


        public static SecureLong operator +(SecureLong a, long b)
        {
            return new SecureLong(a.Value + b);
        }

        public static SecureLong operator -(SecureLong a, long b)
        {
            return new SecureLong(a.Value - b);
        }

        public static SecureLong operator *(SecureLong a, long b)
        {
            return new SecureLong(a.Value * b);
        }

        public static SecureLong operator /(SecureLong a, long b)
        {
            return new SecureLong(a.Value / b);
        }

        public static SecureLong operator %(SecureLong a, long b)
        {
            return new SecureLong(a.Value % b);
        }

        public static bool operator ==(SecureLong a, long b)
        {
            return a.Value == b;
        }

        public static bool operator !=(SecureLong a, long b)
        {
            return !(a == b);
        }

        public static bool operator >(SecureLong a, long b)
        {
            return a.Value > b;
        }

        public static bool operator <(SecureLong a, long b)
        {
            return a.Value < b;
        }

        public static bool operator >=(SecureLong a, long b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(SecureLong a, long b)
        {
            return a.Value <= b;
        }

        public static implicit operator SecureLong(long v)
        {
            return new SecureLong(v);
        }

        public static implicit operator long(SecureLong v)
        {
            return v.Value;
        }


        public override int GetHashCode()
        {
            return this.encryVal.GetHashCode() ^ this.key.GetHashCode() << 2;
        }

        public override bool Equals(object other)
        {
            return other is SecureLong other1 && this.Equals(other1);
        }

        public bool Equals(SecureLong other)
        {
            return Value == other.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}