using System;

namespace Core
{
    public struct SecureInt : IEquatable<SecureInt>
    {
        private int key;
        private long encryVal;
        private int dummyVal;

        public SecureInt(int value, int key)
        {
            if (key == 0)
            {
                key = new Random().Next();
            }

            this.key = key;
            this.encryVal = value ^ key;
            this.dummyVal = value;
        }

        public SecureInt(int value) : this(value, 0)
        {
        }

        public void Initial(int value, int key)
        {
            this.key = key;
            this.encryVal = value ^ key;
            this.dummyVal = value;
        }

        public int Value
        {
            get { return (int)(encryVal ^ key); }
            set
            {
                dummyVal = value;
                encryVal = value ^ key;
            }
        }

        public static SecureInt operator +(SecureInt a, SecureInt b)
        {
            return new SecureInt(a.Value + b.Value);
        }

        public static SecureInt operator -(SecureInt a, SecureInt b)
        {
            return new SecureInt(a.Value - b.Value);
        }

        public static SecureInt operator *(SecureInt a, SecureInt b)
        {
            return new SecureInt(a.Value * b.Value);
        }

        public static SecureInt operator /(SecureInt a, SecureInt b)
        {
            return new SecureInt(a.Value / b.Value);
        }

        public static SecureInt operator %(SecureInt a, SecureInt b)
        {
            return new SecureInt(a.Value % b.Value);
        }

        public static bool operator ==(SecureInt a, SecureInt b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(SecureInt a, SecureInt b)
        {
            return !(a == b);
        }

        public static bool operator >(SecureInt a, SecureInt b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(SecureInt a, SecureInt b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(SecureInt a, SecureInt b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(SecureInt a, SecureInt b)
        {
            return a.Value <= b.Value;
        }


        public static SecureInt operator +(SecureInt a, int b)
        {
            return new SecureInt(a.Value + b);
        }

        public static SecureInt operator -(SecureInt a, int b)
        {
            return new SecureInt(a.Value - b);
        }

        public static SecureInt operator *(SecureInt a, int b)
        {
            return new SecureInt(a.Value * b);
        }

        public static SecureInt operator /(SecureInt a, int b)
        {
            return new SecureInt(a.Value / b);
        }

        public static SecureInt operator %(SecureInt a, int b)
        {
            return new SecureInt(a.Value % b);
        }

        public static bool operator ==(SecureInt a, int b)
        {
            return a.Value == b;
        }

        public static bool operator !=(SecureInt a, int b)
        {
            return !(a == b);
        }

        public static bool operator >(SecureInt a, int b)
        {
            return a.Value > b;
        }

        public static bool operator <(SecureInt a, int b)
        {
            return a.Value < b;
        }

        public static bool operator >=(SecureInt a, int b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(SecureInt a, int b)
        {
            return a.Value <= b;
        }

        public static implicit operator SecureInt(int v)
        {
            return new SecureInt(v);
        }

        public static implicit operator int(SecureInt v)
        {
            return v.Value;
        }


        public override int GetHashCode()
        {
            return this.encryVal.GetHashCode() ^ this.key.GetHashCode() << 2;
        }

        public override bool Equals(object other)
        {
            return other is SecureInt other1 && this.Equals(other1);
        }

        public bool Equals(SecureInt other)
        {
            return Value == other.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}