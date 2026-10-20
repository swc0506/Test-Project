using FixMath;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;
public class LogicTimerManager : Singleton<LogicTimerManager>
{
    /// <summary>
    /// 计时器列表
    /// </summary>
    private List<LogicTimer> mTimerList = new List<LogicTimer>();

    /// <summary>
    /// 开始进行行动
    /// </summary>
    /// <param name="action"></param>
    public void DelayCall(FixInt delayTime,Action timerCallBack,int loopCount=1)
    {
        LogicTimer timer = new LogicTimer(delayTime,timerCallBack,loopCount);
        mTimerList.Add(timer);
    }
    /// <summary>
    /// 逻辑帧更新帧
    /// </summary>
    public void OnLogicFrameUpdate()
    {
        //移除已经完成的行动
        for (int i = mTimerList.Count-1; i >=0 ; i--)
        {
            LogicTimer timer = mTimerList[i];
            if (timer.TimerFinish)
            {
                RemoveTimer(timer);
            }
        }
        //更新逻辑帧
        foreach (var item in mTimerList)
        {
            item.OnLogicFrameUpdate();
        }
    }
    /// <summary>
    /// 移除对应的行动
    /// </summary>
    /// <param name="action"></param>
    public void RemoveTimer(LogicTimer timer)
    {
        mTimerList.Remove(timer);
    }
    /// <summary>
    /// 脚本资源释放
    /// </summary>
    public void OnDestroy()
    {
        mTimerList.Clear();
    }
}
