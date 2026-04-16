using System;

namespace Core.FS
{
    public enum FileType
    {
        Normal,
        AssetBundle,
    }

    public struct AssetFileInfo : IEquatable<AssetFileInfo>
    {
        private const char SEPARATOR = '|';

        public string path;
        public string md5;
        public long length;
        public FileType type;

        public AssetFileInfo(string path, string md5, long length, FileType type)
        {
            this.path = path;
            this.md5 = md5;
            this.length = length;
            this.type = type;
        }

        public AssetFileInfo(string path, string md5, long length) : this(path, md5, length, FileType.Normal)
        {
        }

        public AssetFileInfo(string path, string md5) : this(path, md5, 0)
        {
        }

        public AssetFileInfo(string path, long length) : this(path, null, length)
        {
        }


        public AssetFileInfo(string content)
        {
            path = null;
            md5 = null;
            length = 0;
            type = FileType.Normal;
            if (!string.IsNullOrEmpty(content))
            {
                string[] strInfo = content.Split(SEPARATOR);
                if (null != strInfo && strInfo.Length == 4)
                {
                    path = strInfo[0];
                    md5 = strInfo[1];
                    long.TryParse(strInfo[2], out length);
                    Enum.TryParse<FileType>(strInfo[3], out type);
                }
            }
        }

        public bool Equals(AssetFileInfo other)
        {
            return path == other.path && md5 == other.md5 && type == other.type;
        }

        public override bool Equals(object obj)
        {
            return obj is AssetFileInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (path != null ? path.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}", path, SEPARATOR, md5, SEPARATOR, length, SEPARATOR,
                (int)type);
        }
    }
}