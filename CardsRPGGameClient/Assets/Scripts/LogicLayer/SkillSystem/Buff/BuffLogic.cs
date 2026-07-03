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
    protected LogicObject attackTarget; //buff攻击目标
    
    public BuffLogic(int buffId, LogicObject owner, LogicObject attacker)
    {
        BuffId = buffId;
        this.buffOwner = owner;
        this.attackTarget = attacker;
        ownerHero = owner as HeroLogic;
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
                case BuffTriggerType.OneDamageRealTime:// 单次伤害触发
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
                        
                    }
                    break;
                case BuffTriggerType.MultisegmentDamageRealTime:// 多段伤害触发
                    break;
                case BuffTriggerType.DamageRoundStart:// 回合开始触发
                    break;
                case BuffTriggerType.DamageRoundEnd:// 回合结束触发
                    break;
            }
        }
    }
    
    public void TriggerBuff()
    {
        if (BuffConfig.damageType != BuffDamageType.None)
        {
            VInt damage = BattleRule.CalBuffDamage(BuffConfig, ownerHero, attackTarget as HeroLogic);
            HeroLogic attackTargetHero = attackTarget as HeroLogic;
            attackTargetHero.BuffDamage(damage, BuffConfig);
        }
    }

    public void AddBuffAndEffect()
    {
        
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
