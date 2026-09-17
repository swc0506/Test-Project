using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FixIntPhysics;
using FixMath;

public class MonsterLogic : LogicActor
{
    public int MonsterId { get; private set; }

    public MonsterLogic(int monsterid,RenderObject renderObject,FixIntBoxCollider boxCollider,FixIntVector3 logicPos)
    {
        MonsterId = monsterid;
        RenderObj = renderObject;
        Collider = boxCollider;
        LogicPos = logicPos;
        ObjectType = LogicObjectType.Monster;
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
}
