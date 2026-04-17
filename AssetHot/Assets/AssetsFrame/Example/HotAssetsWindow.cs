using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;

public class HotAssetsWindow : MonoBehaviour
{
    public Slider progressSlider;
    public Text progressText;
    
    private IDecompressAssets mDecompressAssets;
    private HotAssetsModule mHotAssetsModule;
    public GameObject updateNoticeObj;// 更新提示节点
    public Text updateNoticeText;// 更新提示文本节点
    /// <summary>
    /// 显示解压进度
    /// </summary>
    /// <param name="decompress"></param>
    public void ShowDecompressProgress(IDecompressAssets decompress)
    {
        mDecompressAssets = decompress;
        progressText.text = string.Empty;
        updateNoticeText.text = string.Empty;
        progressSlider.value = 0;
    }

    private void Update()
    {
        if (mDecompressAssets != null && !Mathf.Approximately(progressSlider.value, 1.0f))
        {
            Debug.Log("mDecompressAssets.GetDecompressProgress():"+ mDecompressAssets.GetDecompressProgress());
            progressText.text = "资源解压中，过程不需要消耗流量...";
            progressSlider.value = mDecompressAssets.GetDecompressProgress();
        }
        
        if (mHotAssetsModule != null && !Mathf.Approximately(progressSlider.value, 1.0f))
        {
            Debug.Log("AssetsDownLoadSizeM:" + mHotAssetsModule.AssetsDownLoadSizeM + " AssetsMaxSizeM:"+ mHotAssetsModule.AssetsMaxSizeM);
            progressText.text =
                $"资源下载中...{mHotAssetsModule.AssetsDownLoadSizeM:F1}m/{mHotAssetsModule.AssetsMaxSizeM:F1}m";
            progressSlider.value = mHotAssetsModule.AssetsDownLoadSizeM / mHotAssetsModule.AssetsMaxSizeM;
        }
    }
    
    /// <summary>
    /// 显示热更新进度
    /// </summary>
    /// <param name="assetsModule"></param>
    public void ShowHotAssetsProgress(HotAssetsModule assetsModule)
    {
        mDecompressAssets = null;
        progressText.text = string.Empty;
        progressSlider.value = 0;
        mHotAssetsModule = assetsModule;
        updateNoticeObj.SetActive(true);
        updateNoticeText.text = assetsModule.UpdateNoticeContent.Replace("\\n", "\n");
    }
}
