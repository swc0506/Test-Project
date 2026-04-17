using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public abstract class IDecompressAssets
    {
        // 总大小
        public float TotalSize { get; protected set; }
        // 已经解压大小
        public float AlreadySize { get; protected set; }
        // 是否开始解压
        public bool IsStart { get; protected set; }

        /// <summary>
        /// 开始解压内嵌文件
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        abstract public IDecompressAssets StartDeCompressBuiltinFile(BundleModuleEnum bundleModuleEnum, Action callBack);
        
        /// <summary>
        /// 获取解压进度
        /// </summary>
        /// <returns></returns>
        abstract public float GetDecompressProgress();
    }
}