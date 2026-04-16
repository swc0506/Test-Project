using System;

namespace Core.I18N
{
    public struct LanguageInfo : IEquatable<LanguageInfo>
    {
        internal static LanguageInfo Empty = new LanguageInfo(string.Empty, string.Empty);

        public string Name { get; private set; }
        public string Display { get; private set; }

        public LanguageInfo(string name, string display)
        {
            this.Name = name;
            this.Display = display;
        }

        public bool Equals(LanguageInfo other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is LanguageInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}