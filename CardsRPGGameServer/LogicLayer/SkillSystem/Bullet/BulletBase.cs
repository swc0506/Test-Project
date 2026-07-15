using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : LogicObject
{
    protected LogicObject mAttacker;
    protected LogicObject mAttackTarget;
    protected VInt mFightTime;//子弹飞行时间
    protected Action onHitComplete;//子弹击中回调

    public BulletBase(LogicObject attacker, LogicObject target, VInt fightTime, Action onHitComplete)
    {
        mAttacker = attacker;
        mAttackTarget = target;
        mFightTime = fightTime;
        this.onHitComplete = onHitComplete;
    }
}
