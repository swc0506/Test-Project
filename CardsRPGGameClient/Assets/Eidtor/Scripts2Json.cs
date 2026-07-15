using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class Scripts2Json : Editor
{
    [MenuItem("Tools/Skill2Json")]
    public static void SkillObjectToJson()
    {
        Debugger.Log("SkillObjectToJson");
        string path = Application.dataPath + "/Resources/Skill";
        string[] paths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        List<SkillConfig> skillJsonList = new List<SkillConfig>();
        for (int i = 0; i < paths.Length; i++)
        {
            if (paths[i].EndsWith(".meta"))
            {
                continue;
            }

            string skillId = Path.GetFileNameWithoutExtension(paths[i]);
            SkillConfig skillConfig = Resources.Load<SkillConfig>("Skill/" + skillId);
            skillJsonList.Add(skillConfig);
        }

        string skillJson = JsonConvert.SerializeObject(skillJsonList, Formatting.Indented);
        Debugger.Log("json:" + skillJson);

        StreamWriter writer = File.CreateText(Application.dataPath + "/Scripts/LogicLayer/Config/SkillJsonConfig.txt");
        writer.WriteLine(skillJson);
        writer.Flush();
        writer.Dispose();
        writer.Close();
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Tools/Buff2Json")]
    public static void BuffObjectToJson()
    {
        Debugger.Log("BuffObjectToJson");
        string path = Application.dataPath + "/Resources/Buff";
        string[] paths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        List<BuffConfig> buffJsonList = new List<BuffConfig>();
        for (int i = 0; i < paths.Length; i++)
        {
            if (paths[i].EndsWith(".meta"))
            {
                continue;
            }
            string buffId = Path.GetFileNameWithoutExtension(paths[i]);
            BuffConfig buffConfig = Resources.Load<BuffConfig>("Buff/" + buffId);
            buffJsonList.Add(buffConfig);
        }
        string buffJson = JsonConvert.SerializeObject(buffJsonList, Formatting.Indented);
        Debugger.Log("json:" + buffJson);

        StreamWriter writer = File.CreateText(Application.dataPath + "/Scripts/LogicLayer/Config/BuffJsonConfig.txt");
        writer.Write(buffJson);
        writer.Flush();
        writer.Dispose();
        writer.Close();
        AssetDatabase.Refresh();
    }
}
