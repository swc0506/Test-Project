using UnityEngine;
using ZM.AssetFrameWork;

namespace ZM.AssetFrameWork
{
    public class HotUpdateManager : Singleton<HotUpdateManager>
    {
        public void CheckAssetsVersion(BundleModuleEnum bundleModule)
        {
            FrameBase.Instance.CheckAssetsVersion(bundleModule, (isHot, size) =>
            {
                if (isHot)
                {
                    //当用户使用是流量的时候，需要提示用户是否继续下载
                    if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                        Application.platform == RuntimePlatform.WindowsEditor ||
                        Application.platform == RuntimePlatform.OSXEditor)
                    {
                        //弹出选择框，是否继续下载
                        UpdateTipsWindow updateTipsWindow =
                            InstantiateResourcesObj<UpdateTipsWindow>("UpdateTipsWindow");
                        updateTipsWindow.InitView("当前有" + size.ToString("F2") + "M的资源需要更新，是否继续下载？",
                            () =>
                            {
                                //确认
                                StartHotAssets(bundleModule);
                            },
                            Application.Quit);
                    }
                    else
                    {
                        //进行热更
                        StartHotAssets(bundleModule);
                    }
                }
                else
                {
                    //资源是最新的，直接进入游戏
                    OnHotAssetsFinish(bundleModule);
                }
            });
        }

        /// <summary>
        /// 开始热更
        /// </summary>
        /// <param name="bundleModule"></param>
        public void StartHotAssets(BundleModuleEnum bundleModule)
        {
        }

        /// <summary>
        /// 完成热更回调
        /// </summary>
        /// <param name="bundleModule"></param>
        public void OnHotAssetsFinish(BundleModuleEnum bundleModule)
        {
            FrameBase.Instance.HotAssets(bundleModule, OnHotStart, OnHotFinish, null, false);
        }

        public T InstantiateResourcesObj<T>(string prefabName)
        {
            return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(prefabName)).GetComponent<T>();
        }

        private void OnHotStart(BundleModuleEnum bundleModule)
        {
            Debug.Log("OnHotStart");
        }

        private void OnHotFinish(BundleModuleEnum bundleModule)
        {
            Debug.Log("OnHotFinish");
        }
    }
}