using FixMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理演员对象移动逻辑脚本
/// </summary>
public partial class LogicActor
{
    private FixIntVector3 mInputMoveDir;
    /// <summary>
    /// 逻辑帧位置更新
    /// </summary>
    public void OnLogicFrameUpdateMove()
    {
        Collider?.UpdateColliderInfo(LogicPos, Collider.Size);
        if (ActionSate != LogicObjectActionState.Idle && ActionSate != LogicObjectActionState.Move)
        {
            return;
        }
        //计算逻辑位置
        LogicPos += mInputMoveDir* LogicMoveSpeed * (FixInt)LogicFrameConfig.LogicFrameInterval;

        //计算逻辑对象的朝向
        if (LogicDir!=mInputMoveDir)
        {
            LogicDir = mInputMoveDir;
        }
        //计算逻辑轴向
        if (LogicDir.x!=FixInt.Zero)
        {
            LogicXAxis = LogicDir.x > 0 ? 1 : -1;
        }
        Debug.Log("LogicPos:"+LogicPos);
     }

    public void InputLoigcFrameEvent(FixIntVector3 inputDir)
    {
        mInputMoveDir = inputDir;
    }
}
