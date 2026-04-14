using System.Collections.Generic;

[System.Serializable]
public class BundleConfig
{
    /// <summary>
    /// 所有AssetBundle的信息列表
    /// </summary>
    public List<BundleInfo> bundleInfoList;
}

[System.Serializable]
public class BundleInfo
{
    public string bundleName;
    public string bundlePath;
    public uint crc;
    public string assetName;
    public List<string> bundleDependence;
}

/// <summary>
/// 内嵌资源信息
/// </summary>
public class BuiltinBundleInfo
{
    public string fileName;
    public string md5;//校验本地已解压文件是否与包内文件一致，如果不一致，说明本地文件被篡改，需要重新解压（进行校验的前提是 当前解压的模块没有开启热更）
    public float size;//文件大小 用来计算文件解压进度显示
}
