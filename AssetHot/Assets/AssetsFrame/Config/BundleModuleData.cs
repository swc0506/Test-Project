using Sirenix.OdinInspector;

[System.Serializable]
public class BundleModuleData
{
    //AssetBundle模块Id
    public long bundleId;
    //模块名称
    public string moduleName;
    //是否打包
    public bool isBuild;
    //上次点击时间
    public float lastClickTime;
    
    public string[] prefabPathArr;
    public string[] rootFolderPathArr;
    public BundleFileInfo[] signPathArr;
}

[System.Serializable]
public class BundleFileInfo
{
    [HideLabel]
    public string abName = "AB Name";

    [HideLabel]
    [FolderPath]
    public string bundlePath = "Bundle Path...";
}