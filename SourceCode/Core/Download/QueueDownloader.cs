using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 串行下载器
    /// </summary>
    public class QueueDownloader : MultiTaskDownloader
    {
        private Downloader downloader;

        public QueueDownloader(DownloaderType type)
        {
            downloader = CreateDownloader(type);
            downloader.progressAction = OnTaskProgress;
            downloader.completedAction = OnTaskCompleted;
        }

        protected override void StartHttpDownload()
        {
            if (tasks.Count > 0)
            {
                TaskInfo info = tasks[tasks.Count - 1];
                downloader.SetTask(info.url, info.saveDir, info.size, info.md5);
                downloader.Start(useThread);
            }
        }

        private void OnTaskProgress(Downloader downloader)
        {
            long totalDownload = finishedDownload + downloader.downloadedLength;
            long totalCompleted = finishedCompleted + downloader.completedLength;
            SetDownloadLength(totalDownload);
            SetCompletedLength(totalCompleted);
        }

        private void OnTaskCompleted(Downloader downloader)
        {
            if (downloader.IsSuccess)
            {
                finishedDownload += downloader.downloadedLength;
                finishedCompleted += downloader.completedLength;

                tasks.RemoveAt(tasks.Count - 1);
                if (tasks.Count == 0)
                {
                    OnCompleted(true);
                }
                else
                {
                    StartHttpDownload();
                }
            }
            else
            {
                OnCompleted(false);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            downloader.Dispose();
            downloader = null;
        }
    }
}