using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SkillConfigCenter
{
#if CLIENT_LOGIC

#else
    public static List<SkillConfig> SkillConfigList { get; private set; } = new List<SkillConfig>();
    public static List<BuffConfig> BuffConfigList { get; private set; } = new List<BuffConfig>();
#endif
 
    public static void Initialized()
    {
#if CLIENT_LOGIC

#else
        string skillCfgPath = AssetPathConfig.SERVER_CONFIG_PATH+ "SkillJsonConfig.txt";
        string skillJson = File.ReadAllText(skillCfgPath);
        SkillConfigList = JsonConvert.DeserializeObject<List<SkillConfig>>(skillJson);


        string buffCfgPath = AssetPathConfig.SERVER_CONFIG_PATH+ "BuffJsonConfig.txt";
        string buffJson = File.ReadAllText(buffCfgPath);
        BuffConfigList = JsonConvert.DeserializeObject<List<BuffConfig>>(buffJson);
#endif
    }
    public static BuffConfig LoadBuffConfig(int buffid)
    {
#if CLIENT_LOGIC
        return ResourcesManager.Instance.LoadAsset<BuffConfig>(AssetPathConfig.BUFF_CONFIG + buffid);
#else

        for (int i = 0; i < BuffConfigList.Count; i++)
        {
            if (BuffConfigList[i].buffId == buffid)
            {
                return BuffConfigList[i];
            }
        }
        return null;
#endif
    }
    public static SkillConfig LoadSkillConfig(int skillid)
    {
#if CLIENT_LOGIC
        return ResourcesManager.Instance.LoadAsset<SkillConfig>(AssetPathConfig.SKILL_CONFIG + skillid);
#else
        for (int i = 0; i < SkillConfigList.Count; i++)
        {
            if (SkillConfigList[i].skillId == skillid)
            {
                return SkillConfigList[i];
            }
        }
        return null;
#endif
    }
}
