using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class UIWindowEditor : EditorWindow
{
    private string scriptContent;
    private string filePath;
    private Vector2 scroll;
    
    /// <summary>
    /// 显示代码展示窗口
    /// </summary>
    /// <param name="content"></param>
    /// <param name="filePath"></param>
    /// <param name="instDic"></param>
    public static void ShowWindow(string content, string filePath, Dictionary<string, string> instDic = null)
    {
        UIWindowEditor window =
            (UIWindowEditor)GetWindowWithRect(typeof(UIWindowEditor), new Rect(100, 50, 800, 700), true, "Window生成界面");
        window.scriptContent = content;
        window.filePath = filePath;

        //处理代码新增
        if (File.Exists(filePath) && instDic != null)
        {
            //处理新增代码
            string originScript = File.ReadAllText(filePath);
            foreach (var item in instDic)
            {
                //如果没有这个方法就进行插入操作
                if (!originScript.Contains(item.Key))
                {
                    int index = window.GetInterIndex(originScript);
                    originScript = window.scriptContent = originScript.Insert(index, item.Value + "\t\t");
                }
            }
        }
        
        window.Show();
    }

    public void OnGUI()
    {
        //绘制Scroview
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(600), GUILayout.Width(800));
        EditorGUILayout.TextArea(scriptContent);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        
        //绘制脚本生成路径
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextArea("脚本生成路径：" + filePath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        //绘制按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
        {
            //按钮事件
            ButtonClick();
        }
        EditorGUILayout.EndHorizontal();
    }

    public void ButtonClick()
    {
        //生成脚本文件
         if (File.Exists(filePath))
         {
             File.Delete(filePath);
         }
        
         StreamWriter writer = File.CreateText(filePath);
         writer.Write(scriptContent);
         writer.Close();
         AssetDatabase.Refresh();

         if (EditorUtility.DisplayDialog("自动化生成工具", "生成脚本成功", "确定"))
         {
             Close();
         }
    }

    /// <summary>
    /// 获取插入代码的下标
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public int GetInterIndex(string content)
    {
        Regex regex = new Regex("UI组件事件");
        Match match = regex.Match(content);

        Regex regex1 = new Regex("public");
        MatchCollection matchCollection = regex1.Matches(content);

        for (int i = 0; i < matchCollection.Count; i++)
        {
            if (matchCollection[i].Index > match.Index)
            {
                return matchCollection[i].Index;
            }
        }

        return -1;
    }
}