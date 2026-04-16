using System;
using System.Collections;

namespace Core.FS
{
    [Serializable]
    public struct VersionNum : IComparable<VersionNum>, IEquatable<VersionNum>
    {
        public static readonly VersionNum zero = new VersionNum(0, 0, 0);

        public int major;
        public int minor;
        public int build;

        public VersionNum(int major, int minor, int build)
        {
            this.major = major;
            this.minor = minor;
            this.build = build;
        }

        public VersionNum(string version)
        {
            major = 0;
            minor = 0;
            build = 0;
            string[] strs = version.Split('.');
            int length = strs.Length;
            if (length > 0)
            {
                int.TryParse(strs[0], out major);
            }

            if (length > 1)
            {
                int.TryParse(strs[1], out minor);
            }

            if (length > 2)
            {
                int.TryParse(strs[2], out build);
            }
        }

        private void TryParse(string[] strs, int index, out int value)
        {
            if (strs.Length > index)
            {
                int.TryParse(strs[index], out value);
            }
            else
            {
                value = 0;
            }
        }

        public int CompareTo(VersionNum value)
        {
            if (major != value.major)
            {
                return major > value.major ? 1 : -1;
            }

            if (minor != value.minor)
            {
                return minor > value.minor ? 1 : -1;
            }

            if (build != value.build)
            {
                return build > value.build ? 1 : -1;
            }

            return 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = major.GetHashCode();
                hashCode = (hashCode * 397) ^ minor.GetHashCode();
                hashCode = (hashCode * 397) ^ build.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is VersionNum versionNum)
            {
                return Equals(versionNum);
            }

            return false;
        }

        public bool Equals(VersionNum other)
        {
            return major == other.major && minor == other.minor && build == other.build;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", major, minor, build);
        }

        public static bool operator ==(VersionNum v1, VersionNum v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(VersionNum v1, VersionNum v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(VersionNum v1, VersionNum v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(VersionNum v1, VersionNum v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >(VersionNum v1, VersionNum v2)
        {
            return v2 < v1;
        }

        public static bool operator >=(VersionNum v1, VersionNum v2)
        {
            return v2 <= v1;
        }
    }
}