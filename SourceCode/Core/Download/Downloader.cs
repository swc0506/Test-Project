using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace Core
{
    public enum DownloadState
    {
        Idle,
        Downloading,
        Success,
        Fail
    }

    public delegate void DownloadProgressAction(Downloader downloader);

    public delegate void DownloadCompletedAction(Downloader downloader);


    public abstract class Downloader
    {
        //超时时间 单位秒
        public int timeout = 1000 * 20;

        //缓冲区大小
        public int flushSize = 1024 * 512;

        protected bool useThread;
        public bool mainThreadCall;

        private Stopwatch stopWatch;
        internal long completedLength;
        internal long downloadedLength;

        /// <summary>
        ///下载进度百分比,速度b/s
        /// </summary>
        public DownloadProgressAction progressAction;

        /// <summary>
        /// 完成事件 是否成功,文件路径
        /// </summary>
        public DownloadCompletedAction completedAction;

        public string Url { get; private set; }

        /// <summary>
        /// 总大小
        /// </summary>
        public long TotalLength { get; protected set; }

        public long CompletedLength
        {
            get { return completedLength; }
        }

        /// <summary>
        /// 下载进度百分比
        /// </summary>
        public float Percent { get; private set; }

        /// <summary>
        /// 状态
        /// </summary>
        public DownloadState State { get; private set; }

        /// <summary>
        /// 下载速度 单位: b/s
        /// </summary>
        public float Speed { get; private set; }

        public bool IsCompleted
        {
            get { return State == DownloadState.Success || State == DownloadState.Fail; }
        }

        public bool IsSuccess
        {
            get { return State == DownloadState.Success; }
        }

        protected string savePath;
        protected string saveTempPath;
        protected string md5;
        protected HttpWebRequest webRequest;

        internal void SetTask(string url, string saveDir, long length, string md5)
        {
            Url = url;
            string fileName = Path.GetFileName(url);
            savePath = Path.Combine(saveDir, fileName);
            saveTempPath = savePath + ".temp";
            this.md5 = md5;
            TotalLength = length;
        }

        public Downloader()
        {
            stopWatch = new Stopwatch();
            completedLength = 0;
            downloadedLength = 0;
            TotalLength = 0;
            Percent = 0;
            State = DownloadState.Idle;
            Speed = 0;
        }

        protected HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.Timeout = timeout;
            return webRequest;
        }

        protected bool GetHttpResponseContentLength(string url, out long totalLength)
        {
            try
            {
                var webRequest = CreateWebRequest(url);
                totalLength = webRequest.GetResponse().ContentLength;
                webRequest.Abort();
                return true;
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("GetHttpWebResponseLength,Url:{0},Exception:{1}", url, e);
            }

            totalLength = 0;
            return false;
        }

        protected void SetCompletedLength(long value)
        {
            completedLength = value;
            float percent = 0;
            if (TotalLength > 0)
            {
                percent = (float)Math.Round((completedLength / (double)TotalLength) * 100, 1);
            }
            else
            {
                percent = (completedLength > 0 && completedLength == TotalLength) ? 100 : 0;
            }

            if (Percent > percent)
            {
                return;
            }

            Percent = percent;
            if (useThread && mainThreadCall)
            {
                MainThreadCaller.Call(CallProgressAction);
            }
            else
            {
                CallProgressAction();
            }
        }

        private void CallProgressAction()
        {
            progressAction?.Invoke(this);
        }

        protected void SetDownloadLength(long value)
        {
            downloadedLength = value;
            if (value == 0)
            {
                stopWatch.Restart();
            }

            float elapsedTime = stopWatch.ElapsedMilliseconds / 1000f;
            if (elapsedTime > 0)
            {
                Speed = downloadedLength / elapsedTime;
            }
            else
            {
                Speed = 0;
            }
        }

        protected void OnCompleted(bool success)
        {
            stopWatch.Stop();
            State = success ? DownloadState.Success : DownloadState.Fail;
            if (success && TotalLength > 0)
            {
                SetCompletedLength(TotalLength);
            }

            if (useThread && mainThreadCall)
            {
                MainThreadCaller.Call(CallCompletedAction);
            }
            else
            {
                CallCompletedAction();
            }
        }

        private void CallCompletedAction()
        {
            completedAction?.Invoke(this);
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="useThread">是否使用线程现在</param>
        public void Start(bool useThread)
        {
            if (State != DownloadState.Downloading)
            {
                this.useThread = useThread;
                if (useThread)
                {
                    ThreadPool.QueueUserWorkItem(OnThreadCheckDownload);
                }
                else
                {
                    TryDownload();
                }
            }
        }

        public void Start()
        {
            Start(true);
        }

        private void OnThreadCheckDownload(object obj)
        {
            TryDownload();
        }

        private void TryDownload()
        {
            State = DownloadState.Downloading;
            if (!TryGetTotalLength())
            {
                OnCompleted(false);
            }
            else
            {
                completedLength = 0;
                downloadedLength = 0;
                SetCompletedLength(0);
                StartHttpDownload();
            }
        }

        protected virtual bool TryGetTotalLength()
        {
            if (TotalLength < 0)
            {
                if (!GetHttpResponseContentLength(Url, out var totalLength))
                {
                    return false;
                }

                TotalLength = totalLength;
            }

            return true;
        }

        protected abstract void StartHttpDownload();


        /// <summary>
        /// 重试下载
        /// </summary>
        public void Retry()
        {
            if (State == DownloadState.Fail)
            {
                Start();
            }
        }

        public virtual void Dispose()
        {
            stopWatch.Stop();
            stopWatch = null;
            completedAction = null;
            progressAction = null;

            if (null != webRequest)
            {
                webRequest.Abort();
                webRequest = null;
            }

            Url = null;
            savePath = null;
            saveTempPath = null;
        }
    }
}