using System;
using System.IO;
using UnityEngine;

namespace Core.FS
{
    public class AssetFileUpdater : AssetUpdater
    {
        private AssetFileManifest remoteManifest;
        private string saveDir;

        public AssetFileUpdater(string remoteUrl, VersionNum removeVer, CompareAction compareAction) : base(
            remoteUrl, removeVer, compareAction)
        {
        }

        public AssetFileUpdater(string remoteUrl, string removeVer, CompareAction compareAction) : base(remoteUrl,
            removeVer, compareAction)
        {
        }

        protected override void UpdateVersion()
        {
            if (null == remoteManifest)
            {
                string remoteFilePath = string.Format("{0}/{1}/{2}", remoteUrl, removeVerNum, AssetFileManifest.NAME);
                FileReadUtils.Read(remoteFilePath, MimeType.Text, OnReadRemoteManifest);
            }
            else
            {
                OnReadManifestCompleted();
            }
        }

        private void OnReadRemoteManifest(object data, string path, object userData)
        {
            if (null == data)
            {
                Logger.WarnFormat("Remote AssetFileManifest is null:{0}", path);
            }
            else
            {
                remoteManifest = new AssetFileManifest((string)data);
            }

            OnReadManifestCompleted();
        }

        private void OnReadManifestCompleted()
        {
            if (null == remoteManifest)
            {
                compareAction?.Invoke(ErrorCode.Remote);
            }
            else
            {
                localManifest.CompareDiff(remoteManifest);
                compareAction?.Invoke(ErrorCode.None);
            }
        }

        public override long GetUpdateSize()
        {
            if (null != localManifest && null != localManifest.UpdateManifest)
            {
                return localManifest.UpdateManifest.Size;
            }

            return 0;
        }

        protected override Downloader CreateDowner()
        {
            ParallelDownloader downloader = new ParallelDownloader(DownloaderType.Simple);
            foreach (var item in localManifest.UpdateManifest)
            {
                string url = string.Format("{0}/{1}/{2}", remoteUrl, removeVerNum, item.Value.path);
                saveDir = string.Format("{0}/Update", AssetPath.DownloadPath);
                string fileSaveDir = saveDir;
                string parentDir = Path.GetDirectoryName(item.Value.path);
                if (!string.IsNullOrEmpty(parentDir))
                {
                    fileSaveDir = string.Format("{0}/{1}", fileSaveDir, parentDir);
                }

                downloader.AddTask(url, fileSaveDir, item.Value.length);
            }

            return downloader;
        }

        public override void ApplyDownloadFile(Action<float> progress, Action<bool> completed)
        {
            FileUtils.MoveDirectory(saveDir, AssetPath.UpdateAssetsPath);
            FileUtils.DeleteDirectory(saveDir);
        }
    }
}