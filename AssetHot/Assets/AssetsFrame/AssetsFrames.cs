using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public partial class AssetsFrame
    {
        /// <summary>
        /// 开始热更
        /// </summary>
        /// <param name="bundleModule">热更模块</param>
        /// <param name="startHotCallBack">开始热更回调</param>
        /// <param name="hotFinish">热更完成回调</param>
        /// <param name="waitDownLoad">等待下载的回调</param>
        /// <param name="isCheck">是否检测资源版本</param>
        public static void HotAssets(BundleModuleEnum bundleModule, Action<BundleModuleEnum> startHotCallBack,
            Action<BundleModuleEnum> hotFinish, Action<BundleModuleEnum> waitDownLoad, bool isCheck)
        {
            Instance.mHotAssets.HotAssets(bundleModule, startHotCallBack, hotFinish, waitDownLoad, isCheck);
        }

        /// <summary>
        /// 检测资源版本时候需要热更，获取需要热更资源的大小
        /// </summary>
        /// <param name="bundleModule">热更模块类型</param>
        /// <param name="callBack">回调</param>
        public static void CheckAssetsVersion(BundleModuleEnum bundleModule, Action<bool, float> callBack)
        {
            Instance.mHotAssets.CheckAssetsVersion(bundleModule, callBack);
        }

        /// <summary>
        /// 获取热更模块
        /// </summary>
        /// <param name="bundleModule">热更模块类型</param>
        /// <returns></returns>
        public static HotAssetsModule GetHotAssetsModule(BundleModuleEnum bundleModule)
        {
            return Instance.mHotAssets.GetHotAssetsModule(bundleModule);
        }
        
        /// <summary>
        /// 开始解压内嵌文件
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static IDecompressAssets StartDeCompressBuiltinFile(BundleModuleEnum bundleModuleEnum, Action callBack)
        {
            return Instance.mDecompressAssets.StartDeCompressBuiltinFile(bundleModuleEnum, callBack);
        }
        
        /// <summary>
        /// 获取解压进度
        /// </summary>
        /// <returns></returns>
        public static float GetDecompressProgress()
        {
            return Instance.mDecompressAssets.GetDecompressProgress();
        }
    }
}
