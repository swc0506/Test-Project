using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


public class GeneratorWindowTool : Editor
{
    static Dictionary<string, string> methodDic = new Dictionary<string, string>();
    
    [MenuItem("GameObject/生成Window脚本", false, 0)]
    static void CreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;//获取到当前选择的物体
        if (obj == null)
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }
        
        //设置脚本生成路径
        if (!Directory.Exists(GeneratorConfig.WindowGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.WindowGeneratorPath);
        }
        
        string creatCs = CreateWindowCs(obj.name);
        string csPath = GeneratorConfig.WindowGeneratorPath + "/" + obj.name + ".cs";
        //生成脚本文件
        if (File.Exists(csPath))
        {
            File.Delete(csPath);
        }

        StreamWriter writer = File.CreateText(csPath);
        writer.Write(creatCs);
        writer.Close();
        AssetDatabase.Refresh();
    }

    public static string CreateWindowCs(string name)
    {
        string dataListJson = PlayerPrefs.GetString(GeneratorConfig.OBJATALIST_KEY);
        List<EditorObjectData> objDataList = JsonConvert.DeserializeObject<List<EditorObjectData>>(dataListJson);
        methodDic.Clear();
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("/*------------------------------");
        sb.AppendLine(" *Title:UI表现层脚本自动化代码生成工具");
        sb.AppendLine(" *Author:SWC");
        sb.AppendLine(" *Data:" + System.DateTime.Now);
        sb.AppendLine(" *Description: 表现层只负责页面逻辑与交互，不允许编写业务逻辑代码");
        sb.AppendLine(" *注意:生成不会覆盖原有代码，会进行新增操作");
        sb.AppendLine("------------------------------*/");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine("using ZMUIFrameWork;");
        sb.AppendLine("using ZMUIFrameWork.Scripts.Runtime.Base;");
        sb.AppendLine();
        
        //命名空间
        sb.AppendLine("namespace ZMUIFrameWork.Scripts.Window");
        sb.AppendLine("{");
        
        //生成类名
        sb.AppendLine($"\tpublic class {name} : WindowBase");
        sb.AppendLine("\t{");
        //生成字段
        sb.AppendLine($"\t\tpublic {name}UIComponent uiComp = new {name}UIComponent();");
        
        //生成声明周期函数 Awake
        sb.AppendLine("\t");
        sb.AppendLine("\t\t#region 声明周期函数");
        sb.AppendLine("\t\t//调用机制与Mono Awake一致");
        sb.AppendLine("\t\tpublic override void OnAwake()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tbase.OnAwake();");
        sb.AppendLine("\t\t\t uiComp.InitComponent(this);");
        sb.AppendLine("\t\t}");
        //OnShow
        sb.AppendLine("\t\t//物体显示时执行");
        sb.AppendLine("\t\tpublic override void OnShow()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tbase.OnShow();");
        sb.AppendLine("\t\t}");
        //OnHide
        sb.AppendLine("\t\t//物体隐藏时执行");
        sb.AppendLine("\t\tpublic override void OnHide()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tbase.OnHide();");
        sb.AppendLine("\t\t}");
        //OnDestroy
        sb.AppendLine("\t\t//物体销毁时执行");
        sb.AppendLine("\t\tpublic override void OnDestroy()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tbase.OnDestroy();");
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t\t#endregion");
        sb.AppendLine();
        
        //API Function
        sb.AppendLine("\t\t#region API Function");
        sb.AppendLine("\t\t");
        sb.AppendLine("\t\t#endregion");
        sb.AppendLine();
        
        //UI组件事件生成
        sb.AppendLine("\t\t#region UI组件事件");
        foreach (var item in objDataList)
        {
            string type = item.fieldType;
            string methodName = "On" + item.fieldName;
            string suffix = string.Empty;
            if (type.Contains("Button"))
            {
                suffix = "ButtonClick";
                CreateMethod(sb, ref methodDic, methodName + suffix);
            }
            else if (type.Contains("InputField"))
            {
                suffix = "InputChange";
                CreateMethod(sb, ref methodDic, methodName + suffix, "string text");
                suffix = "InputEnd";
                CreateMethod(sb, ref methodDic, methodName + suffix, "string text");
            }
            else if (type.Contains("Toggle"))
            {
                suffix = "ToggleChange";
                CreateMethod(sb, ref methodDic, methodName + suffix, "bool state,Toggle toggle");
            }
        }

        sb.AppendLine("\t\t#endregion");
        sb.AppendLine("\t}");
        sb.AppendLine("}");
        return sb.ToString();
    }
    
    /// <summary>
    /// 生成UI事件方法
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="methodDic"></param>
    /// <param name="methodName"></param>
    /// <param name="param"></param>
    private static void CreateMethod(StringBuilder sb, ref Dictionary<string, string> methodDic, string methodName, string param = "")
    {
        //声明UI组件事件
        sb.AppendLine($"\t\tpublic void {methodName}({param})");
        sb.AppendLine("\t\t{");
        if (methodName == "OnCloseButtonClick")
        {
            sb.AppendLine("\t\t\tHideWindow();");
        }
        sb.AppendLine("\t\t}");

        //存储UI组件事件 提供给后续新增代码使用
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"\t\tpublic void {methodName}({param})");
        builder.AppendLine("\t\t{");
        builder.AppendLine("\t\t");
        builder.AppendLine("\t\t}");
        methodDic.Add(methodName, builder.ToString());
    }
}