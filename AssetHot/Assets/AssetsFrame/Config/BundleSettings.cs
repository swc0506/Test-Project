using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using ZM.AssetFrameWork;

public enum BundleHotEnum
{
    NotHot,
    Hot,
}

// 资源加载模式
public enum LoadAssetEnum
{
    Editor,
    AssetBundle,
}

[CreateAssetMenu(menuName = "AssetsBundleSettings", fileName = "AssetsBundleSettings", order = 0)]
public class BundleSettings : ScriptableObject
{
    [TitleGroup("资源加载热更新设置"), LabelText("AssetBundle下载地址")]
    public string assetBundleDownLoadUrl;
    [TitleGroup("AssetBundle打包设置"), LabelText("AssetBundle加密设置")]
    public BundleEncryptToggle bundleEncrypt = new BundleEncryptToggle();
    [TitleGroup("AssetBundle打包设置"), LabelText("打包目标平台")]
    public BuildTarget buildTarget;
    [TitleGroup("AssetBundle打包设置"), LabelText("资源压缩格式")]
    public BuildAssetBundleOptions buildAssetBundleOptions;
    [TitleGroup("资源加载热更设置"), LabelText("资源热更模式")]
    public BundleHotEnum bundleHotType;
    [TitleGroup("资源加载热更设置"), LabelText("资源加载模式")]
    public LoadAssetEnum loadAssetType;
    [TitleGroup("资源加载热更设置"), LabelText("最大下载线程数量")]
    public int maxThreadCount;
    
    [Title("热更文件储存路径")]
    private string HotAssetsPath => Application.persistentDataPath + "/HotAssets/";
    [Title("资源解压路径")]
    private string DecompressAssetsPath => Application.persistentDataPath + "/DecompressAssets/";
    [Title("资源内嵌路径")]
    private string BuiltinAssetsPath => Application.streamingAssetsPath + "/AssetBundle/";
    public string GetAssetsBuiltinPath(BundleModuleEnum bundleModuleEnum)
    {
        return BuiltinAssetsPath + bundleModuleEnum + "/";
    }
    public string GetAssetsDecompressPath(BundleModuleEnum bundleModuleEnum)
    {
        return DecompressAssetsPath + bundleModuleEnum + "/";
    }
    public string GetHotAssetsPath(BundleModuleEnum bundleModuleEnum)
    {
        return HotAssetsPath + bundleModuleEnum + "/";
    }
    
    private static BundleSettings instance;
    public static BundleSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<BundleSettings>("AssetsBundleSettings");
            }
            return instance;
        }
    }
}

[System.Serializable, Toggle("isEncrypt")]
public class BundleEncryptToggle
{
    //是否加密
    public bool isEncrypt;
    [LabelText("加密密钥")]
    public string encryptKey;
}

public enum BuildTarget
{
  iOS = 9,
  Android = 13,
  StandaloneLinux = 17,
  StandaloneWindows64 = 19,
}

public enum BuildAssetBundleOptions
{
  None = 0,
  UncompressedAssetBundle = 1,
  CollectDependencies = 2,
  CompleteAssets = 4,
  DisableWriteTypeTree = 8,
  DeterministicAssetBundle = 16,
  ForceRebuildAssetBundle = 32,
  IgnoreTypeTreeChanges = 64,
  AppendHashToAssetBundleName = 128,
  ChunkBasedCompression = 256,
  StrictMode = 512,
  DryRunBuild = 1024,
  DisableLoadAssetByFileName = 4096,
  DisableLoadAssetByFileNameWithExtension = 8192,
  AssetBundleStripUnityVersion = 32768,
}
