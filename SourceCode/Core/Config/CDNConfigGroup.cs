using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Core.FS;

namespace Core.Config
{
    public struct CDNFileInfo : IEquatable<CDNFileInfo>
    {
        public string path;
        public string name;
        public string md5;

        public bool Equals(CDNFileInfo other)
        {
            return path == other.path && name == other.name && md5 == other.md5;
        }

        public override bool Equals(object obj)
        {
            return obj is CDNFileInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (path != null ? path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (name != null ? name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (md5 != null ? md5.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public class CDNConfigGroup : BaseConfigGroup
    {
        public static LoadConfigAction completedAction;

        private string groupName;

        private List<string> rootUrls = new List<string>();
        private Dictionary<string, CDNFileInfo> fileMap = new Dictionary<string, CDNFileInfo>();

        private Dictionary<string, AsyncConfigAsset> loadingConfigMap = new Dictionary<string, AsyncConfigAsset>();

        public CDNConfigGroup(string groupName)
        {
            this.groupName = groupName;
        }

        public void SetRootUrls(IEnumerable<string> rootUrls)
        {
            if (null != rootUrls)
            {
                this.rootUrls.Clear();
                this.rootUrls.AddRange(rootUrls);
            }
        }

        public void AddManifestInfo(CDNFileInfo info)
        {
            fileMap[info.path] = info;
        }

        public void AddManifestInfos(IEnumerable<CDNFileInfo> infos)
        {
            if (null != infos)
            {
                foreach (var item in infos)
                {
                    AddManifestInfo(item);
                }
            }
        }

        public void RemoveManifestInfo(string path)
        {
            fileMap.Remove(path);
        }

        public void RemoveManifestInfos(IEnumerable<string> paths)
        {
            if (null != paths)
            {
                foreach (var item in paths)
                {
                    RemoveManifestInfo(item);
                }
            }
        }

        public Dictionary<string, CDNFileInfo>.Enumerator GetFileEnumerator()
        {
            return fileMap.GetEnumerator();
        }

        public void AddConfig(string path, Type type, byte[] bytes)
        {
            CreateConfig(path, type, bytes);
        }

        private string GetFileSavePath(string name)
        {
            var info = fileMap[name];
            string extension = Path.GetExtension(info.path);
            return string.Format("{0}/{1}/{2}{3}", AssetPath.UpdateAssetsPath, groupName, info.md5, extension);
        }

        public void LoadConfig(string path, Type type, LoadConfigAction completed)
        {
            if (string.IsNullOrEmpty(path) || !fileMap.ContainsKey(path))
            {
                completed?.Invoke(path, type, false);
                return;
            }

            if (!HasConfig(path))
            {
                if (!loadingConfigMap.TryGetValue(path, out var item))
                {
                    int count = rootUrls.Count;
                    string[] paths = new string[count + 1];
                    paths[0] = string.Format("file://{0}", GetFileSavePath(path));
                    for (int i = 0; i < count; i++)
                    {
                        paths[i + 1] = string.Format("{0}/{1}", rootUrls[i], path);
                    }

                    FileReadUtils.ChainRead(paths, MimeType.Binary, OnLoadConfigCompleted, path);
                    item = new AsyncConfigAsset(path, type);
                }

                item.completedEvent += completed;
                if (null != completedAction)
                {
                    item.completedEvent += completedAction;
                }

                loadingConfigMap[path] = item;
            }
            else
            {
                completed?.Invoke(path, type, true);
            }
        }

        public void LoadConfig(Type type, LoadConfigAction completed)
        {
            LoadConfig(GetTypeName(type), type, completed);
        }

        private void OnLoadConfigCompleted(object data, string lastPath, object userData)
        {
            string path = (string)userData;
            if (loadingConfigMap.TryGetValue(path, out var item))
            {
                loadingConfigMap.Remove(path);

                byte[] bytes = (byte[])data;
                if (null == bytes)
                {
                    item.Invoke(false);
                    return;
                }

                if (RegexUtils.IsUrlPath(lastPath) && fileMap.TryGetValue(path, out var info))
                {
                    string md5 = FileUtils.GetMD5(bytes);
                    if (info.md5 != md5)
                    {
                        Logger.WarnFormat("CDNConfig md5 error:{0},remoteMd5:{1},fileMd5:{1}", path, info.md5, md5);
                        item.Invoke(false);
                        return;
                    }
                    else
                    {
                        FileUtils.CreateFile(GetFileSavePath(path), bytes);
                    }
                }

                AddConfig(item.path, item.type, bytes);
                item.Invoke(true);
            }
        }

        public void LoadConfigs(Dictionary<string, Type> configs, Action<bool> completed)
        {
            if (configs.Count == 0)
            {
                completed?.Invoke(true);
                return;
            }

            int totalCount = configs.Count;
            int completedCount = 0;
            bool result = true;
            foreach (var item in configs)
            {
                LoadConfig(item.Key, item.Value, (name, type, res) =>
                {
                    result &= res;
                    if (++completedCount >= totalCount)
                    {
                        completed?.Invoke(result);
                    }
                });
            }
        }

        public void LoadConfigs(IEnumerable<Type> types, Action<bool> completed)
        {
            int totalCount = 0;
            int completedCount = 0;
            foreach (var item in types)
            {
                totalCount++;
            }

            bool result = true;
            foreach (var item in types)
            {
                LoadConfig(GetTypeName(item), item, (name, type, res) =>
                {
                    result &= res;
                    if (++completedCount >= totalCount)
                    {
                        completed?.Invoke(result);
                    }
                });
            }

            if (totalCount == 0)
            {
                completed?.Invoke(true);
            }
        }

        private void UnloadLoading(string path)
        {
            loadingConfigMap.Remove(path);
        }

        public override void UnloadConfig(string path)
        {
            base.UnloadConfig(path);
            UnloadLoading(path);
        }

        public override void UnloadAllConfigs()
        {
            base.UnloadAllConfigs();
            List<string> removes = new List<string>();
            foreach (var item in loadingConfigMap)
            {
                if (!dontUnloadSet.Contains(item.Value.path))
                {
                    removes.Add(item.Value.path);
                }
            }

            foreach (var item in removes)
            {
                UnloadLoading(item);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            fileMap = null;
            loadingConfigMap = null;
        }
    }
}