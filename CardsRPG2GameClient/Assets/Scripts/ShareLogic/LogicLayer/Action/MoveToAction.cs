using System;
using System.Collections;
using System.Collections.Generic;
using LogicLayer;
using UnityEngine;

public class MoveToAction : ActionBase
{
    private LogicObject moveObj; //移动对象
    private VInt3 target; // 目标位置
    private VInt3 mOriginPos; // 起始位置
    private VInt timesMs; //移动时间
    private Action moveComplete; // 移动完成回调

    private VInt vintLerpTime = 0;

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    /// <param name="moveObj"></param>
    /// <param name="target"></param>
    /// <param name="timesMs">毫秒</param>
    /// <param name="moveComplete"></param>
    public MoveToAction(LogicObject moveObj, VInt3 target, VInt timesMs, Action moveComplete)
    {
        mOriginPos = moveObj.LogicPosition;
        this.moveObj = moveObj;
        this.target = target;
        this.timesMs = timesMs;
        this.moveComplete = moveComplete;
    }

    public override void OnCreate()
    {
        base.OnCreate();
    }

    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
#if CLIENT_LOGIC
        vintLerpTime += (VInt)LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
        VInt lerpValue = vintLerpTime / timesMs;

        moveObj.LogicPosition = VInt3.Lerp(mOriginPos, target, lerpValue.RawFloat);
        if (lerpValue > VInt.one)
        {
            actionComplete = true;
            moveComplete?.Invoke();
        }
#else
        moveComplete?.Invoke();
        actionComplete = true;
#endif
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}