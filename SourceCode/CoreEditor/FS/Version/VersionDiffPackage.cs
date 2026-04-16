using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Core.FS;
using UnityEngine;

namespace CoreEditor.FS
{
    public enum DiffType
    {
        /// <summary>
        /// 每个版本和最新版本的差异放在一起
        /// </summary>
        EveryNewTogether,

        /// <summary>
        /// 在一定范围内和最新版本生成1个差异包,超过范围的就统一个大包
        /// </summary>
        EveryNewBestRange,
    }

    internal class VersionDiffPackage
    {
        private string assetDir;
        private VersionSettingsObject settings;
        private ChannelInfo channelInfo;

        public string DiffPath { get; private set; }

        public VersionDiffPackage(string assetDir, string platform, string channel)
        {
            this.assetDir = assetDir;
            this.settings = VersionSettingsObject.Get();
            this.channelInfo = settings.FindChannelInfo(platform, channel);
        }

        public void GenerateDiff()
        {
            if (null == channelInfo)
            {
                return;
            }

            if (channelInfo.assetVersions.Count < 1)
            {
                AddVersion(channelInfo.assetVersion);
                return;
            }

            switch (settings.diffType)
            {
                case DiffType.EveryNewTogether:
                    GenerateTogether();
                    break;
                case DiffType.EveryNewBestRange:
                    GenerateBestRange();
                    break;
                default:
                    Debug.Log("Generate Diff Type Don't Realize");
                    break;
            }
        }

        private AssetFileManifest ReadFileManifest(string verCode)
        {
            string path = Path.Combine(AssetBundleUtils.VersionManifestBackupPath(channelInfo.channel), verCode,
                AssetFileManifest.NAME);
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                return new AssetFileManifest(text);
            }

            return null;
        }

        private string GenerateUpdateFiles(AssetFileManifest diffManifest, VersionNum ver, string dir)
        {
            string fileDir = Path.Combine(AssetBundleUtils.UpdatePackagesOutPath, channelInfo.channel, dir);
            var iter = diffManifest.GetEnumerator();
            while (iter.MoveNext())
            {
                AssetFileInfo fileInfo = iter.Current.Value;
                string srcPath = Path.Combine(assetDir, fileInfo.path);
                string desPath = Path.Combine(fileDir, fileInfo.path);
                FileUtils.CopyFile(srcPath, desPath);
            }

            PackageAssetsHandler.CreateAssetFileManifestFile(assetDir, fileDir, ver);

            return ZipDirectory(fileDir);
        }

        private string ZipDirectory(string targetDir)
        {
            string name = Path.GetFileName(targetDir) + ".zip";
            string outZipPath = Path.Combine(Directory.GetParent(targetDir).ToString(), name);
            ZipUtils.workOnThread = false;
            ZipUtils.Zip(targetDir, outZipPath, targetDir);
            FileUtils.DeleteDirectory(targetDir);

            return outZipPath;
        }

        private void CompareWhitNewVersionDiff(int rangeCount,
            Action<VersionNum, VersionNum, AssetFileManifest> callback)
        {
            int count = channelInfo.assetVersions.Count;
            VersionNum lastVer = channelInfo.assetVersions[count - 1];
            VersionNum nowVer = new VersionNum(lastVer.major, lastVer.minor, lastVer.build + 1);
            AssetFileManifest nowManifest = PackageAssetsHandler.CreateAssetFileManifest(assetDir, nowVer);
            int processCount = 0;
            for (int i = count - 1; i >= 0; i--)
            {
                VersionNum ver = channelInfo.assetVersions[i];
                AssetFileManifest fileManifest = ReadFileManifest(ver.ToString());
                if (null == fileManifest)
                {
                    continue;
                }

                fileManifest.CompareDiff(nowManifest);
                int diffCount = fileManifest.UpdateManifest.Count;
                //当前和上1个没有变化直接退出
                if (diffCount <= 0 && i == count - 1)
                {
                    break;
                }

                if (diffCount > 0)
                {
                    callback.Invoke(ver, nowVer, fileManifest.UpdateManifest);
                    if (rangeCount > 0 && ++processCount >= rangeCount)
                    {
                        break;
                    }
                }
            }
        }

        private void GenerateTogether()
        {
            VersionNum currVer = new VersionNum();
            AssetFileManifest diffManifest = new AssetFileManifest(currVer);
            CompareWhitNewVersionDiff(-1, (ver, nowVer, updateManifest) =>
            {
                currVer = nowVer;
                diffManifest.AddFileInfo(updateManifest);
            });
            //资源有变化
            if (diffManifest.Count > 0)
            {
                AddVersion(currVer);
                DiffPath = GenerateUpdateFiles(diffManifest, currVer, currVer.ToString());
                Debug.LogFormat("Diff package out path:{0}", DiffPath);
            }
        }

        private void AddVersion(VersionNum ver)
        {
            channelInfo.assetVersions.Add(ver);
            settings.Save();
            //backup AssetFileManifest
            string saveDir = Path.Combine(AssetBundleUtils.VersionManifestBackupPath(channelInfo.channel),
                ver.ToString());
            PackageAssetsHandler.CreateAssetFileManifestFile(assetDir, saveDir, ver);
        }

        private void GenerateBestRange()
        {
            VersionNum currVer = new VersionNum();
            List<string> diffZips = new List<string>();
            CompareWhitNewVersionDiff(settings.rangeCount, (ver, nowVer, updateManifest) =>
            {
                currVer = nowVer;
                string dir = string.Format("{0}/{1}_{2}", currVer, ver, currVer);
                string zipPath = GenerateUpdateFiles(updateManifest, currVer, dir);
                diffZips.Add(zipPath);
            });
            //需要1个大包
            if (diffZips.Count > 0) //&& diffZips.Count < channelInfo.assetVersions.Count)
            {
                AssetFileManifest diffManifest = new AssetFileManifest(currVer);
                CompareWhitNewVersionDiff(-1,
                    (ver, nowVer, updateManifest) => { diffManifest.AddFileInfo(updateManifest); });
                string dir = string.Format("{0}/{1}", currVer, currVer);
                string zipPath = GenerateUpdateFiles(diffManifest, currVer, dir);
                diffZips.Add(zipPath);
            }

            if (diffZips.Count > 0)
            {
                AddVersion(currVer);

                //crate diff zip manifest
                DiffZipManifest diffZipManifest = new DiffZipManifest(currVer.ToString());
                foreach (var item in diffZips)
                {
                    string name = Path.GetFileNameWithoutExtension(item);
                    string md5 = FileUtils.GetFileMD5(item);

                    string dir = Path.GetDirectoryName(item);
                    string fName = md5 + Path.GetExtension(item);
                    string newPath = Path.Combine(dir, fName);
                    FileUtils.MoveFile(item, newPath);
                    string path = Path.GetFileName(newPath);
                    long size = FileUtils.GetFileSize(newPath);

                    AssetFileInfo fileInfo = new AssetFileInfo(path, md5, size);
                    diffZipManifest.AddFileInfo(name, fileInfo);
                }

                string saveDir = Path.Combine(AssetBundleUtils.UpdatePackagesOutPath, channelInfo.channel,
                    currVer.ToString());
                string savePath = Path.Combine(saveDir, DiffZipManifest.NAME);
                FileUtils.CreateFile(savePath, diffZipManifest.ToString());
                DiffPath = ZipDirectory(saveDir);
                Debug.LogFormat("Diff package out path:{0}", DiffPath);
            }
        }
    }
}