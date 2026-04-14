using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    /// <summary>
    /// 热更新资源清单
    /// </summary>
    public class HotAssetsManifest
    {
        //热更公告
        public string updateNotice;
        //下载地址
        public string downloadUrl;
        //热更新资源补丁列表
        public List<HotAssetsPatch> hotAssetsPatches = new List<HotAssetsPatch>();
    }

    /// <summary>
    /// 热更新资源补丁
    /// </summary>
    public class HotAssetsPatch
    {
        //补丁版本
        public int patchVersion;
        //资源信息列表
        public List<HotFileInfo> hotFileInfos = new List<HotFileInfo>();
    }
    
    /// <summary>
    /// 热更新文件信息
    /// </summary>
    public class HotFileInfo
    {
        public string abName;
        public string md5;
        public float size;
    }
}