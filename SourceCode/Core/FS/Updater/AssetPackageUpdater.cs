using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Core.FS
{
    public class AssetPackageUpdater : AssetUpdater
    {
        private DiffZipManifest diffZipManifest;
        private string saveDir;
        private string verDiffName;
        private string zipPath;
        private string zipOutPath;
        private Action<bool> zipCompleted;

        public AssetPackageUpdater(string remoteUrl, VersionNum removeVer, CompareAction compareAction) : base(
            remoteUrl, removeVer, compareAction)
        {
        }

        public AssetPackageUpdater(string remoteUrl, string removeVer, CompareAction compareAction) : base(remoteUrl,
            removeVer, compareAction)
        {
        }

        protected override void UpdateVersion()
        {
            if (null == diffZipManifest)
            {
                string remoteFilePath = string.Format("{0}/{1}/{2}", remoteUrl, removeVerNum, DiffZipManifest.NAME);
                FileReadUtils.Read(remoteFilePath, MimeType.Text, OnReadRemoteZipManifest);
            }
            else
            {
                OnReadManifestCompleted();
            }
        }

        private void OnReadRemoteZipManifest(object data, string path, object userData)
        {
            if (null == data)
            {
                Logger.WarnFormat("Remote DiffZipManifest is null:{0}", path);
            }
            else
            {
                diffZipManifest = new DiffZipManifest(removeVerNum.ToString(), (string)data);
            }

            OnReadManifestCompleted();
        }

        private void OnReadManifestCompleted()
        {
            if (null == diffZipManifest)
            {
                compareAction?.Invoke(ErrorCode.Remote);
            }
            else
            {
                verDiffName = localManifest.VerNum + "_" + removeVerNum;
                compareAction?.Invoke(ErrorCode.None);
            }
        }

        public override long GetUpdateSize()
        {
            if (removeVerNum > localManifest.VerNum)
            {
                if (null != diffZipManifest && diffZipManifest.GetFileInfo(verDiffName, out var info))
                {
                    return info.length;
                }
            }

            return 0;
        }

        protected override Downloader CreateDowner()
        {
            diffZipManifest.GetFileInfo(verDiffName, out var info);
            string url = string.Format("{0}/{1}/{2}", remoteUrl, removeVerNum, info.path);
            saveDir = string.Format("{0}/Update", AssetPath.DownloadPath);
            zipPath = string.Format("{0}/{1}", saveDir, info.path);
            ResumeDownloader downloader = new ResumeDownloader(url, saveDir, info.length, info.md5);
            return downloader;
        }

        public override void ApplyDownloadFile(Action<float> progress, Action<bool> completed)
        {
            zipOutPath = string.Format("{0}/Temp", saveDir);
            zipCompleted = completed;
            ZipUtils.mainThreadCall = true;
            ZipUtils.Unzip(zipPath, zipOutPath, null, progress, OnZipCompleted);
        }

        private void OnZipCompleted(bool success)
        {
            if (success)
            {
                FileUtils.MoveDirectory(zipOutPath, AssetPath.UpdateAssetsPath);
                FileUtils.DeleteDirectory(saveDir);
            }

            zipCompleted?.Invoke(success);
        }
    }
}