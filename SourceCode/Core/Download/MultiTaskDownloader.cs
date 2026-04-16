using System;
using System.Collections.Generic;

namespace Core
{
    public enum DownloaderType
    {
        Simple,
        Resume,
    }

    public abstract class MultiTaskDownloader : Downloader
    {
        protected struct TaskInfo : IEquatable<TaskInfo>
        {
            public string url;
            public string saveDir;
            public long size;
            public string md5;

            public TaskInfo(string url, string saveDir, long size, string md5)
            {
                this.url = url;
                this.saveDir = saveDir;
                this.size = size;
                this.md5 = md5;
            }

            public TaskInfo(string url, string saveDir, long size) : this(url, saveDir, size, null)
            {
            }

            public TaskInfo(string url, string saveDir, string md5) : this(url, saveDir, 0, md5)
            {
            }

            public TaskInfo(string url, string saveDir) : this(url, saveDir, 0, null)
            {
            }

            public bool Equals(TaskInfo other)
            {
                return url == other.url && saveDir == other.saveDir && size == other.size && md5 == other.md5;
            }

            public override bool Equals(object obj)
            {
                return obj is TaskInfo other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (url != null ? url.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (saveDir != null ? saveDir.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ size.GetHashCode();
                    hashCode = (hashCode * 397) ^ (md5 != null ? md5.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        private bool hasFileLength;
        protected List<TaskInfo> tasks = new List<TaskInfo>();
        protected long finishedDownload;
        protected long finishedCompleted;

        public void AddTask(string url, string saveDir, long size, string md5)
        {
            size = size < 0 ? 0 : size;
            if (!hasFileLength)
            {
                hasFileLength = size > 0;
            }

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(saveDir))
            {
                TaskInfo taskInfo = new TaskInfo(url, saveDir, size, md5);
                if (!tasks.Contains(taskInfo))
                {
                    tasks.Add(taskInfo);
                }
            }
        }

        public void AddTask(string url, string saveDir, long size)
        {
            AddTask(url, saveDir, size, null);
        }

        public void AddTask(string url, string saveDir)
        {
            AddTask(url, saveDir, 0);
        }

        protected Downloader CreateDownloader(DownloaderType type)
        {
            Downloader downloader = null;
            if (type == DownloaderType.Simple)
            {
                downloader = new SimpleDownloader();
            }
            else if (type == DownloaderType.Resume)
            {
                downloader = new ResumeDownloader();
            }

            return downloader;
        }

        protected override bool TryGetTotalLength()
        {
            finishedDownload = 0;
            finishedCompleted = 0;

            if (hasFileLength)
            {
                long totalLength = 0;
                foreach (var item in tasks)
                {
                    totalLength += item.size;
                }

                TotalLength = totalLength;
            }

            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
            tasks = null;
        }
    }
}