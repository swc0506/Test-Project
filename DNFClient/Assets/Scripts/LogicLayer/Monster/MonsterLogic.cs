using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FixIntPhysics;
using FixMath;
using ZMGC.Battle;

public class MonsterLogic : LogicActor
{
    public int MonsterId { get; private set; }

    private FixInt attackRange = 1f; //攻击范围

    private FixInt chaseDinstance = 4; //追踪玩家的距离阈值

    private LogicActor mChaseTarget; //追踪目标

    public MonsterLogic(int monsterid, RenderObject renderObject, FixIntBoxCollider boxCollider, FixIntVector3 logicPos)
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

    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
        UpdateAIMove();
    }

    /// <summary>
    /// 更新怪物移动AI
    /// </summary>
    public void UpdateAIMove()
    {
        if (ObjectState == LogicObjectState.Death)
            return;
        //获取目标位置
        FixIntVector3 targetPos = mChaseTarget.LogicPos;
        //计算怪物与玩家之间的方向向量和距离
        FixIntVector3 directionToPlayer = (targetPos - LogicPos).normalized;
        FixInt dis = FixIntVector3.Distance(LogicPos, targetPos);
        if (dis <= attackRange)
        {
            //可以进行攻击了
            if (ActionSate == LogicObjectActionState.Idle || ActionSate == LogicObjectActionState.Move)
            {
                //PlayAnim(AnimationName.Anim_Idle);
                //正儿八经的攻击应该是通过技能系统释放对应怪物的技能
                PlayAnim(AnimationName.Anim_Gongji_01);
                ActionSate = LogicObjectActionState.ReleasingSkill;
            }
        }
        else if (dis <= chaseDinstance)
        {
            //表示玩家进入到怪物的追踪范围内了，怪物朝向玩家进行移动
            if (ActionSate == LogicObjectActionState.Idle || ActionSate == LogicObjectActionState.Move)
            {
                LogicPos += directionToPlayer * LogicMoveSpeed * LogicFrameConfig.LogicFrameInterval;
                LogicXAxis = directionToPlayer.x;
                PlayAnim(AnimationName.Anim_Walk);
                ActionSate = LogicObjectActionState.Move;
            }
        }
        else
        {
            if (ActionSate != LogicObjectActionState.Idle)
            {
                PlayAnim(AnimationName.Anim_Idle);
                ActionSate = LogicObjectActionState.Idle;
            }
        }
    }

    public override void OnHit(string effectHitObjPath, int survivalTimems, LogicObject source)
    {
        base.OnHit(effectHitObjPath, survivalTimems, source);
        this.LogicXAxis = -source.LogicXAxis;
    }

    public override void Floating(bool upfoating)
    {
        base.Floating(upfoating);
        string animName = upfoating ? AnimationName.Anim_Float_up : AnimationName.Anim_Float_down;
        PlayAnim(animName);

        ActionSate = LogicObjectActionState.Floating;
    }

    public override void TriggerGround()
    {
        base.TriggerGround();
        //处理怪物落地的逻辑
        if (ObjectState != LogicObjectState.Death)
        {
            PlayAnim(AnimationName.Anim_Getup);
            //当怪物从地面完全站起的时候，需要播放待机动画
            //通过逻辑帧延迟器延迟1秒触发逻辑  
            LogicTimerManager.Instance.DelayCall(0.5f, () =>
            {
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