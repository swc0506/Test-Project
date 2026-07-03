using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillConfigCenter
{
    public static SkillConfig LoadSkillConfig(int skillId)
    {
        SkillConfig skillConfig = Resources.Load<SkillConfig>(AssetPathConfig.SKILL_CONFIG + skillId);
        if (skillConfig == null)
        {
            Debugger.LogError("skillConfig is null skillId:" + skillId);
        }

        return skillConfig;
    }
    
    public static BuffConfig LoadBuffConfig(int buffId)
    {
        BuffConfig buffConfig = Resources.Load<BuffConfig>(AssetPathConfig.BUFF_CONFIG + buffId);
        if (buffConfig == null)
        {
            Debugger.LogError("buffConfig is null buffId:" + buffId);
        }

        return buffConfig;
    }
}
