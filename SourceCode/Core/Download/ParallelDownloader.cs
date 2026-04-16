using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 并行下载器
    /// </summary>
    public class ParallelDownloader : MultiTaskDownloader
    {
        private List<Downloader> downloaders = new List<Downloader>();
        private List<TaskInfo> doingTasks = new List<TaskInfo>();
        private bool hasError;

        public ParallelDownloader(DownloaderType type, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Downloader downloader = CreateDownloader(type);
                downloader.progressAction = OnTaskProgress;
                downloader.completedAction = OnTaskCompleted;

                downloaders.Add(downloader);
            }
        }
        public ParallelDownloader(DownloaderType type):this(type,20)
        {
        }

        private void PushDownloadTask(Downloader downloader)
        {
            if (tasks.Count > 0)
            {
                int lastIndex = tasks.Count - 1;
                TaskInfo info = tasks[lastIndex];
                tasks.RemoveAt(lastIndex);
                doingTasks.Add(info);

                downloader.SetTask(info.url, info.saveDir, info.size, info.md5);
                downloader.Start(useThread);
            }
        }

        protected override void StartHttpDownload()
        {
            hasError = false;
            foreach (var item in downloaders)
            {
                if (item.State != DownloadState.Downloading)
                {
                    PushDownloadTask(item);
                }
            }
        }

        private void OnTaskProgress(Downloader downloader)
        {
            long totalDownload = finishedDownload + downloader.downloadedLength;
            long totalCompleted = finishedCompleted + downloader.completedLength;
            SetDownloadLength(totalDownload);
            SetCompletedLength(totalCompleted);
        }

        private int FindDoingTask(string url)
        {
            for (int i = 0; i < doingTasks.Count; i++)
            {
                if (doingTasks[i].url == url)
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnTaskCompleted(Downloader downloader)
        {
            int index = FindDoingTask(downloader.Url);
            if (downloader.IsSuccess)
            {
                finishedDownload += downloader.downloadedLength;
                finishedCompleted += downloader.completedLength;
                if (index >= 0)
                {
                    doingTasks.RemoveAt(index);
                }

                if (tasks.Count == 0 && doingTasks.Count == 0)
                {
                    OnCompleted(true);
                }
                else
                {
                    if (!hasError)
                    {
                        PushDownloadTask(downloader);
                    }
                }
            }
            else
            {
                if (index >= 0)
                {
                    TaskInfo info = doingTasks[index];
                    doingTasks.RemoveAt(index);
                    AddTask(info.url, info.saveDir, info.size);
                }

                hasError = true;
            }

            if (hasError)
            {
                int idleCount = 0;
                foreach (var item in downloaders)
                {
                    if (item.State != DownloadState.Downloading)
                    {
                        idleCount++;
                    }
                }

                if (idleCount == downloaders.Count)
                {
                    OnCompleted(false);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var item in downloaders)
            {
                item.Dispose();
            }

            downloaders = null;
            doingTasks = null;
        }
    }
}