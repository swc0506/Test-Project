using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventControl
{
    public delegate void EventHandler(object data);
    
    private static Dictionary<UIEventEnum, List<EventHandler>> mEventDict = new Dictionary<UIEventEnum, List<EventHandler>>();
    
    public static void AddEvent(UIEventEnum eventEnum, EventHandler handler)
    {
        if (mEventDict.ContainsKey(eventEnum) && !mEventDict[eventEnum].Contains(handler))
        {
            mEventDict[eventEnum].Add(handler);
        }
        else
        {
            mEventDict.Add(eventEnum, new List<EventHandler>() { handler });
        }
    }
    
    public static void RemoveEvent(UIEventEnum eventEnum, EventHandler handler)
    {
        if (mEventDict.ContainsKey(eventEnum))
        {
            if (mEventDict[eventEnum].Contains(handler))
            {
                mEventDict[eventEnum].Remove(handler);
            }
        }
    }
    
    public static void DispatchEvent(UIEventEnum eventEnum, object data = null)
    {
        if (mEventDict.TryGetValue(eventEnum, out List<EventHandler> handlers))
        {
            foreach (var handler in handlers)
            {
                handler?.Invoke(data);
            }
        }
    }
}
