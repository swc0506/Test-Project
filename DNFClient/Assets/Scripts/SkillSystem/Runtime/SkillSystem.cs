using FixMath;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能系统
/// </summary>
public class SkillSystem
{
    /// <summary>
    /// 技能系统创建者
    /// </summary>
    private LogicActor mSkillCreater;
    /// <summary>
    /// 技能id数组
    /// </summary>
    private List<Skill> mSkillArr = new List<Skill>();
    /// <summary>
    /// 当前正在释放中的技能
    /// </summary>
    private Skill mCurReleasingSkill;
    /// <summary>
    /// 技能组合技能id列表
    /// </summary>
    private List<int> mCombinationSkillIdList = new List<int>();
    public SkillSystem(LogicActor logicActor)
    {
        mSkillCreater = logicActor;
    }
    /// <summary>
    /// 初始化技能
    /// </summary>
    /// <param name="skillidArr">技能id数组</param>
    public void InitSKills(int[] skillidArr)//1000 1001 1002
    {
        foreach (var skillid in skillidArr)
        {
            Skill skill = new Skill(skillid, mSkillCreater);
            mSkillArr.Add(skill);
            if (skill.SKillCfg.ComobinationSkillid != 0)
            {
                //使用递归方式调用技能组合技所有技能的初始化
                InitSKills(new int[] { skill.SKillCfg.ComobinationSkillid });
            }
            //初始化所有蓄力技能
            if (skill.SKillCfg.stockPileStageData.Count>0)
            {
                foreach (var item in skill.SKillCfg.stockPileStageData)
                {
                    InitSKills(new int[] { item.skillId });
                }
            }
        }
        Debug.Log("技能初始化完成 技能个数：" + skillidArr.Length);
    }
    public Skill ReleaseSkill(int skillid, FixIntVector3 guidePos,  Action<Skill> releaseAfterCallBack, Action<Skill> releaseSkillEnd )
    {
        //如果当前的技能不为空，与当前的技能处于前摇或者刚开始释放的一个状态，这个时候是不允许释放其他技能的
        if (mCurReleasingSkill!=null&&(mCurReleasingSkill.skillState!= SkillState.End&&mCurReleasingSkill.skillState!= SkillState.After))
        {
            return null;
        }
        //组合技正在释放中，非组合技无法释放
        if (mCombinationSkillIdList.Count>0&&!mCombinationSkillIdList.Contains(skillid))
        {
            return null;
        }
        foreach (var skill in mSkillArr)
        {
            if (skill.skillid == skillid)
            {
                if (skill.skillState != SkillState.None && skill.skillState != SkillState.End)
                {
                    Debug.Log($"当前技能正在释放中 skillID{skill} .不允许在次释放！");
                    return null;
                }
                //判断当前技能是否有组合技
                if (skill.SKillCfg.ComobinationSkillid!=0)
                {
                    CacleteCombinationSkillIdList(skill.SKillCfg.ComobinationSkillid);
                }
                //释放技能
                skill.ReleaseSKill(releaseAfterCallBack , guidePos, (skill, combinationSkill) =>
                {
                    //技能释放完成回调
                    releaseSkillEnd?.Invoke(skill);
                    //如果当前技能是组合技能，处理技能组的逻辑
                    if (!combinationSkill)
                    {
                        //技能释放完成 比如说，组合技能的最后一段
                        mCurReleasingSkill = null;
                        if (skill.SKillCfg.ComobinationSkillid==0&&mCombinationSkillIdList.Count>0)
                        {
                            mCombinationSkillIdList.Clear();
                        }
                    }
                });
                mCurReleasingSkill = skill;
                return skill;
            }
        }
        Debug.LogError("SKillid :" + skillid + " 技能不存在，配置表中没有找到");
        return null;
    }
    /// <summary>
    /// 主动触发蓄力技能
    /// </summary>
    /// <param name="skillid"></param>
    public void TriggerStockPileSkill(int skillid)
    {
        //如果当前的技能不为空，与当前的技能处于前摇或者刚开始释放的一个状态，这个时候是不允许释放其他技能的
        if (mCurReleasingSkill != null && mCurReleasingSkill.skillid!=skillid)
        {
            return;
        }
        //判断当前是否有组合技正在释放中，如果有组合技正在释放中，是不允许释放其他技能的。
        //组合技正在释放中，非组合技无法释放
        if (mCombinationSkillIdList.Count > 0 && !mCombinationSkillIdList.Contains(skillid))
        {
            return;
        }
        Skill skill = GetSKill(skillid);
        if (skill != null)
        {
            skill.TriggerStockPileSkill();
        }
    }
    public Skill GetSKill(int skillid)
    {
        foreach (var item in mSkillArr)
        {
            if (item.skillid == skillid)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 计算组合技能id列表
    /// </summary>
    /// <param name="skillid"></param>
    public void CacleteCombinationSkillIdList(int skillid) //1000,1001 ,1002
    {
        if (skillid!=0)
        {
            int combinationSkillId = skillid;
            while (combinationSkillId!=0)
            {
                mCombinationSkillIdList.Add(combinationSkillId);
                combinationSkillId= GetSKill(combinationSkillId).SKillCfg.ComobinationSkillid;
            }

        }
        else
        {
            Debug.LogError("无效的技能id:"+skillid);
        }
    }
    public void OnLogicFrameUpdate()
    {
        foreach (var item in mSkillArr)
        {
            item.OnLogicFrameUpdate();
        }
    }
}
