using System;
using System.Globalization;

namespace Core
{
    public struct SecureFloat : IEquatable<SecureFloat>, IFormattable
    {
        private float key;
        private float encryVal;
        private float dummyVal;

        public SecureFloat(float value, float key)
        {
            if (key == 0)
            {
                key = (float)new Random().NextDouble();
            }

            this.key = key;
            this.encryVal = value + key;
            this.dummyVal = value;
        }

        public SecureFloat(float value) : this(value, 0)
        {
        }

        public void Initial(float value, float key)
        {
            this.key = key;
            this.encryVal = value + key;
            this.dummyVal = value;
        }

        public float Value
        {
            get { return encryVal - key; }
            set
            {
                dummyVal = value;
                encryVal = value + key;
            }
        }

        public static SecureFloat operator +(SecureFloat a, SecureFloat b)
        {
            return new SecureFloat(a.Value + b.Value);
        }

        public static SecureFloat operator -(SecureFloat a, SecureFloat b)
        {
            return new SecureFloat(a.Value - b.Value);
        }

        public static SecureFloat operator *(SecureFloat a, SecureFloat b)
        {
            return new SecureFloat(a.Value * b.Value);
        }

        public static SecureFloat operator /(SecureFloat a, SecureFloat b)
        {
            return new SecureFloat(a.Value / b.Value);
        }

        public static SecureFloat operator %(SecureFloat a, SecureFloat b)
        {
            return new SecureFloat(a.Value % b.Value);
        }

        public static bool operator ==(SecureFloat a, SecureFloat b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(SecureFloat a, SecureFloat b)
        {
            return !(a == b);
        }

        public static bool operator >(SecureFloat a, SecureFloat b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(SecureFloat a, SecureFloat b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(SecureFloat a, SecureFloat b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(SecureFloat a, SecureFloat b)
        {
            return a.Value <= b.Value;
        }


        public static SecureFloat operator +(SecureFloat a, float b)
        {
            return new SecureFloat(a.Value + b, a.key);
        }

        public static SecureFloat operator -(SecureFloat a, float b)
        {
            return new SecureFloat(a.Value - b, a.key);
        }

        public static SecureFloat operator *(SecureFloat a, float b)
        {
            return new SecureFloat(a.Value * b, a.key);
        }

        public static SecureFloat operator /(SecureFloat a, float b)
        {
            return new SecureFloat(a.Value / b, a.key);
        }

        public static SecureFloat operator %(SecureFloat a, float b)
        {
            return new SecureFloat(a.Value % b, a.key);
        }

        public static bool operator ==(SecureFloat a, float b)
        {
            return a.Value == b;
        }

        public static bool operator !=(SecureFloat a, float b)
        {
            return a.Value != b;
        }

        public static bool operator >(SecureFloat a, float b)
        {
            return a.Value > b;
        }

        public static bool operator <(SecureFloat a, float b)
        {
            return a.Value < b;
        }

        public static bool operator >=(SecureFloat a, float b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(SecureFloat a, float b)
        {
            return a.Value < b;
        }


        public static implicit operator SecureFloat(float v)
        {
            return new SecureFloat(v);
        }

        public static implicit operator float(SecureFloat v)
        {
            return v.Value;
        }


        public override int GetHashCode()
        {
            return this.encryVal.GetHashCode() ^ this.key.GetHashCode() << 2;
        }


        public override bool Equals(object other)
        {
            return other is SecureFloat other1 && this.Equals(other1);
        }

        public bool Equals(SecureFloat other)
        {
            return Value == other.Value;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F2";
            }

            if (formatProvider == null)
            {
                formatProvider = (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat;
            }

            return Value.ToString(format, formatProvider);
        }
    }
}