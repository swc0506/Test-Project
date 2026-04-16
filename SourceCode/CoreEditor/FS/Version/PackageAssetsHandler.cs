using System.Collections.Generic;
using System.IO;
using Core;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    public static class PackageAssetsHandler
    {
        public static void EncryptAssetBundles(string srcDir, string desDir)
        {
            int bundleOffset = FSConfig.Instance.Offset;
            string mappingKey = FSConfig.Instance.Flag;
            AssetBundleUtils.EncryptDirectoryAssetBundles(srcDir, desDir, bundleOffset, mappingKey);
        }

        private static void EncryptProjectAssetBundles(string projectPath, string desDir)
        {
            string srcDir = Path.Combine(projectPath, AssetBundleUtils.AssetPackagesOutDir);
            EncryptAssetBundles(srcDir, desDir);
        }

        public static void EncryptPackageAssetBundles(string desDir)
        {
            string projectPath = Directory.GetCurrentDirectory();
            EncryptProjectAssetBundles(projectPath, desDir);

            var extraProjectsAsset = VersionSettingsObject.Get().extraProjectsAsset;
            foreach (var item in extraProjectsAsset)
            {
                EncryptProjectAssetBundles(item, desDir);
            }
        }

        public static AssetFileManifest CreateAssetFileManifest(string assetsDir, VersionNum ver)
        {
            AssetFileManifest fileManifest = new AssetFileManifest(ver);
            List<string> subDirs = FileUtils.GetSubDirectories(assetsDir);
            foreach (var item in subDirs)
            {
                string abManifestPath = Path.Combine(item, Path.GetFileName(item));
                if (File.Exists(abManifestPath)) //是assetbundleManifest文件
                {
                    fileManifest.AddFilePackage(item, FileType.AssetBundle);
                }
                else //自定义文件,收集fileManifest
                {
                    fileManifest.AddFilePackage(item, FileType.Normal);
                }
            }

            return fileManifest;
        }

        public static void CreateAssetFileManifestFile(string assetsDir, string saveDir, VersionNum ver)
        {
            AssetFileManifest fileManifest = CreateAssetFileManifest(assetsDir, ver);
            //Add FileManifest Self
            string manifestContent = fileManifest.ToString();
            string md5 = FileUtils.GetStringMD5(manifestContent);
            long size = 0;
            fileManifest.AddFileInfo(AssetFileManifest.NAME, md5, size, FileType.Normal);

            //Create FileManifest
            string manifestPath = Path.Combine(saveDir, AssetFileManifest.NAME);
            string content = fileManifest.ToString();
            FileUtils.CreateFile(manifestPath, content);
        }

        public static void CreateAppInfoFile(string platform, string channel)
        {
            VersionSettingsObject settings = VersionSettingsObject.Get();
            ChannelInfo channelInfo = settings.FindChannelInfo(platform, channel);
            if (null == channelInfo)
            {
                return;
            }

            //AppBaseInfo
            AppBaseInfo appInfo = new AppBaseInfo();
            appInfo.channel = channelInfo.channel;
            appInfo.remoteUrls = channelInfo.remoteUrls;
            appInfo.appVersion = channelInfo.appVersion.ToString();
            appInfo.assetVersion = channelInfo.assetVersion.ToString();
            string jsonText = JsonUtils.ToJson(appInfo);
            string savePath = Path.Combine(Directory.GetCurrentDirectory(), AppInfo.PATH);
            FileUtils.CreateFile(savePath, jsonText);
            AssetDatabase.Refresh();
        }

        public static void CreateAppInfoFile(string channelName)
        {
            string platformName = BuildPlatformPath.GetBuildPlatform();
            CreateAppInfoFile(platformName, channelName);
        }
    }
}