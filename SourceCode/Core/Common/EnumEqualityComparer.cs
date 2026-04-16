using System;
using System.Collections.Generic;

namespace Core
{
    public class EnumEqualityComparer<T> : IEqualityComparer<T> where T : Enum
    {
        public bool Equals(T x, T y)
        {
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}