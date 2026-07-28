using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicLayer;

public class SkillManager : Singleton<SkillManager>, LogicLayer.ILogicBehaviour
{
    public void OnCreate()
    {
    }

    public Skill ReleaseSkill(int skillId, HeroLogic skillOwner, bool isNormalAtk)
    {
        Skill skill = new Skill(skillId, skillOwner, isNormalAtk);
        skill.ReleaseSkill();
        return skill;
    }

    public void OnLogicFrameUpdate()
    {
    }

    public void OnDestroy()
    {
    }
}