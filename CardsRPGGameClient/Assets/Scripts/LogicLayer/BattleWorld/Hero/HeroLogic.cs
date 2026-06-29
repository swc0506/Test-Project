using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogic : LogicObject
{
    protected VInt hp;
    protected VInt atk;
    protected VInt def;
    protected VInt agl;
    protected VInt rage;

    public VInt Hp => hp;
    public VInt MaxHp { get; protected set; }
    public VInt Atk => atk;
    public VInt Def => def;
    public VInt Agl => agl;
    public VInt Rage => rage;
    public VInt MaxRage { get; protected set; }

    public HeroData HeroData { get; private set; }
    public HeroTeamEnum TeamEnum { get; private set; }
    public HeroRender HeroRender { get; private set; }

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
        HeroRender = (HeroRender)RednerObj;
        UpdateAnger(rage);
        Debugger.Log("HeroName:" + RednerObj.gameObject.name);
    }

    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void BeginAction()
    {
        base.BeginAction();
        if (objectState == LogicObjectState.Dead)
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

    public void PlayAnim(string animName)
    {
#if RENDER_LOGIC
        HeroRender.PlayAnim(animName);
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

    public void DamageHp(VInt damage)
    {
        if (damage == 0)
            return;

        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            HeroDead();
            return;
        }
        else
        {
            PlayAnim("OnHit");
        }
#if RENDER_LOGIC
        float hpPercent = hp.RawFloat / MaxHp.RawFloat;
        HeroRender.UpdateHp_HUD(damage.RawInt, hpPercent);
#endif
    }

    public void HeroDead()
    {
        objectState = LogicObjectState.Dead;
#if RENDER_LOGIC
        HeroRender.HeroDeath();
#endif
    }
}