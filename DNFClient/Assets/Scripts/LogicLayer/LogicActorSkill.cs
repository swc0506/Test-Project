using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZMGC.Battle;
using FixMath;
public partial class LogicActor
{
    /// <summary>
    /// 技能系统
    /// </summary>
    private SkillSystem mSkillSystem;
    /// <summary>
    /// 普通攻击技能id数组
    /// </summary>
    private int[] mNormalSkillidArr = new int[] { 1001, 1002, 1003 };

    private int[] mSkillidArr;

    /// <summary>
    /// 正在释放技能的列表
    /// </summary>
    public List<Skill> releaseingSkillList = new List<Skill>();
    /// <summary>
    /// 当前普通攻击连招索引
    /// </summary>
    private int mCurNormalComboIndex = 0;
    /// <summary>
    /// 当前自身身上的所有buff
    /// </summary>
    private List<Buff> mBuffList = new List<Buff>();
    /// <summary>
    /// 初始化技能列表
    /// </summary>
    public void InitActorSkill()
    {
        //HeroDataMgr heroData = BattleWorld.GetExitsDataMgr<HeroDataMgr>();
        //mNormalSkillidArr = heroData.GetHeroNormalSkilidArr(1001);
        //mSkillidArr = heroData.GetHeroSkillIdArr(1001);
        mSkillSystem = new SkillSystem(this);
        mSkillSystem.InitSKills(mNormalSkillidArr);
        mSkillSystem.InitSKills(mSkillidArr);
    }
    /// <summary>
    /// 释放普通攻击
    /// </summary>
    public void ReleaseNormalAttack()
    {
        ReleaseSKill(mNormalSkillidArr[mCurNormalComboIndex]);
    }
    /// <summary>
    /// 释放对应的技能
    /// </summary>
    /// <param name="skillid"></param>
    public void ReleaseSKill(int skillid,FixIntVector3 guidePos=default(FixIntVector3), Action<bool> releaseSkillCallBack = null)
    {
        Skill skill = mSkillSystem.ReleaseSkill(skillid, guidePos,   OnSkillReleaseAfter, (skill)=> {
            if (skill.SKillCfg.skillType== SKillType.StockPile)
            {
                releaseSkillCallBack?.Invoke(true);
            }
            OnSkillReleaseEnd(skill);
        });
        //不为null 说明技能释放成功
        if (skill != null)
        {
            releaseingSkillList.Add(skill);
            if (!IsNormalAttackSkill(skill.skillid))
            {
                mCurNormalComboIndex = 0;
            }
            ActionSate = LogicObjectActionState.ReleasingSkill;
            if (skill.SKillCfg.skillType!= SKillType.StockPile)
            {
                releaseSkillCallBack?.Invoke(true);
            }
        }
        else
        {
            releaseSkillCallBack?.Invoke(false);
        }
    }
    /// <summary>
    /// 触发蓄力技能释放
    /// </summary>
    /// <param name="skillid"></param>
    public void TriggerStockPileSkill(int skillid)
    {
        mSkillSystem.TriggerStockPileSkill(skillid);
    }
    /// <summary>
    /// 是否为普通攻击技能
    /// </summary>
    /// <param name="skillid">校验的技能id</param>
    /// <returns></returns>
    public bool IsNormalAttackSkill(int skillid)
    {
        foreach (var item in mNormalSkillidArr)
        {
            if (skillid == item)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 技能释放后回调
    /// </summary>
    /// <param name="skill"></param>
    public void OnSkillReleaseAfter(Skill skill)
    {
        if (!IsNormalAttackSkill(skill.skillid))
        {
            mCurNormalComboIndex = 0;
        }
        else
        {
            mCurNormalComboIndex++;
            //如果当前普通连招索引已经大于等于普通技能数组长度，则重置为0
            if (mCurNormalComboIndex >= mNormalSkillidArr.Length || skill.skillid == mNormalSkillidArr[mNormalSkillidArr.Length - 1])
            {
                mCurNormalComboIndex = 0;
            }
        }
    }
    /// <summary>
    /// 技能释放结束
    /// </summary>
    /// <param name="skill"></param>
    public void OnSkillReleaseEnd(Skill skill)
    {
        releaseingSkillList.Remove(skill);
        if (releaseingSkillList.Count == 0)
        {
            ActionSate = LogicObjectActionState.Idle;
            mCurNormalComboIndex = 0;
        }
    }

    public Skill GetSKill(int skillid)
    {
        return mSkillSystem.GetSKill(skillid);
    }
    /// <summary>
    /// 逻辑帧更新技能接口
    /// </summary>
    public void OnLogicFrameUpdateSkill()
    {
        mSkillSystem.OnLogicFrameUpdate();
    }
    /// <summary>
    /// 添加一个buff
    /// </summary>
    /// <param name="buff"></param>
    public void AddBuff(Buff buff)
    {
        mBuffList.Add(buff);
    }
    /// <summary>
    /// 移除一个buff
    /// </summary>
    /// <param name="buff"></param>
    public void RemoveBuff(Buff buff)
    {
        if (mBuffList.Contains(buff))
        {
            mBuffList.Remove(buff);
        }
        if (ObjectState == LogicObjectState.Death)
        {
            return;
        }

        if (mBuffList.Count == 0 && RenderObj.GetCurAnimName() != AnimationName.Anim_Getup)
        {
            PlayAnim(AnimationName.Anim_Idle);
            ActionSate = LogicObjectActionState.Idle;
        }
    }
}
