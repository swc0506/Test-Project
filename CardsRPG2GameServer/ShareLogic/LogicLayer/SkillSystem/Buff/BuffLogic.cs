using System;
using System.Collections;
using System.Collections.Generic;
using LogicLayer;
using UnityEngine;

public class BuffLogic : LogicObject
{
    public BuffConfig BuffConfig { get; set; }
    public int BuffId { get; private set; }
    protected LogicObject buffOwner; //buff释放者
    public HeroLogic ownerHero; //buff释放者
    public HeroLogic targetHero; //buff拥有者
    protected LogicObject attackTarget; //buff攻击目标

    private int mCurAccTime; //当前buff累计生效时间
    private int mCurRealTime; //当前buff真实生效时间
    private int mCurBuffSurvivalRound; //当前buff存活回合数

    public BuffLogic(int buffId, LogicObject owner, LogicObject attacker)
    {
        BuffId = buffId;
        this.buffOwner = owner;
        this.attackTarget = attacker;
        ownerHero = owner as HeroLogic;
        targetHero = attacker as HeroLogic;
    }

    public override void OnCreate()
    {
        base.OnCreate();
        objectState = LogicObjectState.Survival;
        BuffConfig = SkillConfigCenter.LoadBuffConfig(BuffId);
#if CLIENT_LOGIC

#else
        OnLogicFrameUpdate();
        if (objectState == LogicObjectState.Dead)
        {
            OnDestroy();
            BuffManager.Instance.RemoveBuff(this);
        }
#endif
    }

    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
        if (objectState == LogicObjectState.Survival)
        {
            switch (BuffConfig.triggerType)
            {
                case BuffTriggerType.OneDamageRealTime: // 单次伤害触发
                    if (BuffConfig.buffDurationTimeMs == 0 && BuffConfig.buffTriggerIntervalMs == 0)
                    {
                        TriggerBuff();
                        AddBuffAndEffect();
                        //如果是多回合伤害，需要把buff状态设置为等待触发伤害
                        if (BuffConfig.buffDurationRound == 0)
                            objectState = LogicObjectState.Dead;
                        else
                            objectState = LogicObjectState.SurvivalWaiting;
                    }
                    else
                    {
#if CLIENT_LOGIC
                        
                        //延时buff
                        mCurRealTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
                        if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                        {
                            TriggerBuff();
                            AddBuffAndEffect();
                            mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                        }

                        mCurAccTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
                        {
                            objectState = LogicObjectState.Dead;
                            break;
                        }
#else
                        while (mCurAccTime < BuffConfig.buffDurationTimeMs)
                        {
                            mCurRealTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
                            if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                            {
                                TriggerBuff();
                                AddBuffAndEffect();
                                mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                            }
                            mCurAccTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
                        }
                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
                        {
                            objectState = LogicObjectState.Dead;
                            break;
                        }
#endif
                    }
                    break;
                case BuffTriggerType.MultisegmentDamageRealTime: // 多段伤害触发
#if CLIENT_LOGIC
                    if (BuffConfig.buffDurationTimeMs > 0 && BuffConfig.buffTriggerIntervalMs > 0)
                    {
                        mCurRealTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
                        if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                        {
                            TriggerBuff();
                            AddBuffAndEffect();
                            mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                        }

                        mCurAccTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
                        {
                            objectState = LogicObjectState.Dead;
                            break;
                        }
                    }
#else
                    if (BuffConfig.buffDurationTimeMs > 0 && BuffConfig.buffTriggerIntervalMs > 0)
                    {
                        while (mCurAccTime < BuffConfig.buffDurationTimeMs)
                        { 
                            mCurRealTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
                            if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                            {
                                TriggerBuff();
                                AddBuffAndEffect();
                                mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                            }
                            mCurAccTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
                        }
                        objectState = LogicObjectState.Dead;
                    }
#endif
                    break;
                case BuffTriggerType.DamageRoundStart: // 回合开始触发
                    AddBuffAndEffect();
                    objectState = LogicObjectState.SurvivalWaiting;
                    break;
                case BuffTriggerType.DamageRoundEnd: // 回合结束触发
                    AddBuffAndEffect();
                    objectState = LogicObjectState.SurvivalWaiting;
                    break;
            }
        }
    }

    public override void RoundStarEvent(int round)
    {
        base.RoundStarEvent(round);
    }

    public override void RoundEndEvent()
    {
        base.RoundEndEvent();

        if (objectState == LogicObjectState.SurvivalWaiting && BuffConfig.triggerType == BuffTriggerType.DamageRoundEnd)
        {
            TriggerBuff();
        }

        mCurBuffSurvivalRound++;
        if (objectState == LogicObjectState.Survival || objectState == LogicObjectState.SurvivalWaiting)
        {
            if (BuffConfig.buffDurationRound > 0 && mCurBuffSurvivalRound >= BuffConfig.buffDurationRound)
            {
                targetHero.SetAnimState(AnimState.RePlayAnim);
                objectState = LogicObjectState.Dead;
                OnDestroy();
            }
        }
    }

    private void TriggerBuff()
    {
        if (BuffConfig.damageType != BuffDamageType.None)
        {
            VInt damage = BattleRule.CalBuffDamage(BuffConfig, ownerHero, attackTarget as HeroLogic);
            HeroLogic attackTargetHero = attackTarget as HeroLogic;
            attackTargetHero.BuffDamage(damage, BuffConfig);
        }
        else
        {
            HeroLogic attackTargetHero = attackTarget as HeroLogic;
            attackTargetHero.BuffDamage(0, BuffConfig);
        }
    }

    private void AddBuffAndEffect()
    {
        bool isTrigger = BuffConfig.buffTriggerProbability == 100;
        //处理概率性buff
        if (BuffConfig.buffTriggerProbability is > 0 and < 100)
        {
            int result = LogicRandom.Instance.Range(0, 100);
            if (result < BuffConfig.buffTriggerProbability)
                isTrigger = true;
        }

        if (isTrigger)
        {
            int addBuffCount = targetHero.GetBuffCount(BuffId);

            if (!string.IsNullOrEmpty(BuffConfig.buffEffect))
            {
#if CLIENT_LOGIC
                RednerObj = ResourcesManager.Instance.LoadObject<RenderObject>(AssetPathConfig.BUFF_EFFECT +
                                                                               BuffConfig.buffEffect);
                SetRenderObject(RednerObj);
                RednerObj.SetLogicObject(attackTarget);
                Debugger.Log("创建buffEffect：" + BuffConfig.buffEffect);
#endif
            }

            // 如果buff已经存在，则刷新buff持续时间
            if (addBuffCount != 0)
            {
                targetHero.RefreshBuffDuration(BuffId);
            }

            if (BuffConfig.buffType == BuffType.Control)
            {
                targetHero.SetAnimState(AnimState.StopAnim);
            }

            targetHero.AddBuff(this);
        }
    }

    public void RefreshBuffDuration()
    {
        mCurBuffSurvivalRound = 0;
    }

    public override void OnDestroy()
    {
        Debugger.Log("buff onDestroy buffId: " + BuffId);
        objectState = LogicObjectState.Dead;
        if (RednerObj != null)
            RednerObj?.OnRelease();

        BuffManager.Instance.DestroyBuff(this);
    }
}