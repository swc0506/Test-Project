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
        hp = 100;
        MaxHp = 100;
        atk = data.atk;
        def = data.def;
        agl = data.agl;
        rage = data.atkRange;
        MaxRage = data.maxRage;
    }

    public override void OnCreate()
    {
        base.OnCreate();
        HeroRender = (HeroRender)RednerObj;
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

    public void PlayAnim(string animName)
    {
#if RENDER_LOGIC
        HeroRender.PlayAnim(animName);
#endif
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
    }
}