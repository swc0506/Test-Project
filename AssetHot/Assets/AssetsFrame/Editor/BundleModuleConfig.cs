using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

public class BundleModuleConfig : OdinEditorWindow
{
    [PropertySpace(spaceAfter:5, spaceBefore:5)]
    [Required("请输入模块名称")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    [LabelText("模块名称")]
    public string moduleName;

    [ReadOnly]
    [HideLabel]
    [TabGroup("预制体包")]
    [DisplayAsString]
    public string prefabTable = "该文件下的所有预制体都会单独打成一个AB包";
    
    [ReadOnly]
    [HideLabel]
    [TabGroup("文件夹子包")]
    [DisplayAsString]
    public string rootFolderSubBundle = "该文件下的所有子文件夹都会单独打成一个AB包";
    
    [ReadOnly]
    [HideLabel]
    [TabGroup("单个补丁包")]
    [DisplayAsString]
    public string signBundle = "指定文件夹会单独打成一个AB包";
    
    [FolderPath]
    [TabGroup("预制体包")]
    [LabelText("预制体资源路径配置")]
    public string[] prefabPathArr = new[] { "Path..." };
    
    [FolderPath]
    [TabGroup("文件夹子包")]
    [LabelText("文件夹子包资源路径配置")]
    public string[] rootFolderPathArr = new[] { "Path..." };
    
    [TabGroup("单个补丁包")]
    [LabelText("单个补丁包源路径配置")]
    public BundleFileInfo[] signPathArr = new BundleFileInfo[] {};
    
    public static void ShowWindow(string moduleName)
    {
        BundleModuleConfig window = GetWindowWithRect<BundleModuleConfig>(new Rect(0, 0, 600, 600)); 
        window.Show();
        //更新窗口数据
        BundleModuleData data = BuildBundleConfig.Instance.GetBundleDataByName(moduleName);
        if (null != data)
        {
            window.moduleName = data.moduleName;
            window.prefabPathArr = data.prefabPathArr;
            window.rootFolderPathArr = data.rootFolderPathArr;
            window.signPathArr = data.signPathArr;
        }
    }
    
    /// <summary>
    /// 保存配置按钮
    /// </summary>
    [OnInspectorGUI]
    public void DrawSaveConfigButton()
    {
        GUILayout.BeginArea(new Rect(0, 510, 600, 200));
        if (GUILayout.Button("删除配置", GUILayout.Height(47)))
        {
            DeleteConfig();
        }
        GUILayout.EndArea();
        
        GUILayout.BeginArea(new Rect(0, 550, 600, 200));
        if (GUILayout.Button("保存配置", GUILayout.Height(47)))
        {
            SaveConfig();
        }
        GUILayout.EndArea();
    }
    
    private void DeleteConfig()
    {
        BuildBundleConfig.Instance.RemoveBundleDataByName(moduleName);
        UnityEditor.EditorUtility.DisplayDialog("提示", "删除成功", "确定");
        Close();
        BuildWindow.ShowAssetBundleWindow();
    }
    
    private void SaveConfig()
    {
        if (string.IsNullOrEmpty(moduleName))
        {
            UnityEditor.EditorUtility.DisplayDialog("提示", "模块名称不能为空", "确定");
            return;
        }
        
        BundleModuleData moduleData = BuildBundleConfig.Instance.GetBundleDataByName(moduleName);
        if (null == moduleData)
        {
            moduleData = new BundleModuleData(); 
            BuildBundleConfig.Instance.SaveModuleData(moduleData);
        }
        
        moduleData.moduleName = moduleName;
        moduleData.prefabPathArr = prefabPathArr;
        moduleData.rootFolderPathArr = rootFolderPathArr;
        moduleData.signPathArr = signPathArr;
        
        UnityEditor.EditorUtility.DisplayDialog("提示", "保存成功", "确定");
        Close();
        BuildWindow.ShowAssetBundleWindow();
    }
}
