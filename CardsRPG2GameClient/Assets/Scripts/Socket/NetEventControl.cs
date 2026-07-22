using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网络事件派发中心
/// </summary>
public class NetEventControl
{
    /// <summary>
    /// 互斥对象
    /// </summary>
    private static object mMutex = new object();

    /// <summary>
    /// 委托对象
    /// </summary>
    public delegate void EventHandler(byte[] msgBytes);

    /// <summary>
    /// 事件派发字典
    /// </summary>
    private static Dictionary<Protocal, List<EventHandler>> mEventDic = new Dictionary<Protocal, List<EventHandler>>();

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="protocal"></param>
    /// <param name="handler"></param>
    public static void AddEvent(Protocal protocal, EventHandler handler)
    {
        lock (mMutex)
        {
            //如果该事件没有注册，就进行创建
            if (!mEventDic.ContainsKey(protocal))
            {
                mEventDic.Add(protocal, new List<EventHandler>());
            }

            //如果该事件没有注册，就进行添加
            if (!mEventDic[protocal].Contains(handler))
            {
                mEventDic[protocal].Add(handler);
            }
        }
    }
    
    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="protocal"></param>
    /// <param name="handler"></param>
    public static void RemoveEvent(Protocal protocal, EventHandler handler)
    {
        lock (mMutex)
        {
            // 如果该事件存在，就进行移除
            if (mEventDic.ContainsKey(protocal))
            {
                if (mEventDic[protocal].Contains(handler))
                {
                    mEventDic[protocal].Remove(handler);
                }
            }
        }
    }
    
    /// <summary>
    /// 派发事件
    /// </summary>
    /// <param name="protocal"></param>
    /// <param name="msgBytes"></param>
    public static void DispatchEvent(Protocal protocal, byte[] msgBytes)
    {
        lock (mMutex)
        {
            List<EventHandler> eventHandlers = null;
            if (mEventDic.ContainsKey(protocal))
            {
                eventHandlers = mEventDic[protocal];
            }

            //派发事件
            if (eventHandlers != null)
            {
                for (int i = 0; i < eventHandlers.Count; i++)
                {
                    eventHandlers[i]?.Invoke(msgBytes);
                }
            }
        }
    }
}
