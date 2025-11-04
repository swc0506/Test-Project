using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GeneratorBindComponentTool : Editor
{
    public static List<EditorObjectData> objDataList; //查找对象的数据
    
    [MenuItem("GameObject/生成组件数据脚本(Shift+B) #B", false, 0)]
    static void CreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;//获取到当前选择的物体
        if (obj == null)
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }

        objDataList = new List<EditorObjectData>();
        
        //设置脚本生成路径
        if (!Directory.Exists(GeneratorConfig.BindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.BindComponentGeneratorPath);
        }
        
        PresWindowNodeData(obj.transform, obj.name);
        //存储字段名称
        string dataListJson = JsonConvert.SerializeObject(objDataList);
        PlayerPrefs.SetString(GeneratorConfig.OBJATALIST_KEY, dataListJson);
        
        string creatCs = CreatCS(obj.name);
        string csPath = GeneratorConfig.BindComponentGeneratorPath + "/" + obj.name + "DataComponent.cs";
        
        UIWindowEditor.ShowWindow(creatCs, csPath);
        EditorPrefs.SetString("GeneratorClassName", obj.name + "DataComponent");
    }

    /// <summary>
    /// 解析窗口节点组件数据
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="windowName"></param>
    public static void PresWindowNodeData(Transform trans, string windowName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject obj = trans.GetChild(i).gameObject;
            string name = obj.name;
            if (name.Contains("[") && name.Contains("]"))
            {
                int index = name.IndexOf("]") + 1;
                string fieldName = name.Substring(index, name.Length - index);
                string fieldType = name.Substring(1, index - 2);
                objDataList.Add(new EditorObjectData{fieldName = fieldName, fieldType = fieldType, insId = obj.GetInstanceID()});
            }
            PresWindowNodeData(trans.GetChild(i), windowName);
        }
    }

    /// <summary>
    /// 生成CS脚本
    /// </summary>
    /// <param name="name"></param>
    public static string CreatCS(string name)
    {
        StringBuilder sb = new StringBuilder();
        string nameSpace = "ZMUIFrameWork";
        
        sb.AppendLine("/*------------------------------");
        sb.AppendLine(" *Title:UI自动化组件生成代码生成工具");
        sb.AppendLine(" *Author:SWC");
        sb.AppendLine(" *Data:" + System.DateTime.Now);
        sb.AppendLine(" *Description: 变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI组件数据脚本即可");
        sb.AppendLine(" *注意:生成会覆盖原有代码");
        sb.AppendLine("------------------------------*/");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine("using ZMUIFrameWork.Scripts.Runtime.Base;");
        sb.AppendLine("using ZMUIFrameWork.Scripts.Window;");

        sb.AppendLine();

        if (!string.IsNullOrEmpty(nameSpace))
        {
            sb.AppendLine($"namespace {nameSpace}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"\tpublic class {name + "DataComponent : MonoBehaviour"}");
        sb.AppendLine("\t{");
        
        //根据字段数据列表 声明字段
        foreach (var item in objDataList)
        {
            sb.AppendLine("\t\tpublic " + item.fieldType + " " + item.fieldName.ToLower() + item.fieldType + ";");
        }
        
        //声明初始化组件接口
        sb.AppendLine("\t\tpublic void InitComponent(WindowBase target)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\t//组件查找");

        sb.AppendLine("\t");
        sb.AppendLine("\t\t\t//组件事件绑定");
        //得到逻辑类 WindowBase => LoginWindow
        sb.AppendLine($"\t\t\t{name} mWindow = ({name})target;");
        
        //生成UI事件绑定代码
        foreach (var item in objDataList)
        {
            string type = item.fieldType;
            string methodName = item.fieldName;
            string suffix = "";
            if (type.Contains("Button"))
            {
                suffix = "Click";
                sb.AppendLine(
                    $"\t\t\ttarget.AddButtonClickListener({methodName.ToLower()}{type}, mWindow.On{methodName}Button{suffix});");
            }
            else if (type.Contains("InputField"))
            {
                sb.AppendLine(
                    $"\t\t\ttarget.AddInputFieldListener({methodName.ToLower()}{type}, mWindow.On{methodName}InputChange, mWindow.On{methodName}InputEnd);");
            }
            else if (type.Contains("Toggle"))
            {
                suffix = "Change";
                sb.AppendLine(
                    $"\t\t\ttarget.AddToggleClickListener({methodName.ToLower()}{type}, mWindow.On{methodName}Toggle{suffix});");
            }
        }

        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
        if (!string.IsNullOrEmpty(nameSpace))
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    public static EditorObjectData GetEditorObjectData(int insId)
    {
        foreach (var item in objDataList)
        {
            if (item.insId == insId)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// 编译完成之后自动调用
    /// </summary>
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void AddComponentToWindow()
    {
        //不是生成数据脚本的回调就不处理
        string className = EditorPrefs.GetString("GeneratorClassName");
        if (string.IsNullOrEmpty(className))
        {
            return;
        }
        
        //1.通过反射的方式，从程序集中找到这个脚本，把它挂载到当前物体上
        //获取所有程序集
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //找到Csharp程序集
        var cSharpAssembly = assemblies.First(assembly => assembly.GetName().Name == "Assembly-CSharp");
        //获取类所在的程序集路径
        string reClassName = "ZMUIFrameWork." + className;
        Type type = cSharpAssembly.GetType(reClassName);
        if (type == null)
        {
            return;
        }
        
        //获取要挂载的那个物体
        string windowObjName = className.Replace("DataComponent", "");
        GameObject windowObj = GameObject.Find(windowObjName);
        if (windowObj == null)
        {
            windowObj = GameObject.Find("UIRoot/" + windowObjName);
            if (windowObj == null)
                return;
        }
        
        //先获取现窗口上有没有挂载该数据组件，如果没有挂载在进行挂载
        Component comp = windowObj.GetComponent(type);
        if (comp == null)
        {
            comp = windowObj.AddComponent(type);
        }

        //2.通过反射的方式，遍历数据列表，找到对应的字段，赋值
        //获取对象数据列表
        string dataListJson = PlayerPrefs.GetString(GeneratorConfig.OBJATALIST_KEY);
        List<EditorObjectData> dataList = JsonConvert.DeserializeObject<List<EditorObjectData>>(dataListJson);
        //获取脚本所有的字段
        FieldInfo[] fieldInfos = type.GetFields();

        foreach (var info in fieldInfos)
        {
            foreach (var obj in dataList)
            {
                if (info.Name == obj.fieldName.ToLower() + obj.fieldType)
                {
                    //根据InsId找到对应的对象
                    GameObject uiObj = EditorUtility.InstanceIDToObject(obj.insId) as GameObject;
                    //设置该字段所对应的对象
                    if (string.Equals(obj.fieldType, "GameObject"))
                    {
                        info.SetValue(comp, uiObj);
                    }
                    else
                    {
                        info.SetValue(comp, uiObj.GetComponent(obj.fieldType));
                    }
                    break;
                }
            }
        }
        
        EditorPrefs.DeleteKey("GeneratorClassName");
    }
}