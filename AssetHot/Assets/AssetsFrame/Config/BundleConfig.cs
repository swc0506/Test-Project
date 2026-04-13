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
