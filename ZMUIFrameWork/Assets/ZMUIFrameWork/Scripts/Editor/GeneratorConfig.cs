using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;

public class GeneratorConfig : Editor
{
    public static string FindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/FindComponent";
    public static string WindowGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/Window";
    public static string OBJATALIST_KEY = "objDataList";
}
