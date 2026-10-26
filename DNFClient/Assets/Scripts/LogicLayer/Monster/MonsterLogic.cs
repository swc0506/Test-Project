using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FixIntPhysics;
using FixMath;
using ZMGC.Battle;

public class MonsterLogic : LogicActor
{
    public int MonsterId { get; private set; }
    
    private FixInt attackRange = 1f;//攻击范围

    private FixInt chaseDinstance = 4;//追踪玩家的距离阈值

    private LogicActor mChaseTarget;//追踪目标

    public MonsterLogic(int monsterid,RenderObject renderObject,FixIntBoxCollider boxCollider,FixIntVector3 logicPos)
    {
        MonsterId = monsterid;
        RenderObj = renderObject;
        Collider = boxCollider;
        LogicPos = logicPos;
        ObjectType = LogicObjectType.Monster;
    }
    
    public override void OnCreate()
    {
        base.OnCreate();
        InitMonsterAttribute();
        mChaseTarget = BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>().HeroLogic;
        LogicMoveSpeed = 1;
    }
    
    public override void OnHit(GameObject effectHitObj, int survivalTimems, LogicObject source)
    {
        base.OnHit(effectHitObj, survivalTimems, source);
        LogicXAxis = - source.LogicXAxis;
    }

    public override void Floating(bool upfoating)
    {
        base.Floating(upfoating);
        string animName= upfoating ?AnimationName.Anim_Float_up : AnimationName.Anim_Float_down;
        PlayAnim(animName);

        ActionSate = LogicObjectActionState.Floating;
    }

    public override void TriggerGround()
    {
        base.TriggerGround();
        //处理怪物落地的逻辑
        if (ObjectState!= LogicObjectState.Death)
        {
            PlayAnim(AnimationName.Anim_Getup);
            //当怪物从地面完全站起的时候，需要播放待机动画
            //通过逻辑帧延迟器延迟1秒触发逻辑  
            LogicTimerManager.Instance.DelayCall(0.5f,()=> {
                PlayAnim(AnimationName.Anim_Idle);
                ActionSate = LogicObjectActionState.Idle;
            });
        }
        else
        {
            PlayAnim(AnimationName.Anim_Dead);
        }
    }
    
    private void InitMonsterAttribute()
    {
        MonsterCfg data = ConfigCenter.Instance.GetMonsterCfgById(MonsterId);
        if (data == null)
        {
            Debug.LogError(MonsterId + " MonsterCfg is Null! Return...");
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
        apMctRate = data.apMctRate;
        str = data.str;
        sta = data.sta;
        Int = data.Int;
        spi = data.spi;
        agl = data.agl;

        atkRange = data.atkRange;
        searchDisRange = data.searchDisRange;
        Debug.Log("InitMonsterAttribute Success..");
    }
}
