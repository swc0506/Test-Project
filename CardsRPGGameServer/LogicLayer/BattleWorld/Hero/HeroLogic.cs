using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    StopAnim,
    RePlayAnim
}

public class HeroLogic : LogicObject
{
    protected VInt hp;
    protected VInt atk;
    protected VInt def;
    protected VInt agl;
    protected VInt rage;

    public int Id => HeroData.id;
    public VInt Hp => hp;
    public VInt MaxHp { get; protected set; }
    public VInt Atk => atk + addAtk;
    public VInt Def => def + addDef;
    public VInt addDef; // 增益的防御值
    public VInt addAtk; // 增益的攻击值
    public VInt Agl => agl;
    public VInt Rage => rage;
    public VInt MaxRage { get; protected set; }

    public HeroData HeroData { get; private set; }
    public HeroTeamEnum TeamEnum { get; private set; }

#if RENDER_LOGIC
    public HeroRender HeroRender { get; private set; }
#endif

    public List<BuffLogic> haveBuffList = new List<BuffLogic>(); //已经拥有的列表

    public HeroLogic(HeroData data, HeroTeamEnum heroTeam)
    {
        HeroData = data;
        TeamEnum = heroTeam;
        hp = data.hp;
        MaxHp = data.hp;
        atk = data.atk;
        def = data.def;
        agl = data.agl;
        MaxRage = data.maxRage;
        rage = 0;
    }

    public override void OnCreate()
    {
        base.OnCreate();
#if RENDER_LOGIC
        HeroRender = (HeroRender)RednerObj;
        Debugger.Log("HeroName:" + RednerObj.gameObject.name);
#endif
        UpdateAnger(rage);
    }

    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        OnActionEndListener = null;
        ClearBuff();
    }

    public override void BeginAction()
    {
        base.BeginAction();
        if (objectState == LogicObjectState.Dead || IsBeControl())
        {
            EndAction();
            return;
        }

        //判断英雄怒气值是否大于100，释放技能
        bool isNormalAttack = Rage < MaxRage;
        if (Rage > MaxRage)
        {
            rage = 0;
        }

        int skillId = isNormalAttack ? HeroData.skillidArr[0] : HeroData.skillidArr[1];
        SkillManager.Instance.ReleaseSkill(skillId, this, isNormalAttack);
        UpdateAnger(0);
    }

    public override void EndAction()
    {
        base.EndAction();
        OnActionEndListener?.Invoke();
    }

    public override void RoundStarEvent(int round)
    {
        base.RoundStarEvent(round);
        for (var index = haveBuffList.Count - 1; index >= 0; index--)
        {
            var buffLogic = haveBuffList[index];
            buffLogic.RoundStarEvent(round);
        }
    }

    public override void RoundEndEvent()
    {
        base.RoundEndEvent();
        for (var index = haveBuffList.Count - 1; index >= 0; index--)
        {
            if (haveBuffList.Count > 0)
            {
                var buffLogic = haveBuffList[index];
                buffLogic.RoundEndEvent();
            }
        }
    }

    public void PlayAnim(string animName)
    {
#if RENDER_LOGIC
        HeroRender.PlayAnim(animName);
#endif
    }

    public void SetAnimState(AnimState state)
    {
#if RENDER_LOGIC
        HeroRender.SetAnimState(state);
#endif
    }

    public void UpdateAnger(VInt anger)
    {
        if (Rage >= MaxRage)
        {
            rage = MaxRage;
        }

        rage += anger;
#if RENDER_LOGIC
        //计算怒气比率
        float rate = (float)(rage / MaxRage).RawFloat;
        HeroRender.UpdateAnger_HUD(rate);
#endif
    }

    public void TryClearRage()
    {
        if (rage >= MaxRage)
        {
            rage = 0;
        }
    }

    public void DamageHp(VInt damage, BuffConfig buffConfig = null)
    {
        if (damage == 0)
        {
#if RENDER_LOGIC
            float percent = hp.RawFloat / MaxHp.RawFloat;
            HeroRender.UpdateHp_HUD(damage.RawInt, percent, buffConfig);
#endif
            return;
        }

        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            HeroDead();
            return;
        }
        else
        {
            if (damage > 0)
                PlayAnim("OnHit");
        }
