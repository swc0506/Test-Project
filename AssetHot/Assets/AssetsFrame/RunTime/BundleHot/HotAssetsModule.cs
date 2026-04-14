using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using ZM.AssetFrameWork;

namespace ZM.AssetsFrameWork
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
        /// 当前热更模块类型
        /// </summary>
        public BundleModuleEnum CurBundleModuleEnum{ get; set; }
        
        public Action<BundleModuleEnum> onDownLoadAllAssetsFinish;
        
        public HotAssetsModule(BundleModuleEnum bundleModule)
        {
            CurBundleModuleEnum = bundleModule;
        }

        /// <summary>
        /// 开始热更
        /// </summary>
        /// <param name="startDownLoadCallback">开始下载的回调</param>
        /// <param name="hotFinish">热更完成回调</param>
        /// <param name="isCheckAssetsVersion">是否检测资源版本</param>
        public void StartHotAssets(Action startDownLoadCallback, Action<BundleModuleEnum> hotFinish = null, bool isCheckAssetsVersion = true)
        {
            onDownLoadAllAssetsFinish = hotFinish;
            if (isCheckAssetsVersion)
            {
            }
            else
            {
            }
        }
        
        /// <summary>
        /// 检测资源版本
        /// </summary>
        /// <param name="checkFinishCallback"></param>
        private void CheckAssetsVersion(Action<bool, float> checkFinishCallback)
        {
            GenerateHotAssetsManifest();
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
    }
}