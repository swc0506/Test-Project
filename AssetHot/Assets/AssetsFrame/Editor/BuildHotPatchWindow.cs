using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildHotPatchWindow : BundleBehaviour
{
    protected string[] buildButtonNames = new string[] { "打包资源",  "上传资源"};
    [HideInInspector]public string patchDes = "请输入本次热更描述...";
    [HideInInspector]public string patchVersion = "1.0.0";
    
    public override void Init()
    {
        base.Init();
    }
    
    public override void OnGUI()
    {
        base.OnGUI();
        GUILayout.BeginArea(new Rect(0,400,800,600));
        
        EditorGUILayout.LabelField("请输入本次热更公告");
        patchDes = EditorGUILayout.TextField(patchDes, GUILayout.Width(800),GUILayout.Height(80));
        
        GUILayout.Space(10);
        patchVersion = EditorGUILayout.TextField("热更资源版本", patchVersion, GUILayout.Width(800),GUILayout.Height(24));
        GUILayout.EndArea();
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
        GUI.DrawTexture(new Rect(545, 13, 30, 30), EditorGUIUtility.IconContent("CollabPush").image);
        
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