#if RENDER_LOGIC
        float hpPercent = hp.RawFloat / MaxHp.RawFloat;
        HeroRender.UpdateHp_HUD(damage.RawInt, hpPercent, buffConfig);
#endif
    }

    public void BuffDamage(VInt damage, BuffConfig buffConfig)
    {
        Debugger.Log("BuffDamage damage:" + damage);
        DamageHp(damage, buffConfig);
    }

    public bool IsBeControl()
    {
        foreach (var buffLogic in haveBuffList)
        {
            if (buffLogic.BuffConfig.buffType == BuffType.Control)
            {
                return true;
            }
        }

        return false;
    }

    public void AddBuff(BuffLogic buff)
    {
        int buffId = buff.BuffId;
        if (buff.BuffConfig.maxStackingNum > 0)
        {
            int buffCount = GetBuffCount(buffId);
            if (buffCount >= buff.BuffConfig.maxStackingNum)
            {
                Debugger.Log("buff " + buffId + buff.BuffConfig.buffName + " 已达到最大叠加层数");
                return;
            }

            haveBuffList.Add(buff);
            if (buff.BuffConfig.buffState == BuffState.DefAdd)
            {
                addDef += BattleRule.CalAddDef(buff.BuffConfig.damagePercentage, this).RawInt;
            }
            else if (buff.BuffConfig.buffState == BuffState.AtkAdd)
            {
                addAtk += BattleRule.CalAddAtk(buff.BuffConfig.damagePercentage, this).RawInt;
            }
        }
        else
        {
            haveBuffList.Add(buff);
        }
        
#if  RENDER_LOGIC
        if (buff.BuffConfig.buffType != BuffType.DamageBuff)
        {
            HeroRender.AddBuffIcon(buff.BuffConfig);
        }
#endif
    }

    public int GetBuffCount(int buffId)
    {
        int buffCount = 0;
        for (int i = 0; i < haveBuffList.Count; i++)
        {
            if (haveBuffList[i].BuffId == buffId)
            {
                buffCount++;
            }
        }

        return buffCount;
    }

    public void RemoveBuff(BuffLogic buff)
    {
        if (haveBuffList.Contains(buff))
        {
            if (buff.BuffConfig.buffState == BuffState.DefAdd)
            {
                addDef -= BattleRule.CalAddDef(buff.BuffConfig.damagePercentage, this).RawInt;
            }
            else if (buff.BuffConfig.buffState == BuffState.AtkAdd)
            {
                addAtk -= BattleRule.CalAddAtk(buff.BuffConfig.damagePercentage, this).RawInt;
            }

            haveBuffList.Remove(buff);
        }
        
#if  RENDER_LOGIC
        if (buff.BuffConfig.buffType != BuffType.DamageBuff)
        {
            HeroRender.RemoveBuffIcon(buff.BuffConfig.buffIcon);
        }
#endif
    }

    public void RefreshBuffDuration(int buffId)
    {
        for (int i = 0; i < haveBuffList.Count; i++)
        {
            if (haveBuffList[i].BuffId == buffId)
            {
                haveBuffList[i].RefreshBuffDuration();
            }
        }
    }

    public void ClearBuff()
    {
        for (int i = haveBuffList.Count - 1; i >= 0; i--)
        {
            haveBuffList[i].OnDestroy();
        }
    }

    public void HeroDead()
    {
        objectState = LogicObjectState.Dead;
#if RENDER_LOGIC
        HeroRender.HeroDeath();
        SetAnimState(AnimState.RePlayAnim);
#endif
        ClearBuff();
    }
}