using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillState
{
    None,
    ShakeBefore,
    ShakeAfter,
}

public class Skill
{
    public int SkillId { get; private set; }
    public SkillState SkillState { get; private set; }
    private HeroLogic mSkillOwner;
    private bool mIsNormalAtk;
    
    public Skill(int skillId, LogicObject skillOwner, bool isNormalAtk)
    {
        SkillId = skillId;
        mSkillOwner = (HeroLogic)skillOwner;
        mIsNormalAtk = isNormalAtk;
    }
    
    /// <summary>
    /// 释放技能
    /// </summary>
    public void ReleaseSkill()
    {
        SkillShakeBefore();
        MoveToTarget();
        PlaySkillAnim();
    }
    
    /// <summary>
    /// 技能前摇
    /// </summary>
    public void SkillShakeBefore()
    {
        SkillState = SkillState.ShakeBefore;
    }
    
    public void PlaySkillAnim()
    {
        
    }
    
    /// <summary>
    /// 移动到目标位置
    /// </summary>
    public void MoveToTarget()
    {
        
    }

    /// <summary>
    /// 技能触发
    /// </summary>
    public void SkillTrigger()
    {
        
    }

    /// <summary>
    /// 创建技能特效
    /// </summary>
    public void CreateSkillEffect()
    {
        
    }
    
    public void CauseDamage()
    {
        
    }

    public void AdditionBuff()
    {
        
    }
    
    public void SkillShakeAfter()
    {
        
    }

    public void MoveToSet()
    {
        
    }
    
    public void SkillEnd()
    {
        
    }
}
