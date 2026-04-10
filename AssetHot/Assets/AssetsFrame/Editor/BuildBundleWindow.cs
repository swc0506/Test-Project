using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZM.AssetsFrameWork;

public class BuildBundleWindow : BundleBehaviour
{
    protected string[] buildButtonNames = new string[] { "打包资源",  "内嵌资源"};
    
    public override void Init()
    {
        base.Init();
    }
    
    /// <summary>
    /// 绘制添加资源模块的按钮
    /// </summary>
    public override void DrawAddModuleButton()
    {
        base.DrawAddModuleButton();

        GUIContent content = EditorGUIUtility.IconContent("CollabCreate Icon".Trim(), "");
        if (GUILayout.Button(content, GUILayout.Width(130), GUILayout.Height(170)))
        {
            BundleModuleConfig.ShowWindow(string.Empty);
        }
    }

    public override void DrawBuildButton()
    {
        base.DrawBuildButton();
        GUILayout.BeginArea(new Rect(0,555,800,600));
        
        GUILayout.BeginHorizontal();

        for (int i = 0; i < buildButtonNames.Length; i++)
        {
            GUIStyle style = UnityEditorUility.GetGUIStyle("PreButtonBlue");
            style.fixedHeight = 55;
            if (GUILayout.Button(buildButtonNames[i], style, GUILayout.Height(400)))
            {
                if (i == 0)
                {
                    BuildBundle();
                }
                else
                {
                    CopyBundleToStreamingAssetsPath();
                }
            }
        }
        
        // 绘制平台图标
        GUI.DrawTexture(new Rect(130, 13, 30, 30), EditorGUIUtility.IconContent(curPlatFam).image);
        GUI.DrawTexture(new Rect(545, 13, 30, 30), EditorGUIUtility.IconContent("SceneSet Icon").image);
        
        GUILayout.EndHorizontal();
        
        GUILayout.EndArea();
    }

    public override void BuildBundle()
    {
        base.BuildBundle();
        foreach (var item in moduleDataList)
        {
            if (item.isBuild)
            {
                BuildBundleCompiler.BuildAssetBundle(item, BundleType.AssetBundle);
            }
        }
    }

    /// <summary>
    /// 内嵌资源
    /// </summary>
    public void CopyBundleToStreamingAssetsPath()
    {
        foreach (var item in moduleDataList)
        {
            if (item.isBuild)
            {
                
            }
        }
    }
}
