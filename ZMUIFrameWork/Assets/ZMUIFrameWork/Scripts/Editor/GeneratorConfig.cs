using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;

public enum GeneratorType
{
    Find,//组件查找
    Bind,//组件绑定
}

public enum ParseType
{
    Name,
    Tag
}

public class GeneratorConfig : Editor
{
    public static string BindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/BindComponent";
    public static string FindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/FindComponent";
    public static string WindowGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/Window";
    public static string OBJATALIST_KEY = "objDataList";
    public static GeneratorType GeneratorType = GeneratorType.Bind;
    public static ParseType ParseType = ParseType.Name;
    public static string[] TAGArr = { "Image", "RawImage", "Text", "Button", "Slider", "Dropdown", "InputField", "Canvas", "Panel", "ScrollRect" ,"Toggle"};
}
