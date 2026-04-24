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
                instance = AssetDatabase.LoadAssetAtPath<BuildBundleConfig>("Assets/AssetsFrame/Resources/BuildBundleConfig.asset");
            }

            return instance;
        }
    }

    /// <summary>
    /// 通过模块名称获取模块数据
    /// </summary>
    /// <param name="moduleName"></param>
    public BundleModuleData GetBundleDataByName(string moduleName)
    {
        foreach (var data in assetBundleModuleList)
        {
            if (string.Equals(data.moduleName, moduleName))
                return data;
        }
        return null;
    }
    
    /// <summary>
    /// 通过模块名称移除模块数据
    /// </summary>
    /// <param name="moduleName"></param>
    public void RemoveBundleDataByName(string moduleName)
    {
        for (int i = 0; i < assetBundleModuleList.Count; i++)
        {
            if (string.Equals(assetBundleModuleList[i].moduleName, moduleName))
            {
                assetBundleModuleList.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// 保存模块数据
    /// </summary>
    /// <param name="data"></param>
    public void SaveModuleData(BundleModuleData data)
    {
        if (assetBundleModuleList.Contains(data))
        {
            for (int i = 0; i < assetBundleModuleList.Count; i++)
            {
                if (assetBundleModuleList[i] == data)
                {
                    assetBundleModuleList[i] = data;
                    break;
                }
            }
        }
        else
        {
            assetBundleModuleList.Add(data);
        }
        Save();
    }

    public void Save()
    {
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        #endif
    }
}