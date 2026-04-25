using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;

public class GameModeItem : MonoBehaviour
{
    public Button mButton;
    public Image mSliderImage;
    public Text mSpeedText; // 速度 m/s
    public Text mRatioText; // 比例 %
    public Text mProgressText; // 进度 30m / 100m
    public Text mTipText;
    public GameObject upDateObj;
    
    public BundleModuleEnum gameType;
    
    private HotAssetsModule mHotAssetsModule;
    private float lastTime;
    private float lastDownSize;
    
    void Start()
    {
        mButton.onClick.AddListener(OnGameClick);
    }
    
    void Update()
    {
        if (mHotAssetsModule != null)
        {
            mProgressText.text =
                $"{mHotAssetsModule.AssetsDownLoadSizeM:0.00}m / {mHotAssetsModule.AssetsMaxSizeM:0.00}m";
            mRatioText.text = $"{mHotAssetsModule.AssetsDownLoadSizeM / mHotAssetsModule.AssetsMaxSizeM * 100:0.00}%";
            mSliderImage.fillAmount = mHotAssetsModule.AssetsDownLoadSizeM / mHotAssetsModule.AssetsMaxSizeM;
            if (Time.realtimeSinceStartup - lastTime > 1f)
            {
                lastTime = Time.realtimeSinceStartup;
                float downSize = mHotAssetsModule.AssetsDownLoadSizeM - lastDownSize;
                lastDownSize = mHotAssetsModule.AssetsDownLoadSizeM;
                mSpeedText.text = $"{downSize:0.00}m/s";
            }
        }
    }

    private void OnGameClick()
    {
        AssetsFrame.CheckAssetsVersion(gameType, OnHotAssetsFinish);
    }

    private void OnHotAssetsFinish(bool isHot, float size)
    {
        // 如果需要热更，则显示更新进度，否则显示已是最新版本
        if (isHot)
        {
            AssetsFrame.HotAssets(gameType, OnStartHotAssets, OnHotAssetsFinish, OnWaitHotAssets, true);
        }
        else
        {
            AssetsFrame.Release(transform.parent.parent.parent.parent.gameObject);
            //直接进入游戏
            AssetsFrame.ClearResourcesAssets(true);
            AssetsFrame.Instantiate("Assets/Demo/" + gameType + "/Prefab/" + gameType + "Window", null, Vector3.zero,
                Vector3.one,
                Quaternion.identity);
        }
    }

    public void OnStartHotAssets(BundleModuleEnum bundleModuleEnum)
    {
        upDateObj.SetActive(true);
        mTipText.text = "正在更新中...";
        mHotAssetsModule = AssetsFrame.GetHotAssetsModule(bundleModuleEnum);
    }

    public void OnWaitHotAssets(BundleModuleEnum bundleModuleEnum)
    {
        upDateObj.SetActive(true);
        mTipText.text = "等待下载中...";
    }
    
    public void OnHotAssetsFinish(BundleModuleEnum bundleModuleEnum)
    {
        mHotAssetsModule = null;
        upDateObj.SetActive(false);
        mTipText.text = "更新完成";
        Debug.Log("更新完成:" + bundleModuleEnum);
    }
}
