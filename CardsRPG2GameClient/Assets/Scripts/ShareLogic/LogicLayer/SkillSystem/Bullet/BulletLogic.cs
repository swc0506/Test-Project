using System;
using System.Collections;
using System.Collections.Generic;
using LogicLayer;
using UnityEngine;

public class BulletLogic : BulletBase
{
    public BulletLogic(LogicObject attacker, LogicObject target, VInt fightTime, Action onHitComplete) : base(attacker,
        target, fightTime, onHitComplete)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
        MoveToAction action = new MoveToAction(this, mAttackTarget.LogicPosition, mFightTime, BulletMoveComplete);
        ActionManager.Instance.RunAction(action);
    }
    
    public void BulletMoveComplete()
    {
        onHitComplete?.Invoke();
        BulletManager.Instance.RemoveBullet(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
#if RENDER_LOGIC
        RednerObj.OnRelease();
#endif
    }
}