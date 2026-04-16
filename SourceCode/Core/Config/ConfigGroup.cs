using System;
using System.Collections.Generic;
using System.IO;
using Core.FS;
using Unity.Collections;
using UnityEngine;
#if FLATBUFFERS
using Google.FlatBuffers;

#endif

namespace Core.Config
{
    public class ConfigGroup : BaseConfigGroup
    {
        private AssetPackage assetPkg;
        private string prefixPath;

        private Dictionary<string, AsyncConfigAsset> loadingConfigMap = new Dictionary<string, AsyncConfigAsset>();
        private string fileLoadDir;

        public bool EnableLazyLoad
        {
            set
            {
                if (value == true)
                {
                    if (lazyLoadAction == null)
                    {
                        lazyLoadAction = new Action<string, Type>(LoadConfig);
                    }
                }
                else
                {
                    lazyLoadAction = null;
                }
            }
        }

        public void SetAssetPackage(AssetPackage assetPkg, string prefixPath = null)
        {
            this.assetPkg = assetPkg;
            this.prefixPath = prefixPath;
        }

        public void SetAssetPackageName(string pkgName, string prefixPath = null)
        {
            SetAssetPackage(ResourceManager.Instance.Get(pkgName), prefixPath);
        }


        public void SetFileLoadDir(string fileLoadDir)
        {
            this.fileLoadDir = fileLoadDir;
        }

        private bool AddConfigFromFile(string name, Type type)
        {
            if (!string.IsNullOrEmpty(fileLoadDir))
            {
                string filePath = string.Format("{0}/{1}.bytes", fileLoadDir, name);
                byte[] bytes = FileUtils.ReadFile(filePath);
                if (null != bytes)
                {
                    CreateConfig(name, type, bytes);
                    Logger.DebugFormat("Load file config from:{0}", filePath);
                    return true;
                }
            }

            return false;
        }

        private bool AddConfig(string name, Type type, AssetObject asset)
        {
            if (null == asset)
            {
                return false;
            }

            if (!HasConfig(name))
            {
                TextAsset textAsset = asset.Get<TextAsset>();
                // ByteBufferAllocator bufferAllocator = new ReadOnlyNativeArrayAllocator(textAsset.GetData<byte>());
                // CreateConfig(name, type, bufferAllocator);
                CreateConfig(name, type, textAsset.bytes);
                asset.DelayRelease();
            }

            return true;
        }

        private string GetConfigPath(string name)
        {
            if (!string.IsNullOrEmpty(prefixPath))
            {
                return string.Format("{0}/{1}.bytes", prefixPath, name);
            }
            else
            {
                return string.Format("{0}.bytes", name);
            }
        }

        public void LoadConfig(string name, Type type)
        {
            if (!HasConfig(name))
            {
                string path = GetConfigPath(name);
                if (AddConfigFromFile(name, type))
                {
                    return;
                }

                AssetObject asset = assetPkg.Load(path);
                AddConfig(name, type, asset);
            }
        }

        public void LoadConfig(Type type)
        {
            LoadConfig(GetTypeName(type), type);
        }

        public void LoadConfig<T>()
        {
            LoadConfig(typeof(T));
        }

        public void LoadConfigs(Dictionary<string, Type> configs)
        {
            foreach (var item in configs)
            {
                LoadConfig(item.Key, item.Value);
            }
        }

        public void LoadConfigs(IEnumerable<Type> types)
        {
            foreach (var item in types)
            {
                LoadConfig(item);
            }
        }


        public void LoadConfigAsync(string name, Type type, LoadConfigAction completed)
        {
            if (!HasConfig(name))
            {
                if (!loadingConfigMap.TryGetValue(name, out var item))
                {
                    string path = GetConfigPath(name);
                    AssetAsyncHandler handler = assetPkg.LoadAsync(path);
                    handler.CompletedEvent += OnLoadConfigCompleted;
                    item = new AsyncConfigAsset(name, type, handler);
                }

                item.completedEvent += completed;
                loadingConfigMap[name] = item;
            }
            else
            {
                completed?.Invoke(name, type, true);
            }
        }

        private void OnLoadConfigCompleted(AssetObject asset, string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (loadingConfigMap.TryGetValue(name, out var item))
            {
                bool res = AddConfigFromFile(name, item.type);
                if (!res)
                {
                    res = AddConfig(item.path, item.type, asset);
                }

                item.Invoke(res);
                loadingConfigMap.Remove(name);
            }
        }

        public void LoadConfigsAsync(Dictionary<string, Type> configs, Action<bool> completed)
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
                LoadConfigAsync(item.Key, item.Value, (name, type, res) =>
                {
                    result &= res;
                    if (++completedCount >= totalCount)
                    {
                        completed?.Invoke(result);
                    }
                });
            }
        }

        public void LoadConfigsAsync(IEnumerable<Type> types, Action<bool> completed)
        {
            int totalCount = 0;
            int completedCount = 0;
            bool result = true;
            foreach (var item in types)
            {
                totalCount++;
                LoadConfigAsync(GetTypeName(item), item, (name, type, res) =>
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

        private void UnloadLoading(string name)
        {
            if (loadingConfigMap.TryGetValue(name, out var item))
            {
                item.handler.CompletedEvent -= OnLoadConfigCompleted;
                loadingConfigMap.Remove(name);
            }
        }

        public override void UnloadConfig(string name)
        {
            base.UnloadConfig(name);
            UnloadLoading(name);
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
            assetPkg = null;
            prefixPath = null;
            loadingConfigMap = null;
        }
    }
}