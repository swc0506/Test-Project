using System;
using UnityEngine;

namespace Core.FS
{
    public abstract class AssetUpdater : IDisposable
    {
        public enum ErrorCode
        {
            None,
            Local,
            Remote,
        }

        public delegate void CompareAction(ErrorCode errorCode);


        protected string remoteUrl;
        protected VersionNum removeVerNum;
        protected CompareAction compareAction;

        protected AssetFileManifest localManifest;

        private Downloader downloader;

        public AssetFileManifest LocalManifest
        {
            get { return localManifest; }
        }

        public VersionNum RemoveVerNum
        {
            get { return removeVerNum; }
        }

        public AssetUpdater(string remoteUrl, VersionNum removeVer, CompareAction compareAction)
        {
            Initial(remoteUrl, removeVer, compareAction);
        }

        public AssetUpdater(string remoteUrl, string removeVer, CompareAction compareAction)
        {
            Initial(remoteUrl, new VersionNum(removeVer), compareAction);
        }

        private void Initial(string remoteUrl, VersionNum removeVerNum, CompareAction compareAction)
        {
            this.remoteUrl = remoteUrl;
            this.removeVerNum = removeVerNum;
            this.compareAction = compareAction;
        }

        public void Compare()
        {
            if (null == localManifest)
            {
                string[] paths = new string[2];
                paths[0] = string.Format("file://{0}/{1}", AssetPath.UpdateAssetsPath, AssetFileManifest.NAME);
                if (Application.isEditor || Application.platform == RuntimePlatform.Android)
                {
                    paths[1] = string.Format("{0}/{1}", AssetPath.InternalAssetsPath, AssetFileManifest.NAME);
                }
                else
                {
                    paths[1] = string.Format("file://{0}/{1}", AssetPath.InternalAssetsPath, AssetFileManifest.NAME);
                }

                ChainReader chainReader = new ChainReader(paths, MimeType.Text);
                chainReader.CompletedEvent += OnReadLocalManifest;
                chainReader.Read();
            }
            else
            {
                CompareVersion();
            }
        }

        private void OnReadLocalManifest(object data, string path, object userData)
        {
            if (null != data)
            {
                localManifest = new AssetFileManifest((string)data);
            }

            CompareVersion();
        }

        private void CompareVersion()
        {
            if (null == localManifest)
            {
                compareAction?.Invoke(ErrorCode.Local);
            }
            else
            {
                if (localManifest.VerNum >= removeVerNum)
                {
                    compareAction?.Invoke(ErrorCode.None);
                }
                else
                {
                    UpdateVersion();
                }
            }
        }

        protected abstract void UpdateVersion();

        public abstract long GetUpdateSize();

        public void Download(DownloadProgressAction progressAction, DownloadCompletedAction completedAction)
        {
            if (null == downloader)
            {
                downloader = CreateDowner();
                downloader.progressAction = progressAction;
                downloader.completedAction = completedAction;
                downloader.mainThreadCall = true;
                downloader.Start();
            }
        }

        protected abstract Downloader CreateDowner();

        public void RetryDownload()
        {
            downloader.Retry();
        }

        public abstract void ApplyDownloadFile(Action<float> progress, Action<bool> completed);

        public void ApplyDownloadFile()
        {
            ApplyDownloadFile(null, null);
        }

        public void Dispose()
        {
            remoteUrl = null;
            compareAction = null;
            localManifest = null;
            if (null != downloader)
            {
                downloader.Dispose();
                downloader = null;
            }
        }
    }
}