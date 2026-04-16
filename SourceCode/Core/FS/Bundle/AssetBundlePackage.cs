using System;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

namespace Core.FS
{
    /// <summary>
    /// assetbundle 管理器 可以构建多个assetbundle package 
    /// </summary>
    public class AssetBundlePackage : IAssetCachePool<AssetBundleObject>
    {
        public AssetPackage AssetPkg { get; private set; }

        /// <summary>
        /// 更新包路径
        /// </summary>
        public string UpdateBundlePath { get; private set; }

        /// <summary>
        /// 内部包路径
        /// </summary>
        public string InternalBundlePath { get; private set; }

        private AssetBundleManifest assetBundleManifest;
        private AddressManifest addressManifest;
        private readonly Dictionary<string, string[]> dependMap = new Dictionary<string, string[]>();
        private readonly IBundleLoader loader;

        private Action<AssetPackage, bool> completedAction;
        private readonly Dictionary<string, AssetBundleObject> bundleMap = new Dictionary<string, AssetBundleObject>();
        private readonly string disableFlagPath;
        private bool disableUpdateBundle;

        public bool IsLoaded
        {
            get { return null != addressManifest; }
        }

        public bool DisableUpdateBundle
        {
            get { return disableUpdateBundle; }
            set
            {
                if (value != disableUpdateBundle)
                {
                    disableUpdateBundle = value;
                    if (!disableUpdateBundle)
                    {
                        FileUtils.DeleteFile(disableFlagPath);
                    }
                    else
                    {
                        FileUtils.CreateFile(disableFlagPath, "disable");
                    }
                }
            }
        }

        public AssetBundlePackage(AssetPackage assetPkg)
        {
            this.AssetPkg = assetPkg;
            string pkgName = assetPkg.Name;
            UpdateBundlePath = string.Format("{0}/{1}", AssetPath.UpdateAssetsPath, pkgName);
            InternalBundlePath = string.Format("{0}/{1}", AssetPath.InternalAssetsPath, pkgName);

            loader = new FileBundleLoader(this);

            disableFlagPath = string.Format("{0}/{1}/Disable.flag", AssetPath.UpdateAssetsPath, pkgName);
            disableUpdateBundle = FileUtils.ExistsFile(disableFlagPath);
        }

        public static ulong GetOffset(string path)
        {
            //编辑器下不加密偏移
            if (Application.isEditor)
            {
                return 0;
            }
            else
            {
                return (ulong)FSConfig.Instance.Offset;
            }
        }

        public void LoadManifest()
        {
            var bundle = loader.Load(AssetPkg.Name);
            if (null != bundle)
            {
                InitialAssetBundleManifest(bundle);

                //Load AddressManifest
                string addressManifestPath = AssetPath.GetAddressManifestBundleName(AssetPkg.Name);
                bundle = loader.Load(addressManifestPath);
                InitialAddressManifest(bundle);
            }
        }

        public void SetCompletedAction(Action<AssetPackage, bool> completedAction)
        {
            this.completedAction = completedAction;
        }

        public void LoadManifestAsync()
        {
            var asyncOperation = loader.LoadAsync(AssetPkg.Name, 0);
            asyncOperation.CompletedEvent += OnLoadManifest;
        }

        private void OnLoadManifest(AssetBundleObject bundle)
        {
            if (null != bundle)
            {
                InitialAssetBundleManifest(bundle);
                //Load AddressManifest
                string addressManifestPath = AssetPath.GetAddressManifestBundleName(AssetPkg.Name);
                var asyncOperation = loader.LoadAsync(addressManifestPath, 0);
                asyncOperation.CompletedEvent += OnLoadAddressManifest;
            }
            else
            {
                completedAction?.Invoke(AssetPkg, false);
            }
        }

        private void OnLoadAddressManifest(AssetBundleObject bundle)
        {
            InitialAddressManifest(bundle);
            completedAction?.Invoke(AssetPkg, null != bundle);
        }

        private void InitialAssetBundleManifest(AssetBundleObject bundle)
        {
            dependMap.Clear();
            assetBundleManifest = bundle.Result.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] allBundles = assetBundleManifest.GetAllAssetBundles();
            foreach (var item in allBundles)
            {
                dependMap.Add(item, assetBundleManifest.GetAllDependencies(item));
            }

            bundle.Dispose(false);
        }

        private void InitialAddressManifest(AssetBundleObject bundle)
        {
            if (null != bundle)
            {
                TextAsset manifestText = bundle.Result.LoadAsset<TextAsset>(AddressManifest.NAME);
                addressManifest = new AddressManifest(manifestText.text);
                bundle.Dispose();
            }
        }

        public bool TryGetDependencies(string path, out string[] dependencies)
        {
            if (dependMap.TryGetValue(path, out dependencies))
            {
                return true;
            }

            return false;
        }

        public string GetAddressPath(string path)
        {
            if (null != addressManifest)
            {
                return addressManifest.GetAddress(path);
            }

            return null;
        }

        public AssetBundleObject Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new NullReferenceException("LoadBundle path is null");
            }

            return loader.Load(path);
        }

        public AssetBundleAsyncOperation LoadAsync(string path, int priority)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new NullReferenceException("LoadBundle path is null");
            }

            AssetBundleAsyncOperation asyncOperation = loader.LoadAsync(path, priority);
            return asyncOperation;
        }

        void IAssetCachePool<AssetBundleObject>.Push(AssetBundleObject bundle)
        {
            bundleMap.Add(bundle.Path, bundle);
        }

        void IAssetCachePool<AssetBundleObject>.Pop(AssetBundleObject bundle)
        {
            bundleMap.Remove(bundle.Path);
        }

        void IAssetCachePool<AssetBundleObject>.DelayDecRef(AssetBundleObject asset, float delayTime)
        {
        }

        public bool TryGet(string path, out AssetBundleObject bundle)
        {
            if (!string.IsNullOrEmpty(path) && bundleMap.TryGetValue(path, out bundle))
            {
                return true;
            }

            bundle = null;
            return false;
        }

        public void SetMaxAsyncCount(int value)
        {
            loader.SetMaxAsyncCount(value);
        }

        public void StopAllAsync()
        {
            loader.StopAllAsync();
        }

        public void Update(float deltaTime)
        {
            loader.Update(deltaTime);
        }
    }
}