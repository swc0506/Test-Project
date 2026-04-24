using System.Collections;
using UnityEngine;
using ZM.AssetFrameWork;

namespace ZM.AssetFrameWork
{
    public class HotUpdateManager : Singleton<HotUpdateManager>
    {
        private Main mMain;
        private HotAssetsWindow mHotAssetsWindow;
        
        /// <summary>
        /// 热更和解压 
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        public void HotAndUnPackAssets(BundleModuleEnum bundleModuleEnum, Main main)
        {
            mMain = mMain ? mMain : main;
            mHotAssetsWindow = InstantiateResourcesObj<HotAssetsWindow>("HotAssetsWindow");
            IDecompressAssets decompressAssets = AssetsFrame.StartDeCompressBuiltinFile(bundleModuleEnum, () =>
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    InstantiateResourcesObj<UpdateTipsWindow>("UpdateTipsWindow")
                        .InitView("当前网络不可用，请检查网络后重试", () => { NotNetButtonClick(bundleModuleEnum); },
                            () => { NotNetButtonClick(bundleModuleEnum); });
                    return;
                }
                else
                {
                    if (BundleSettings.Instance.bundleHotType == BundleHotEnum.Hot)
                        CheckAssetsVersion(bundleModuleEnum);
                    else
                    {
                        //如果不需要热更，说明用户已经热更过了，资源是最新的，直接进入游戏
                        OnHotAssetsFinish(bundleModuleEnum);
                    }
                }
            });
            
            //更新解压进度
            mHotAssetsWindow.ShowDecompressProgress(decompressAssets);
        }

        private void NotNetButtonClick(BundleModuleEnum bundleModuleEnum)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                CheckAssetsVersion(bundleModuleEnum);
            }
        }

        private void CheckAssetsVersion(BundleModuleEnum bundleModule)
        {
            AssetsFrame.CheckAssetsVersion(bundleModule, (isHot, size) =>
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
        private void StartHotAssets(BundleModuleEnum bundleModule)
        {
            AssetsFrame.HotAssets(bundleModule, OnHotStart, OnHotFinish, null, false);
            //更新热更进度
            mHotAssetsWindow.ShowHotAssetsProgress(AssetsFrame.GetHotAssetsModule(bundleModule));
        }

        /// <summary>
        /// 完成热更回调
        /// </summary>
        /// <param name="bundleModule"></param>
        private void OnHotAssetsFinish(BundleModuleEnum bundleModule)
        {
            AssetsFrame.HotAssets(bundleModule, OnHotStart, OnHotFinish, null, false);
        }

        private T InstantiateResourcesObj<T>(string prefabName)
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
            AssetBundleManager.Instance.LoadAssetBundleConfig(bundleModule);
            mMain.StartCoroutine(InitGameEnv());
        }

        /// <summary>
        /// 初始化游戏环境
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitGameEnv()
        {
            for (int i = 0; i < 100; i++)
            {
                mHotAssetsWindow.progressSlider.value = i / 100f;
                if (i == 1)
                {
                    mHotAssetsWindow.progressText.text = "加载本地资源...";
                }
                else if (i == 20)
                {
                    mHotAssetsWindow.progressText.text = "加载配置文件...";
                }
                else if (i == 50)
                {
                    mHotAssetsWindow.progressText.text = "加载场景...";
                }
                else if (i == 70)
                {
                    mHotAssetsWindow.progressText.text = "加载AB配置文件...";
                    //AssetBundleManager.Instance.LoadAssetBundleConfig(BundleModuleEnum.Hall);
                }
                else if (i == 90)
                {
                    mHotAssetsWindow.progressText.text = "加载游戏配置文件...";
                    LoadGameConfig();
                }
                else if (i == 99)
                {
                    mHotAssetsWindow.progressText.text = "加载地图场景...";
                }
                yield return null;
            }
            
            mMain.StartGame();
            Object.Destroy(mHotAssetsWindow.gameObject);
        }

        public void LoadGameConfig()
        {
            
        }
    }
}