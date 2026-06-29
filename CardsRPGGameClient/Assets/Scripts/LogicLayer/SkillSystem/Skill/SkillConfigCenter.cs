using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillConfigCenter : MonoBehaviour
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
}
