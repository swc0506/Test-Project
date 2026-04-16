using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using Core.FS;
using UnityEngine;
using FileUtils = Core.FileUtils;

namespace CoreEditor.FS
{
    public static class AssetBundleUtils
    {
        private static readonly HashSet<string> IGNORE_SUFFIXES = new HashSet<string>()
        {
            ".cs"
        };

        private static readonly HashSet<string> AB_EXTENSION = new HashSet<string>() { ".ab", "" };

        public static string AssetPackagesOutDir
        {
            get { return Path.Combine("ExtraAssets", "AssetPackages", BuildPlatformPath.GetBuildPlatform()); }
        }

        /// <summary>
        /// 所有资源根目录
        /// </summary>
        public static string ExtraAssetsPath
        {
            get { return Path.Combine(Directory.GetCurrentDirectory(), "ExtraAssets"); }
        }

        /// <summary>
        /// 当前平台下的assetbundles资源包目录
        /// </summary>
        public static string AssetPackagesOutPath
        {
            get { return Path.Combine(ExtraAssetsPath, "AssetPackages", BuildPlatformPath.GetBuildPlatform()); }
        }

        /// <summary>
        /// 当前平台下的更新版本资源包的目录
        /// </summary>
        public static string UpdatePackagesOutPath
        {
            get { return Path.Combine(ExtraAssetsPath, "UpdatePackages", BuildPlatformPath.GetBuildPlatform()); }
        }

        /// <summary>
        /// 当前平台下的更新版本资源manifest的备份目录
        /// </summary>
        public static string VersionManifestBackupPath(string channelName)
        {
            return Path.Combine(UpdatePackagesOutPath, channelName, "Backup");
        }

        /// <summary>
        /// 版本资源临时目录
        /// </summary>
        public static string VersionAssetsTempPath(string channelName)
        {
            return Path.Combine(UpdatePackagesOutPath, channelName, "AssetsTemp");
        }

        /// <summary>
        /// 是否是忽略的打包资源
        /// </summary>
        /// <param bundleName="path"></param>
        /// <returns></returns>
        public static bool IsInvalidBuildAsset(string path)
        {
            return IGNORE_SUFFIXES.Contains(Path.GetExtension(path));
        }

        public static string[] GetBuildDependencies(string path)
        {
            List<string> paths = new List<string>();
            string[] dependencies = AssetDatabase.GetDependencies(path);
            foreach (var item in dependencies)
            {
                if (!IsInvalidBuildAsset(item))
                {
                    paths.Add(item);
                }
            }

            return paths.ToArray();
        }

        public static void SetAssetBundleName(string path, string bundleName)
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer.assetBundleName != bundleName)
            {
                importer.assetBundleName = bundleName;
                // importer.SaveAndReimport();
            }
        }

        public static void CreateAddressManifest(Dictionary<string, string> map, string pkgName, out string path,
            out string bundleName)
        {
            bundleName =  AssetPath.GetAddressManifestBundleName(pkgName);
            AddressManifest manifest = new AddressManifest(map.GetEnumerator());
            
            string name = AddressManifest.NAME + ".txt";
            path = string.Format("Assets/{0}", name);
            string savePath = Path.Combine(Directory.GetCurrentDirectory(), path);
            FileUtils.CreateFile(savePath, manifest.ToString());
            AssetDatabase.Refresh();

            //backup AddressManifest
            string backupPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", pkgName + "_" + name);
            FileUtils.CreateFile(backupPath, manifest.ToString());
        }

        public static void EncryptAssetBundle(string srcPath, string desPath, int offset, string key)
        {
            byte[] rawBuffer = File.ReadAllBytes(srcPath);
            byte[] buffer = new byte[offset + rawBuffer.Length];

            byte[] keyBuffer = null;
            if (!string.IsNullOrEmpty(key))
            {
                keyBuffer = Encoding.UTF8.GetBytes(key);
            }

            //head    
            for (int i = 0; i < offset; i++)
            {
                byte[] headBuffer = rawBuffer;
                if (null != keyBuffer && i % 2 == 0)
                {
                    headBuffer = keyBuffer;
                }

                int length = headBuffer.Length;
                int index = length / (i + 1) * offset % (i + 1);
                index = index > length - 1 ? index % length : index;
                buffer[i] = headBuffer[index];
            }

            //body
            for (int i = 0; i < rawBuffer.Length; i++)
            {
                buffer[i + offset] = rawBuffer[i];
            }

            FileUtils.CreateFile(desPath, buffer);
        }

        public static void EncryptDirectoryAssetBundles(string srcDir, string desDir, int offset, string key)
        {
            if (!Directory.Exists(srcDir))
            {
                return;
            }

            List<string> paths = FileUtils.GetFiles(srcDir, AB_EXTENSION);
            ProgressHandler progressHandler = new ProgressHandler();
            progressHandler.SetInfo(paths.Count, "Process Project AssetBundles");
            foreach (var item in paths)
            {
                progressHandler.Tick();
                string desPath = item.Replace("\\", "/");
                desPath = desDir + desPath.Substring(srcDir.Length);
                EncryptAssetBundle(item, desPath, offset, key);
            }
        }

        [MenuItem("Assets/Clear AssetsBundleName", false, 1100)]
        public static void ClearAssetsBundleName()
        {
            List<Object> objects = AssetUtils.GetSelectAssets();
            var progress = new ProgressHandler();
            progress.SetInfo(objects.Count, "Clear AssetsBundleName");
            foreach (var item in objects)
            {
                progress.Tick();
                string path = AssetDatabase.GetAssetPath(item);
                if (IsInvalidBuildAsset(path))
                {
                    continue;
                }
        
                SetAssetBundleName(path, null);
                string[] dependencies = GetBuildDependencies(path);
                foreach (var dep in dependencies)
                {
                    SetAssetBundleName(dep, null);
                }
            }
        
            AssetDatabase.Refresh();
        }
    }
}