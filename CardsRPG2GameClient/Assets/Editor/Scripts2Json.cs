using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
public class Scripts2Json : Editor
{
    [MenuItem("Tools/SKillObjectToJson")]
    public static void SKillObjectToJson()
    {
        string path = Application.dataPath + "/GameData/BattleWorld/CfgData/SkillConfig/";
        string[] paths = System.IO.Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        List<SkillConfig> skillJsonlist = new List<SkillConfig>();
        for (int i = 0; i < paths.Length; i++)
        {
            if (paths[i].EndsWith(".meta"))
            {
                continue;
            }
            string skillid = Path.GetFileNameWithoutExtension(paths[i]);
            SkillConfig skillConfig =AssetDatabase.LoadAssetAtPath<SkillConfig>("Assets/GameData/BattleWorld/CfgData/SkillConfig/"+skillid+".asset");
            // SkillConfig skillConfig = Resources.Load<SkillConfig>("Skill/" + skillid);
            skillJsonlist.Add(skillConfig);
        }
        string skillJson = JsonConvert.SerializeObject(skillJsonlist, Formatting.Indented);
        Debugger.Log("json:" + skillJson);

        StreamWriter writer = File.CreateText(Application.dataPath + "/Scripts/ShareLogic/LogicalLayer/Config/SkillJsonCfg.txt");
        writer.Write(skillJson);
        writer.Flush();
        writer.Dispose();
        writer.Close();
        AssetDatabase.Refresh();

  
    }


    [MenuItem("Tools/BuffObjectToJson")]
    public static void BuffObjectToJson()
    {
        string path = Application.dataPath + "/GameData/BattleWorld/CfgData/BuffConfig/";
        string[] paths = System.IO.Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        List<BuffConfig> skillJsonlist = new List<BuffConfig>();
        for (int i = 0; i < paths.Length; i++)
        {
            if (paths[i].EndsWith(".meta"))
            {
                continue;
            }
            string buffid = Path.GetFileNameWithoutExtension(paths[i]);
            BuffConfig buffConfig = AssetDatabase.LoadAssetAtPath<BuffConfig>("Assets/GameData/BattleWorld/CfgData/BuffConfig/"+buffid+".asset");
            skillJsonlist.Add(buffConfig);
        }
        string skillJson = JsonConvert.SerializeObject(skillJsonlist, Formatting.Indented);
        Debugger.Log("json:" + skillJson);

        StreamWriter writer = File.CreateText(Application.dataPath + "/Scripts/ShareLogic/LogicalLayer/Config/BuffJsonCfg.txt");
        writer.Write(skillJson);
        writer.Flush();
        writer.Dispose();
        writer.Close();
        AssetDatabase.Refresh();
    }
}