using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace ZM.AssetFrameWork
{
    public class HotAssetsModule
    {
        private HotAssetsManifest mServerHotAssetsManifest;
        private HotAssetsManifest mLocalHotAssetsManifest;
        /// <summary>
        /// 服务器热更资源清单路径
        /// </summary>
        private string mServerHotAssetsManifestPath;
        /// <summary>
        /// 本地热更资源清单路径
        /// </summary>
        private string mLocalHotAssetsManifestPath;
        /// <summary>
        /// 热更资源下载储存路径
        /// </summary>
        private string HotAssetsSavePath => Application.persistentDataPath + "/HotAssets/" + CurBundleModuleEnum + "/";
        /// <summary>
        /// 所有热更的资源列表
        /// </summary>
        private List<HotFileInfo> mAllHotAssetsList = new List<HotFileInfo>();
        /// <summary>
        /// 需要下载的资源列表
        /// </summary>
        private List<HotFileInfo> mNeedDownLoadAssetsList = new List<HotFileInfo>();
        /// <summary>
        /// 当前热更模块类型
        /// </summary>
        private BundleModuleEnum CurBundleModuleEnum{ get; set; }
        
        /// <summary>
        /// 资源已下载大小
        /// </summary>
        public float AssetsDownLoadSizeM { get; set; }
        /// <summary>
        /// 最大资源下载大小
        /// </summary>
        public float AssetsMaxSizeM { get; set; }
        
        /// <summary>
        /// 所有热更资源列表长度
        /// </summary>
        public int HotAssetCount => mAllHotAssetsList.Count;

        /// <summary>
        /// 资源下载器
        /// </summary>
        private AssetsDownLoader mAssetsDownLoader;

        /// <summary>
        /// 下载AssetBundle配置文件完成的回调
        /// </summary>
        private Action<string> onDownLoadABConfigListener;
        
        /// <summary>
        /// 下载AssetBundle文件完成的回调
        /// </summary>
        private Action<string> onDownLoadABListener;

        private MonoBehaviour mMono;
        
        /// <summary>
        /// 下载所有资源完成的回调
        /// </summary>
        public Action<BundleModuleEnum> onDownLoadAllAssetsFinish;
        
        public string UpdateNoticeContent => mServerHotAssetsManifest.updateNotice;

        public HotAssetsModule(BundleModuleEnum bundleModule, MonoBehaviour mono)
        {
            CurBundleModuleEnum = bundleModule;
            mMono = mono;
        }

        /// <summary>
        /// 开始热更
        /// </summary>
        /// <param name="startDownLoadCallback">开始下载的回调</param>
        /// <param name="hotFinish">热更完成回调</param>
        /// <param name="isCheckAssetsVersion">是否检测资源版本</param>
        public void StartHotAssets(Action startDownLoadCallback, Action<BundleModuleEnum> hotFinish = null, bool isCheckAssetsVersion = true)
        {
            onDownLoadAllAssetsFinish += hotFinish;
            if (isCheckAssetsVersion)
            {
                CheckAssetsVersion((isHot, size) =>
                {
                    if (isHot)
                    {
                        StartDownLoadHotAssets(startDownLoadCallback);
                    }
                    else
                    {
                        onDownLoadAllAssetsFinish?.Invoke(CurBundleModuleEnum);
                    }
                });
            }
        }

        /// <summary>
        /// 开始下载热更资源
        /// </summary>
        /// <param name="startDownLoadCallBack"></param>
        private void StartDownLoadHotAssets(Action startDownLoadCallBack)
        {
            //优先下载AssetBundle配置文件
            List<HotFileInfo> downLoadList = new List<HotFileInfo>();
            for (int i = 0; i < mNeedDownLoadAssetsList.Count; i++)
            {
                HotFileInfo hotFileInfo = mNeedDownLoadAssetsList[i];
                //配置文件
                if (hotFileInfo.abName.Contains("config"))
                {
                    downLoadList.Insert(0, hotFileInfo);
                }
                else
                {
                    downLoadList.Add(hotFileInfo);
                }
            }

            //下载队列
            Queue<HotFileInfo> downLoadQueue = new Queue<HotFileInfo>();
            foreach (var hotFileInfo in downLoadList)
            {
                downLoadQueue.Enqueue(hotFileInfo);
            }
            
            //通过资源下载器 下载
            mAssetsDownLoader = new AssetsDownLoader(this, downLoadQueue, mServerHotAssetsManifest.downloadUrl,
                HotAssetsSavePath, DownLoadAssetsSuccess, DownLoadAssetsFailed, DownLoadAssetsFinish);
            
            startDownLoadCallBack?.Invoke();
            //开始下载队列
            mAssetsDownLoader.StartThreadDownLoadQueue();
        }
        
        /// <summary>
        /// 检测资源版本
        /// </summary>
        /// <param name="checkFinishCallback"></param>
        public void CheckAssetsVersion(Action<bool, float> checkFinishCallback)
        {
            GenerateHotAssetsManifest();
            mNeedDownLoadAssetsList.Clear();
            mMono.StartCoroutine(DownLoadHotAssetsManifest(() =>
            {
                //资源清单下载完成 检测是否需要热更 计算需要下载的文件 如果不需要 直接完成
                if (CheckModuleAssetsIsHot())
                {
                    HotAssetsPatch serverHotPatch =
                        mServerHotAssetsManifest.hotAssetsPatches[mServerHotAssetsManifest.hotAssetsPatches.Count - 1];
                    bool isNeedHot = ComputeNeedHotAssetsList(serverHotPatch);
                    if (isNeedHot)
                    {
                        checkFinishCallback?.Invoke(true, AssetsMaxSizeM);
                    }
                    else
                    {
                        checkFinishCallback?.Invoke(false, 0);
                    }
                }
                else
                {
                    checkFinishCallback?.Invoke(false, 0);
                }
            }));
        }

        /// <summary>
        /// 计算需要热更文件列表
        /// </summary>
        /// <param name="serverAssetsPatch"></param>
        /// <returns></returns>
        private bool ComputeNeedHotAssetsList(HotAssetsPatch serverAssetsPatch)
        {
            if (!Directory.Exists(HotAssetsSavePath))
            {
                Directory.CreateDirectory(HotAssetsSavePath);
            }
            
            if(File.Exists(mLocalHotAssetsManifestPath))
                mLocalHotAssetsManifest = JsonConvert.DeserializeObject<HotAssetsManifest>(File.ReadAllText(mLocalHotAssetsManifestPath));
            
            AssetsMaxSizeM = 0;
            foreach (var info in serverAssetsPatch.hotFileInfos)
            {
                //获取本地AssetBundle文件路径
                string localFilePath = HotAssetsSavePath + info.abName;
                mAllHotAssetsList.Add(info);
                //如果本地文件不存在 或者本地文件与服务端不一致，就需要热更
                if (!File.Exists(localFilePath) || info.md5 != GetLocalFileMd5ByBundleName(info.abName))
                {
                    mNeedDownLoadAssetsList.Add(info);
                    AssetsMaxSizeM += info.size / 1024f;
                }
            }
            
            return mNeedDownLoadAssetsList.Count > 0;
        }
        
        public string GetLocalFileMd5ByBundleName(string bundleName)
        {
            if (mLocalHotAssetsManifest!=null&& mLocalHotAssetsManifest.hotAssetsPatches.Count>0)
            {
                HotAssetsPatch localPatch = mLocalHotAssetsManifest.hotAssetsPatches[mLocalHotAssetsManifest.hotAssetsPatches.Count-1];
                foreach (var item in localPatch.hotFileInfos)
                {
                    if (string.Equals(bundleName,item.abName))
                    {
                        return item.md5;
                    }
                }
            }
            return "";
        }

        private bool CheckModuleAssetsIsHot()
        {
            //如果服务端资源清单不存，不需要热更
            if (mServerHotAssetsManifest == null)
                return false;

            //如果本地资源清单文件不存在，说明我们需要热更
            if (!File.Exists(mLocalHotAssetsManifestPath))
                return true;
            
            //判断是否一致
            HotAssetsManifest localHotAssetsManifest =
                JsonConvert.DeserializeObject<HotAssetsManifest>(File.ReadAllText(mLocalHotAssetsManifestPath));
            if (localHotAssetsManifest.hotAssetsPatches.Count == 0 &&
                mServerHotAssetsManifest.hotAssetsPatches.Count != 0)
            {
                return true;
            }
            
            //获取本地热更补丁的最后一个补丁
            HotAssetsPatch localHotPatch =
                localHotAssetsManifest.hotAssetsPatches[localHotAssetsManifest.hotAssetsPatches.Count - 1];
            //获取服务端热更补丁的最后一个补丁
            HotAssetsPatch serverHotPatch =
                mServerHotAssetsManifest.hotAssetsPatches[mServerHotAssetsManifest.hotAssetsPatches.Count - 1];

            if (localHotPatch!=null&& serverHotPatch!=null)
            {
                if (localHotPatch.patchVersion!=serverHotPatch.patchVersion)
                {
                    return true;
                }
                //else
                //{
                //    return false;
                //}
            }

            return serverHotPatch != null;
        }
        

        private IEnumerator DownLoadHotAssetsManifest(Action downLoadFinishCallback)
        {
            string url = BundleSettings.Instance.assetBundleDownLoadUrl + "/HotAssets/" + CurBundleModuleEnum + "AssetsHotManifest.json";
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.timeout = 30;
            Debug.Log("DownLoadHotAssetsManifest url: " + url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("DownLoadHotAssetsManifest error: " + request.error);
            }
            else
            {
                try
                {
                    Debug.Log(CurBundleModuleEnum + " success: " + request.downloadHandler.text);
                    //写入到本地
                    FileHelper.WriteFile(mServerHotAssetsManifestPath, request.downloadHandler.data);
                    mServerHotAssetsManifest = JsonConvert.DeserializeObject<HotAssetsManifest>(request.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError("DownLoadHotAssetsManifest error: " + e.Message);
                }
            }
            downLoadFinishCallback?.Invoke();
        }
        
        private void GenerateHotAssetsManifest()
        {
            mServerHotAssetsManifestPath = Application.persistentDataPath + "/Server" + CurBundleModuleEnum + "AssetsHotManifest.json";
            mLocalHotAssetsManifestPath = Application.persistentDataPath + "/Local" + CurBundleModuleEnum + "AssetsHotManifest.json";
        }

        #region 资源下载回调

        private void DownLoadAssetsSuccess(HotFileInfo hotFileInfo)
        {
            string abName = hotFileInfo.abName.Replace(".unity", string.Empty);
            if (hotFileInfo.abName.ToLower().Contains("bundleconfig"))
            {
                onDownLoadABConfigListener?.Invoke(abName);
                //如果下载成功需要及时去加载配置文件
                AssetBundleManager.Instance.LoadAssetBundleConfig(CurBundleModuleEnum);
            }
            else
            {
                onDownLoadABListener?.Invoke(abName);
            }
            
            HotAssetsManager.downLoadBundleFinish?.Invoke(hotFileInfo);
        }
        
        private void DownLoadAssetsFailed(HotFileInfo hotFileInfo)
        {
            
        }

        private void DownLoadAssetsFinish(HotFileInfo hotFileInfo)
        {
            // 下载完成 删除本地文件 复制到本地文件
            if (File.Exists(mLocalHotAssetsManifestPath))
            {
                File.Delete(mLocalHotAssetsManifestPath);
            }
            File.Copy(mServerHotAssetsManifestPath, mLocalHotAssetsManifestPath);
            onDownLoadAllAssetsFinish?.Invoke(CurBundleModuleEnum);
        }
        
        #endregion

        public void OnMainThreadUpdate()
        {
            mAssetsDownLoader?.OnMainThreadUpdate();
        }

        public void SetDownLoadThreadCount(int threadCount)
        {
            Debug.Log("SetDownLoadThreadCount: " + threadCount + " 模块" + CurBundleModuleEnum);
            if (mAssetsDownLoader != null)
            {
                mAssetsDownLoader.max_Download_Thread_Count = threadCount;
            }
        }

        /// <summary>
        /// 判断热更文件是否存在
        /// </summary>
        public bool HotAssetsIsExists(string bundleName)
        {
            foreach (var hotFileInfo in mAllHotAssetsList)
            {
                if (string.Equals(bundleName, hotFileInfo.abName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}