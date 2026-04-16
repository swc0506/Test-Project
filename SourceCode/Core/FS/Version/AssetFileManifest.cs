using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Core.FS
{
    internal class SortValue : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return x.CompareTo(y);
        }
    }

    public class AssetFileManifest : IEnumerable<KeyValuePair<string, AssetFileInfo>>
    {
        public const string NAME = "AssetManifest.txt";

        public VersionNum VerNum { get; private set; }

        private readonly Dictionary<string, AssetFileInfo> fileMap = new Dictionary<string, AssetFileInfo>();

        public AssetFileManifest DeleteManifest { get; private set; }
        public AssetFileManifest UpdateManifest { get; private set; }

        public int Count
        {
            get { return fileMap.Count; }
        }

        public long Size
        {
            get
            {
                long size = 0;
                foreach (var item in fileMap)
                {
                    size += item.Value.length;
                }

                return size;
            }
        }


        public AssetFileManifest(VersionNum verNum)
        {
            this.VerNum = verNum;
        }

        public AssetFileManifest(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            using (StringReader strReader = new StringReader(content))
            {
                int index = 0;
                string line;
                while (!string.IsNullOrEmpty((line = strReader.ReadLine())))
                {
                    if (++index == 1)
                    {
                        VerNum = new VersionNum(line);
                    }
                    else
                    {
                        AssetFileInfo fileInfo = new AssetFileInfo(line);
                        fileMap.Add(fileInfo.path, fileInfo);
                    }
                }
            }
        }


        public IEnumerator<KeyValuePair<string, AssetFileInfo>> GetEnumerator()
        {
            return fileMap.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddFilePackage(string pkgDir, FileType type)
        {
            switch (type)
            {
                case FileType.AssetBundle:
                    LoadFromAssetBundleManifest(pkgDir);
                    break;
                case FileType.Normal:
                    ScanFromDirectory(pkgDir);
                    break;
            }
        }

        public void AddFileInfo(string path, string md5, long size, FileType type)
        {
            AssetFileInfo info = new AssetFileInfo(path, md5, size, type);
            fileMap[path] = info;
        }

        public void AddFileInfo(AssetFileInfo info)
        {
            if (!fileMap.ContainsKey(info.path))
            {
                fileMap.Add(info.path, info);
            }
        }

        public void AddFileInfo(AssetFileManifest manifest)
        {
            foreach (var item in manifest)
            {
                AddFileInfo(item.Value);
            }
        }

        private void LoadFromAssetBundleManifest(string pkgDir)
        {
            string pkgName = Path.GetFileName(pkgDir);
            string manifestPath = string.Format("{0}/{1}", pkgDir, pkgName);
            ulong offset = (ulong)FSConfig.Instance.Offset;
            AssetBundle ab = AssetBundle.LoadFromFile(manifestPath, 0, offset);
            if (null == ab)
            {
                return;
            }

            AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] bundles = manifest.GetAllAssetBundles();
            foreach (var item in bundles)
            {
                string path = string.Format("{0}/{1}", pkgName, item);
                string md5 = manifest.GetAssetBundleHash(item).ToString();
                long size = FileUtils.GetFileSize(string.Format("{0}/{1}", pkgDir, item));
                AssetFileInfo info = new AssetFileInfo(path, md5, size, FileType.AssetBundle);
                fileMap.Add(info.path, info);
            }

            ab.Unload(true);

            //AssetBundleManifest
            string dirName = Path.GetDirectoryName(pkgDir);
            AssetFileInfo manifestInfo = GetAssetFileInfo(manifestPath, dirName, FileType.AssetBundle);
            fileMap.Add(manifestInfo.path, manifestInfo);
        }

        private void ScanFromDirectory(string pkgDir)
        {
            if (!Directory.Exists(pkgDir))
            {
                return;
            }

            string dirName = Path.GetDirectoryName(pkgDir);
            List<string> files = FileUtils.GetFiles(pkgDir);
            foreach (var item in files)
            {
                AssetFileInfo info = GetAssetFileInfo(item, dirName, FileType.Normal);
                fileMap.Add(info.path, info);
            }
        }

        private AssetFileInfo GetAssetFileInfo(string filePath, string dirName, FileType type)
        {
            string path = filePath.Remove(0, dirName.Length + 1);
            string md5 = FileUtils.GetFileMD5(filePath);
            long size = FileUtils.GetFileSize(filePath);
            return new AssetFileInfo(path, md5, size, type);
        }

        /// <summary>
        /// 对比差异
        /// </summary>
        /// <param name="fileManifest">另外的文件集</param>
        public void CompareDiff(AssetFileManifest newManifest)
        {
            DeleteManifest = new AssetFileManifest(VerNum);
            UpdateManifest = new AssetFileManifest(VerNum);
            //delete
            foreach (var item in fileMap)
            {
                if (!newManifest.fileMap.ContainsKey(item.Key))
                {
                    DeleteManifest.fileMap.Add(item.Key, item.Value);
                }
            }

            //update
            foreach (var item in newManifest.fileMap)
            {
                //新增的
                if (!fileMap.TryGetValue(item.Key, out var info))
                {
                    UpdateManifest.fileMap.Add(item.Key, item.Value);
                    continue;
                }

                //修改的
                if (!item.Value.Equals(info))
                {
                    UpdateManifest.fileMap.Add(item.Key, item.Value);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine(VerNum.ToString());
            SortedDictionary<string, AssetFileInfo> map =
                new SortedDictionary<string, AssetFileInfo>(fileMap, new SortValue());
            foreach (var item in map)
            {
                strBuilder.AppendLine(item.Value.ToString());
            }

            return strBuilder.ToString();
        }
        
    }
}