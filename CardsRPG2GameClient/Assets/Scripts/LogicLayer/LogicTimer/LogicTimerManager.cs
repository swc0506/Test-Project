using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicTimerManager : Singleton<LogicTimerManager>, ILogicBehaviour
{
    private List<LogicTimer> mLogicTimers = new List<LogicTimer>();

    public void OnCreate()
    {
    }

    public void DelayCall(VInt delayTime, Action callback, int loop = 1)
    {
#if CLIENT_LOGIC
        LogicTimer logicTimer = new LogicTimer(delayTime, callback, loop);
        mLogicTimers.Add(logicTimer);
#else
        //服务端立即触发回调 无需延迟
        for (int i = 0; i < loop; i++)
        {
            callback?.Invoke();
        }
#endif
    }

    public void OnLogicFrameUpdate()
    {
        for (int i = mLogicTimers.Count - 1; i >= 0; i--)
        {
            if (mLogicTimers[i].workFinished)
            {
                mLogicTimers.RemoveAt(i);
            }
        }
        
        for (int i = 0; i < mLogicTimers.Count; i++)
        {
            mLogicTimers[i].OnLogicFrameUpdate();
        }
    }

    public void OnDestroy()
    {
        mLogicTimers.Clear();
    }
}