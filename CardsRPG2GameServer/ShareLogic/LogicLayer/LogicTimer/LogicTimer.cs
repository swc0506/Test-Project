using System;
using System.Collections;
using System.Collections.Generic;
using LogicLayer;
using UnityEngine;

public class LogicTimer : ILogicBehaviour
{
    public VInt delayTime;
    public int loopCount;
    public bool workFinished;
    public Action callback;

    private VInt curAccTime;//当前累积时间
    
    public LogicTimer(VInt delayTime, Action callback, int loop = 1)
    {
        this.delayTime = delayTime;
        this.loopCount = loop;
        this.callback = callback;
    }
    
    public void OnCreate()
    {
        
    }

    public void OnLogicFrameUpdate()
    {
        curAccTime += (VInt)LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS;
        if (curAccTime >= delayTime && loopCount > 0)
        {
            callback?.Invoke();
            curAccTime = 0;
            loopCount--;
            if (loopCount == 0)
            {
                workFinished = true;
            }
        }
    }

    public void OnDestroy()
    {
        
    }
}
