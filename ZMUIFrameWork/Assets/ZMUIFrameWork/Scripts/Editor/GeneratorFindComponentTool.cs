using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class EditorObjectData
{
    public int insId;
    public string fieldName;
    public string fieldType;
}

public class GeneratorFindComponentTool : Editor
{
    public static Dictionary<int, string> objFindPathDic; //key 物体的insId, value 代表物体的查找路径
    public static List<EditorObjectData> objDataList; //查找对象的数据
    
    [MenuItem("GameObject/生成组件查找脚本(Shift+U) #U", false, 0)]
    static void CreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;//获取到当前选择的物体
        if (obj == null)
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }

        objDataList = new List<EditorObjectData>();
        objFindPathDic = new Dictionary<int, string>();
        
        //设置脚本生成路径
        if (!Directory.Exists(GeneratorConfig.FindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.FindComponentGeneratorPath);
        }
        
        PresWindowNodeData(obj.transform, obj.name);
        //存储字段名称
        string dataListJson = JsonConvert.SerializeObject(objDataList);
        PlayerPrefs.SetString(GeneratorConfig.OBJATALIST_KEY, dataListJson);
        
        string creatCs = CreatCS(obj.name);
        string csPath = GeneratorConfig.FindComponentGeneratorPath + "/" + obj.name + "UIComponent.cs";
        
        UIWindowEditor.ShowWindow(creatCs, csPath);
        //生成脚本文件
        // if (File.Exists(csPath))
        // {
        //     File.Delete(csPath);
        // }
        //
        // StreamWriter writer = File.CreateText(csPath);
        // writer.Write(creatCs);
        // writer.Close();
        // AssetDatabase.Refresh();
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
                
                //计算过该节点的查找路径
                string objPath = name;
                Transform parent = obj.transform;
                for (int k = 0; k < 20; k++)
                {
                    parent = parent.parent;
                    //父节点是当前窗口时，查找结束
                    if (string.Equals(parent.name, windowName))
                    {
                        break;
                    }

                    objPath = objPath.Insert(0, parent.name + "/");
                }
                objFindPathDic.Add(obj.GetInstanceID(), objPath);
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
        sb.AppendLine(" *Title:UI自动化组件查找代码生成工具");
        sb.AppendLine(" *Author:SWC");
        sb.AppendLine(" *Data:" + System.DateTime.Now);
        sb.AppendLine(" *Description: 变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI组件查找脚本即可");
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

        sb.AppendLine($"\tpublic class {name + "UIComponent"}");
        sb.AppendLine("\t{");
        
        //根据字段数据列表 声明字段
        foreach (var item in objDataList)
        {
            sb.AppendLine("\t\tpublic " + item.fieldType + " " + item.fieldName.ToLower() + item.fieldType + ";");
        }

        sb.AppendLine();
        //声明初始化组件接口
        sb.AppendLine("\t\tpublic void InitComponent(WindowBase target)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\t//组件查找");
        //根据查找路径字典 和字段数据列表生成组件查找代码
        foreach (var item in objFindPathDic)
        {
            EditorObjectData itemData = GetEditorObjectData(item.Key);
            string relFieldName = itemData.fieldName.ToLower() + itemData.fieldType;

            if (string.Equals("GameObject", itemData.fieldType))
            {
                sb.AppendLine(
                    $"\t\t\t{relFieldName} = target.Transform.Find(\"{item.Value}\").gameObject;");
            }
            else if (string.Equals("Transform", itemData.fieldType))
            {
                sb.AppendLine(
                    $"\t\t\t{relFieldName} = target.Transform.Find(\"{item.Value}\").transform;");
            }
            else
            {
                sb.AppendLine(
                    $"\t\t\t{relFieldName} = target.Transform.Find(\"{item.Value}\").GetComponent<{itemData.fieldType}>();");
            }
        }

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
}
