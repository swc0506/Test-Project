using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public class BundleItem
    {
        public string bundleName;
        public string bundlePath;
        public uint crc;
        public string assetName;
        public BundleModuleEnum bundleModuleType;//类型
        public List<string> bundleDependence;//依赖
        public AssetBundle assetBundle;//AssetBundle对象
        public UnityEngine.Object obj;//资源对象
    }

    /// <summary>
    /// AssetBundle缓存
    /// </summary>
    public class AssetBundleCache
    {
        public AssetBundle assetBundle;//对象
        public int referenceCount;//引用计数

        public void Release()
        {
            assetBundle = null;
            referenceCount = 0;
        }
    }
    
    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        /// <summary>
        /// 已经加载的资源模块
        /// </summary>
        private List<BundleModuleEnum> mAlreadyLoadBundleModuleList = new List<BundleModuleEnum>();
        
        // 所有AssetBundle的信息列表
        private Dictionary<uint, BundleItem> mAllBundleItemDic = new Dictionary<uint, BundleItem>();
        private Dictionary<string, AssetBundleCache> mAllReadyLoadDic = new Dictionary<string, AssetBundleCache>();

        public ClassObjectPool<AssetBundleCache> mBundleCachePool = new ClassObjectPool<AssetBundleCache>(200);
        
        // AssetBundle配置文件加载路径
        private string mBundleConfigPath;

        // AssetBundle配置文件名称
        private string mBundleConfigName;

        //
        private string mAssetsBundleConfigName;
        
        /// <summary>
        /// 生成AB配置文件加载路径
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        /// <returns></returns>
        public bool GeneratorBundleConfigPath(BundleModuleEnum bundleModuleEnum)
        {
            mAssetsBundleConfigName = bundleModuleEnum.ToString().ToLower() + "AssetBundleConfig";
            mBundleConfigName = bundleModuleEnum.ToString().ToLower() + "bundleconfig.unity";
            mBundleConfigPath = BundleSettings.Instance.GetHotAssetsPath(bundleModuleEnum) + mBundleConfigName;
            //存在 return true
            if (!File.Exists(mBundleConfigPath))
            {
                mBundleConfigPath = BundleSettings.Instance.GetAssetsDecompressPath(bundleModuleEnum) + mBundleConfigName;
                if (!File.Exists(mBundleConfigPath))
                {
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// 加载AssetBundle配置文件
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        public void LoadAssetBundleConfig(BundleModuleEnum bundleModuleEnum)
        {
            try
            {
                if (mAlreadyLoadBundleModuleList.Contains(bundleModuleEnum))
                {
                    Debug.Log("该模块配置文件已经加载："+ bundleModuleEnum);
                    return;
                }
                
                if (GeneratorBundleConfigPath(bundleModuleEnum))
                {
                    AssetBundle bundleConfig = null;
                    if (BundleSettings.Instance.bundleEncrypt.isEncrypt)
                    {
                        bundleConfig = AssetBundle.LoadFromMemory(AES.Decrypt(mBundleConfigPath, BundleSettings.Instance.bundleEncrypt.encryptKey));
                    }
                    else
                    {
                        bundleConfig = AssetBundle.LoadFromFile(mBundleConfigPath);
                    }
                    string bundleConfigJson = bundleConfig.LoadAsset<TextAsset>(mAssetsBundleConfigName).text;
                    BundleConfig bundle = JsonConvert.DeserializeObject<BundleConfig>(bundleConfigJson);
                    //存放到字典中进行管理
                    foreach (BundleInfo info in bundle.bundleInfoList)
                    {
                        if (!mAllBundleItemDic.ContainsKey(info.crc))
                        {
                            BundleItem item = new BundleItem
                            {
                                bundleName = info.bundleName,
                                bundlePath = info.bundlePath,
                                crc = info.crc,
                                assetName = info.assetName,
                                bundleDependence = info.bundleDependence,
                                bundleModuleType = bundleModuleEnum
                            };
                            mAllBundleItemDic.Add(item.crc, item);
                        }
                        else
                        {
                            Debug.LogError("LoadAssetBundleConfig error: " + info.bundleName + " already exists");
                        }
                    }
                    bundleConfig.Unload(false);
                    mAlreadyLoadBundleModuleList.Add(bundleModuleEnum);
                }
                else
                {
                    Debug.LogError("AssetBundleConfig Not Find" + mBundleConfigPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Load AssetBundleConfig Failed: " + e);
                throw;
            }
        }

        /// <summary>
        /// 通过资源路径的Crc加载资源所在的AssetBundle
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public BundleItem LoadAssetBundle(uint crc)
        {
            //查询资源是否存在
            if (mAllBundleItemDic.TryGetValue(crc, out BundleItem item))
            {
                //AssetBundle是否加载
                if (item.assetBundle == null)
                {
                    item.assetBundle = LoadAssetBundle(item.bundleName, item.bundleModuleType);
                    //需要加载其他的依赖项
                    foreach (var name in item.bundleDependence)
                    {
                        if (!string.Equals(item.bundleName, name))
                        {
                            LoadAssetBundle(name, item.bundleModuleType);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("LoadAssetBundle error: " + crc + " not found");
            }
            return item;
        }
        
        /// <summary>
        /// 通过AssetBundle名称加载AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="bundleModuleType"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string bundleName, BundleModuleEnum bundleModuleType)
        {
            mAllReadyLoadDic.TryGetValue(bundleName, out AssetBundleCache cache);
            if (cache == null || cache.assetBundle == null)
            {
                cache = mBundleCachePool.Spawn();
                
                string hotFilePath = BundleSettings.Instance.GetHotAssetsPath(bundleModuleType) + bundleName;

                //获取热更模块
                HotAssetsModule module = AssetsFrame.GetHotAssetsModule(bundleModuleType);
                bool isHotPath = module == null ? File.Exists(hotFilePath) :
                    module.HotAssetCount == 0 ? File.Exists(hotFilePath) : module.HotAssetsIsExists(bundleName);
                string bundlePath = isHotPath ? hotFilePath : BundleSettings.Instance.GetAssetsDecompressPath(bundleModuleType) + bundleName;
                //判断是否加密
                if (BundleSettings.Instance.bundleEncrypt.isEncrypt)
                {
                    byte[] bytes = AES.AESFileByteDecrypt(bundlePath, BundleSettings.Instance.bundleEncrypt.encryptKey);
                    cache.assetBundle = AssetBundle.LoadFromMemory(bytes);//二进制读取
                }
                else
                {
                    cache.assetBundle = AssetBundle.LoadFromFile(bundlePath);//文件读取 最快
                }

                if (cache.assetBundle == null)
                {
                    Debug.LogError("LoadAssetBundle error: " + bundleName + " not found");
                    return null;
                }
                mAllReadyLoadDic[bundleName] = cache;
            }
            
            cache.referenceCount++;
            Debug.Log("LoadAssetBundle: " + bundleName + " add referenceCount: " + cache.referenceCount);
            return cache.assetBundle;
        }

        /// <summary>
        /// 释放AssetBundle 并且释放AssetBundle占用内存资源
        /// </summary>
        /// <param name="assetItem"></param>
        /// <param name="unLoad"></param>
        public void ReleaseAssets(BundleItem assetItem, bool unLoad)
        {
            if (assetItem != null)
            {
                assetItem.obj = null;
                ReleaseAssetBundle(assetItem, unLoad);
                if (assetItem.bundleDependence != null)
                {
                    foreach (var name in assetItem.bundleDependence)
                    {
                        //根据内存引用计数释放
                        ReleaseAssetBundle(name, unLoad);
                    }
                }
            }
            else
            {
                Debug.LogError("ReleaseAssets error: " + assetItem.assetName + " not found");
            }
        }
        
        private void ReleaseAssetBundle(BundleItem assetItem, bool unLoad)
        {
            if (!string.IsNullOrEmpty(assetItem.bundleName) && mAllReadyLoadDic.TryGetValue(assetItem.bundleName, out AssetBundleCache cache))
            {
                if (cache.assetBundle != null)
                {
                    cache.referenceCount--;
                    if (cache.referenceCount <= 0)
                    {
                        cache.assetBundle.Unload(unLoad);
                        mAllReadyLoadDic.Remove(assetItem.bundleName);
                        cache.Release();
                        mBundleCachePool.Despawn(cache);
                    }
                    Debug.Log("LoadAssetBundle: " + assetItem.bundleName + " --- referenceCount: " + cache.referenceCount);
                }
            }
        }

        private void ReleaseAssetBundle(string bundleName, bool unLoad)
        {
            if (!string.IsNullOrEmpty(bundleName) && mAllReadyLoadDic.TryGetValue(bundleName, out AssetBundleCache cache))
            {
                if (cache.assetBundle != null)
                {
                    cache.referenceCount--;
                    if (cache.referenceCount <= 0)
                    {
                        cache.assetBundle.Unload(unLoad);
                        mAllReadyLoadDic.Remove(bundleName);
                        cache.Release();
                        mBundleCachePool.Despawn(cache);
                    }
                    Debug.Log("LoadAssetBundle: " + bundleName + " --- referenceCount: " + cache.referenceCount);
                }
            }
        }

        /// <summary>
        /// 根据AB名称，查询该AB包中所有资源的BundleItem
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public List<BundleItem> GetBundleItemByABName(string bundleName)
        {
            List<BundleItem> itemList = new List<BundleItem>();
            foreach (var item in mAllBundleItemDic)
            {
                if (string.Equals(item.Value.bundleName, bundleName))
                {
                    itemList.Add(item.Value);
                }
            }
            return itemList;
        }
    }
}