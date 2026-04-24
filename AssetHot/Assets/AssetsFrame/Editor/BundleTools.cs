using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BundleTools
{
    private static string mBundleModuleEnumFilePath = Application.dataPath + "/AssetsFrame/Config/BundleModuleEnum.cs";

    [MenuItem("Frame/GenerateBundleModuleEnum")]
    public static void GenerateBundleModuleEnumFile()
    {
        string namespaceName = "ZM.AssetFrameWork";
        string name = "BundleModuleEnum";

        if (File.Exists(mBundleModuleEnumFilePath))
        {
            File.Delete(mBundleModuleEnumFilePath);
            AssetDatabase.Refresh();
        }

        var write = File.CreateText(mBundleModuleEnumFilePath);
        write.WriteLine("/*----------------------------------------");
        write.WriteLine("/*Title:AssetBundle模块枚举");
        write.WriteLine("/*Author");
        write.WriteLine("/*Date:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        write.WriteLine("/*Modify:");
        write.WriteLine("----------------------------------------*/");
        
        write.WriteLine($"namespace {namespaceName}");
        write.WriteLine("{");
        List<BundleModuleData> moduleList = BuildBundleConfig.Instance.assetBundleModuleList;

        if (null == moduleList || moduleList.Count <= 0)
        {
            return;
        }
        
        write.WriteLine("\t"+$"public enum {name}");
        write.WriteLine("\t{");
        write.WriteLine("\t\tNone,");
        for (int i = 0; i < moduleList.Count; i++)
        {
            write.WriteLine($"\t\t{moduleList[i].moduleName},");
        }
        write.WriteLine("\t}");
        write.WriteLine("}");
        write.Close();
        AssetDatabase.Refresh();
    }
}
