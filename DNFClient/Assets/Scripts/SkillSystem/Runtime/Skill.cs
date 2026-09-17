using FixMath;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

public enum SkillState
{
    None,
    Befor,//技能前摇
    After,//技能后摇
    End,//技能结束
}

public partial class Skill
{

    public int skillid;
    /// <summary>
    /// 技能创建者
    /// </summary>
    public LogicActor mSkillCreater;
    /// <summary>
    /// 技能配置数据
    /// </summary>
    private SkillDataConfig mSkillData;

    public SkillConfig SKillCfg { get { return mSkillData.skillCfg; } }

    /// <summary>
    /// 释放技能后摇
    /// </summary>
    public Action<Skill> OnReleaseAfter;
    /// <summary>
    /// 释放技能结束回调
    /// </summary>
    public Action<Skill, bool> OnReleaseSkillEnd;
    /// <summary>
    /// 技能状态
    /// </summary>
    public SkillState skillState = SkillState.None;
    /// <summary>
    /// 当前逻辑帧
    /// </summary>
    private int mCurLogicFrame = 0;
    /// <summary>
    /// 当前累计运行时间
    /// </summary>
    private int mCurLogicFrameAccTime = 0;
    /// <summary>
    /// 是否自动匹配蓄力阶段
    /// </summary>
    private bool mAutoMacthStockStage;

    /// <summary>
    /// 技能引导位置
    /// </summary>
    public FixIntVector3 sKillGuidePos;
    /// <summary>
    /// 创建技能
    /// </summary>
    /// <param name="skillid">技能id</param>
    /// <param name="skillCreater">技能创建者</param>
    public Skill(int skillid, LogicActor skillCreater)
    {
        this.skillid = skillid;
        this.mSkillCreater = skillCreater;
        mSkillData = ZMAssetsFrame.LoadScriptableObject<SkillDataConfig>(AssetPathConfig.SKILL_DATA_PATH + skillid + ".asset");
    }
    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="releaseAfterCallBack">技能后摇</param>
    /// <param name="releaseSkillEnd">释放技能结束</param>
    public void ReleaseSKill(Action<Skill> releaseAfterCallBack, FixIntVector3 guidePos, Action<Skill, bool> releaseSkillEnd)
    {
        OnReleaseAfter = releaseAfterCallBack;
        OnReleaseSkillEnd = releaseSkillEnd;
        sKillGuidePos = guidePos;
        SkillStart();
        skillState = SkillState.Befor;
        PlayAnim();
    }
    /// <summary>
    /// 播放技能动画
    /// </summary>
    public void PlayAnim()
    {
        //播放角色动画
        mSkillCreater.PlayAnim(mSkillData.character.skillAnim);
    }
    /// <summary>
    /// 技能前摇
    /// </summary>
    public void SkillStart()
    {
        //开始释放技能时，初始化技能数据
        mCurLogicFrame = 0;
        mCurLogicFrameAccTime = 0;
        mAutoMacthStockStage = false;
        if (mSkillData.character.customLogicFame != 0)
            mSkillData.character.logicFrame = mSkillData.character.customLogicFame;
        OnBulletInit();
    }
    /// <summary>
    /// 技能后摇
    /// </summary>
    public void SkillAfter()
    {
        skillState = SkillState.After;
        OnReleaseAfter?.Invoke(this);
    }
    /// <summary>
    /// 技能释放结束
    /// </summary>
    public void SKillEnd()
    {
        skillState = SkillState.End;
        OnReleaseSkillEnd?.Invoke(this, mSkillData.skillCfg.ComobinationSkillid != 0);
        ReleaseAllEffect();
        OnBulletRelease();
        //检测是否有组合技能
        if (mSkillData.skillCfg.ComobinationSkillid != 0)
        {
            mSkillCreater.ReleaseSKill(mSkillData.skillCfg.ComobinationSkillid);
        }
    }

    /// <summary>
    /// 逻辑帧更新
    /// </summary>
    public void OnLogicFrameUpdate()
    {
        if (skillState == SkillState.None||skillState== SkillState.End)
        {
            return;
        }
        //计算累计运行时间
        mCurLogicFrameAccTime = mCurLogicFrame * LogicFrameConfig.LogicFrameIntervalms;

        //处理技能后摇
        if (skillState == SkillState.Befor && mCurLogicFrameAccTime >= mSkillData.skillCfg.skillShakeArfterMs&&mSkillData.skillCfg.skillType!= SKillType.StockPile)
        {
            SkillAfter();
        }

        //更新不同配置的逻辑帧，处理不同配置的逻辑

        //更新特效逻辑帧
        OnLogicFrameUpdateEffect();
        //更新伤害逻辑帧
        OnLogicFrameUpdateDamage();
        //更新行动逻辑帧
        OnLogicFrameUpdateAction();
        //更新音效逻辑帧
        OnLogicFrameUpdateAudio();
        //更新子弹逻辑帧
        OnLogicFrameUpdateBullet();
        //因为蓄力技能需要通过蓄力时间进行触发，所以和技能的结束帧无关
        if (mSkillData.skillCfg.skillType == SKillType.StockPile)
        {
            int stockDataCount = mSkillData.skillCfg.stockPileStageData.Count;
            if (stockDataCount > 0)
            {
                //1.处理手指按下立马抬起的一种情况
                if (mAutoMacthStockStage)
                {
                    //自动匹配第一阶段蓄力技能进行释放
                    StockPileStageData stockData = mSkillData.skillCfg.stockPileStageData[0];
                    if (mCurLogicFrameAccTime >= stockData.startTimeMs)
                    {
                        StockPileFinish(stockData);
                    }
                }
                else
                {
                    //2.处理超时蓄力的逻辑
                    StockPileStageData stockData = mSkillData.skillCfg.stockPileStageData[stockDataCount - 1];
                    //计算蓄力时间是否达到最大值，如果达到最大值就自动触发最大蓄力阶段技能(比如用户手指按着蓄力技能按钮不松)
                    if (mCurLogicFrameAccTime >= stockData.endTimeMs)
                    {
                        StockPileFinish(stockData);
                    }
                }
            }
        }
        else
        {
            //判断技能是否释放结束
            if (mCurLogicFrame == mSkillData.character.logicFrame)
            {
                SKillEnd();
            }
        }

        //逻辑帧自增
        mCurLogicFrame++;
    }
    /// <summary>
    /// 主动触发蓄力技能
    /// </summary>
    public void TriggerStockPileSkill()
    {
        //1.蓄力时间符合蓄力阶段配置中的某一个技能
        foreach (var item in mSkillData.skillCfg.stockPileStageData)
        {
            if (mCurLogicFrameAccTime>=item.startTimeMs&&mCurLogicFrameAccTime<=item.endTimeMs)
            {
                StockPileFinish(item);
                return;
            }
        }
        //2.蓄力时间过短，不符合任意蓄力里阶段技能，自动触发第一阶段蓄力技能
        mAutoMacthStockStage = true;
    }
    /// <summary>
    /// 蓄力技能完成
    /// </summary>
    /// <param name="stockData"></param>
    public void StockPileFinish(StockPileStageData stockData)
    {
        SKillEnd();
        if (stockData.skillid == 0)
        {
            Debug.LogError("蓄力技能释放失败，蓄力阶段技能id为0");
        }
        else
        {
            mSkillCreater.ReleaseSKill(stockData.skillid);
        }
    }
}
