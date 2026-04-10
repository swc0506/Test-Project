using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class BundleBehaviour
{
    /// <summary>
    /// 模块数据列表
    /// </summary>
    protected List<BundleModuleData> moduleDataList;

    /// <summary>
    /// 行模块数据列表
    /// </summary>
    protected List<List<BundleModuleData>> rowModuleDataList;

    protected string curPlatFam;

    public virtual void Init()
    {
        // 获取模块数据列表
        moduleDataList = BuildBundleConfig.Instance.assetBundleModuleList;
        rowModuleDataList = new List<List<BundleModuleData>>();

        for (int i = 0; i < moduleDataList.Count; i++)
        {
            //计算模块绘制的行数索引
            int rowIndex = Mathf.FloorToInt(i / 6f);
            if (rowIndex >= rowModuleDataList.Count)
            {
                rowModuleDataList.Add(new List<BundleModuleData>());
            }

            rowModuleDataList[rowIndex].Add(moduleDataList[i]);
        }

#if UNITY_IOS
        curPlatFam = "BuildSettings.iPhone";
#else
        curPlatFam = "BuildSettings.Android";
#endif
    }

    // 打开绘制时调用
    [OnInspectorGUI]
    public virtual void OnGUI()
    {
        if (null == rowModuleDataList)
            return;

        GUIContent content = EditorGUIUtility.IconContent("SceneAsset Icon".Trim(), "测试文字显示");
        content.tooltip = "单机可选中和取消\n快速双击可打开配置窗口";

        for (int i = 0; i < rowModuleDataList.Count; i++)
        {
            GUILayout.BeginHorizontal(); //开始水平布局
            for (int j = 0; j < rowModuleDataList[i].Count; j++)
            {
                BundleModuleData moduleData = rowModuleDataList[i][j];
                if (GUILayout.Button(content, GUILayout.Width(130), GUILayout.Height(170)))
                {
                    moduleData.isBuild = !moduleData.isBuild;

                    if (Time.realtimeSinceStartup - moduleData.lastClickTime <= 0.25f)
                    {
                        BundleModuleConfig.ShowWindow(moduleData.moduleName);
                    }
                    moduleData.lastClickTime = Time.realtimeSinceStartup;
                }

                GUI.Label(new Rect((j + 1) * 20 + (j * 112), 150 * (i + 1) + (i * 20), 115, 20), moduleData.moduleName,
                    new GUIStyle { alignment = TextAnchor.MiddleCenter });

                if (moduleData.isBuild)
                {
                    GUIStyle style = UnityEditorUility.GetGUIStyle("LightmapEditorSelectedHighlight");
                    style.contentOffset = new Vector2(100, -70);
                    GUI.Toggle(new Rect(10 + (j * 133), -160 + 1 * (i + 1) * 170, 120, 160),
                        moduleData.isBuild, EditorGUIUtility.IconContent("Collab"), style);
                }
            }

            if (i == rowModuleDataList.Count - 1)
            {
                DrawAddModuleButton(); // 绘制添加模块按钮
            }

            GUILayout.EndHorizontal(); //结束水平布局
        }

        if (rowModuleDataList.Count == 0)
        {
            DrawAddModuleButton(); // 绘制添加模块按钮
        }

        DrawBuildButton();
    }

    /// <summary>
    /// 绘制打包按钮
    /// </summary>
    public virtual void DrawBuildButton()
    {
    }

    /// <summary>
    /// 打包资源
    /// </summary>
    public virtual void BuildBundle()
    {
    }

    /// <summary>
    /// 绘制添加模块按钮
    /// </summary>
    public virtual void DrawAddModuleButton()
    {
    }
}