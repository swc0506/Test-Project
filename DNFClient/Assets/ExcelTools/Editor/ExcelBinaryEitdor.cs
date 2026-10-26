using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ExcelBinaryEitdor
{

    
    public static void GennerteExcelBinaryData()
    {
        string[] excelsFilePath = Directory.GetFiles(ExcelExportSetting.ExcelsDataPaths, "*", SearchOption.AllDirectories);
        for (int i = 0; i < excelsFilePath.Length; i++)
        {
            string excelPath = excelsFilePath[i];
            if (Path.GetExtension(excelPath) != ".meta")
            {
                string dataClassName = Path.GetFileNameWithoutExtension(excelPath);
                Type type = GetExcelDataType(dataClassName);
                if (type != null)
                {
                    //Excel数据序列化为二进制
                    ExcelUtility excelUtility = new ExcelUtility(excelPath);
                    excelUtility.ConvertToDataListBinaryFile(ExcelExportSetting.BinaryDataPath, type);
                }
            }
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("Tools/GennerteExcelBinaryData")]
    public static void GennerteExcelToCSharp()
    {
        ////获取所有Excel文件路径
        string[] excelsFilePath = Directory.GetFiles(ExcelExportSetting.ExcelsDataPaths, "*", SearchOption.AllDirectories);
        for (int i = 0; i < excelsFilePath.Length; i++)
        {
            string excelPath = excelsFilePath[i];
            if (Path.GetExtension(excelPath) != ".meta")
            {
                string dataClassName = Path.GetFileNameWithoutExtension(excelPath);
                ExcelUtility excelUtility = new ExcelUtility(excelPath);
                excelUtility.ConverToCSharpFile(dataClassName, ExcelExportSetting.CSharpDataPath);
            }
        }
        AssetDatabase.Refresh();
        EditorPrefs.SetBool("GennerteCSharp",true);
    }

    [UnityEditor.Callbacks.DidReloadScripts(0)]
    static void OnScriptReload()
    {
        if (EditorPrefs.GetBool("GennerteCSharp"))
        {
            EditorPrefs.SetBool("GennerteCSharp", false);
            GennerteExcelBinaryData();
        }
    }
    //根据数据类名 获取该类
    public static Type GetExcelDataType(string name)
    {
        Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblys)
        {
            string className = assembly.GetName().Name;
            if (className == "Assembly-CSharp")
            {
                Type[] types = assembly.GetTypes();
                foreach (var t in types)
                {
                    if (t.Name == name)
                    {
                        return t;
                    }
                }
            }
        }
        return null;
    }
}
