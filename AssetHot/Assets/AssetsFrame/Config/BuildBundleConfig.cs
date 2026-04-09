using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[CreateAssetMenu(menuName = "AssetBundle", fileName = "BuildBundleConfig", order = 4)]
public class BuildBundleConfig : ScriptableObject
{
    private static BuildBundleConfig instance;
    
    /// <summary>
    /// 模块资源配置
    /// </summary>
    [SerializeField]
    public List<BundleModuleData> assetBundleModuleList = new List<BundleModuleData>();

    public static BuildBundleConfig Instance
    {
        get
        {
            if (null == instance)
            {
                instance = AssetDatabase.LoadAssetAtPath<BuildBundleConfig>("Assets/AssetsFrame/Config/BuildBundleConfig.asset");
            }

            return instance;
        }
    }
}