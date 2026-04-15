using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetsFrameWork;

public class Test : MonoBehaviour
{
    private HotAssetsModule assetsModule;
    
    void Start()
    {
        assetsModule = new HotAssetsModule(BundleModuleEnum.Game, this);
        assetsModule.StartHotAssets(StartDownLoadAsset, DownLoadAssetFinish);
    }

    private void StartDownLoadAsset()
    {
        Debug.Log("开始下载资源");
    }
    
    private void DownLoadAssetFinish(BundleModuleEnum moduleEnum)
    {
        Debug.Log("资源下载完成:" + moduleEnum);
    }

    private void Update()
    {
        assetsModule?.OnMainThreadUpdate();
    }
}
