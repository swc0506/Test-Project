using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetsFrameWork;

public interface IHotAssets
{
    /// <summary>
    /// 开始热更
    /// </summary>
    /// <param name="bundleModule">热更模块</param>
    /// <param name="startHotCallBack">开始热更回调</param>
    /// <param name="hotFinish">热更完成回调</param>
    /// <param name="waitDownLoad">等待下载的回调</param>
    /// <param name="isCheck">是否检测资源版本</param>
    void HotAssets(BundleModuleEnum bundleModule, Action<BundleModuleEnum> startHotCallBack,
        Action<BundleModuleEnum> hotFinish, Action<BundleModuleEnum> waitDownLoad, bool isCheck);

    /// <summary>
    /// 检测资源版本时候需要热更，获取需要热更资源的大小
    /// </summary>
    /// <param name="bundleModule">热更模块类型</param>
    /// <param name="callBack">回调</param>
    void CheckAssetsVersion(BundleModuleEnum bundleModule, Action<bool, float> callBack);
    
    /// <summary>
    /// 获取热更模块
    /// </summary>
    /// <param name="bundleModule">热更模块类型</param>
    /// <returns></returns>
    HotAssetsModule GetHotAssetsModule(BundleModuleEnum bundleModule);
    
    /// <summary>
    /// 主线程更新
    /// </summary>
    void OnMainThreadUpdate();
}
