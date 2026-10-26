using FixMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogic : LogicActor
{
    /// <summary>
    /// 英雄id
    /// </summary>
    public int Heroid { get; private set; }

    public HeroLogic(int heroid, RenderObject renderObj)
    {
        Heroid = heroid;
        RenderObj = renderObj;
        ObjectType = LogicObjectType.Hero;
    }

    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
    }
    
    public override void OnCreate()
    {
        base.OnCreate();
        InitActorSkill(Heroid);
        InitHeroAttribute();
    }
    
    private void InitHeroAttribute()
    {
        HeroDataCfg data= ConfigCenter.Instance.GetHeroCfgById(Heroid);
        if (data==null)
        {
            Debug.LogError(Heroid+ " HeroDataCfg is Null! Return...");
            return;
        }
        hp = data.hp;
        mp = data.mp;
        ap = data.ap;
        ad = data.ad;
        adDef = data.adDef;
        apDef = data.apDef;
        pct = data.pct;
        mct = data.mct;
        adPctRate = data.adPctRate;
        apMctRate=data.apMctRate;
        str = data.str;
        sta = data.sta;
        Int = data.Int;
        spi = data.spi;
        agl = data.agl;
        Debug.Log("InitHeroAttribute Success..");
    }
}