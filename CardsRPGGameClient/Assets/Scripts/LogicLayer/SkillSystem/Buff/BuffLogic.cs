using System;
using System.Collections;
using System.Collections.Generic;
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
                    }

                    break;
                case BuffTriggerType.MultisegmentDamageRealTime: // 多段伤害触发
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

                    break;
                case BuffTriggerType.DamageRoundStart: // 回合开始触发
                    break;
                case BuffTriggerType.DamageRoundEnd: // 回合结束触发
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
        mCurBuffSurvivalRound++;
        if (objectState == LogicObjectState.Survival || objectState == LogicObjectState.SurvivalWaiting)
        {
            if (BuffConfig.buffDurationRound > 0 && mCurBuffSurvivalRound >= BuffConfig.buffDurationRound)
            {
                targetHero.SetAnimState(AnimState.RePlayAnim);
                objectState = LogicObjectState.Dead;
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
            if (!string.IsNullOrEmpty(BuffConfig.buffEffect))
            {
                RednerObj = ResourcesManager.Instance.LoadObject<RenderObject>(AssetPathConfig.BUFF_EFFECT +
                                                                               BuffConfig.buffEffect);
                SetRenderObject(RednerObj);
                RednerObj.SetLogicObject(attackTarget);
                Debugger.Log("创建buffEffect：" + BuffConfig.buffEffect);
            }

            if (BuffConfig.buffType == BuffType.Control)
            {
                targetHero.SetAnimState(AnimState.StopAnim);
            }
            targetHero.AddBuff(this);
        }
    }

    public override void OnDestroy()
    {
        Debugger.Log("buff onDestroy buffId: " + BuffId);
        objectState = LogicObjectState.Dead;
        RednerObj?.OnRelease();
        BuffManager.Instance.DestroyBuff(this);
    }
}