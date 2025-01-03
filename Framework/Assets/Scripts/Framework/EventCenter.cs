using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//用于包裹
public abstract class EventInfoBass
{
}

public class EventInfo<T> : EventInfoBass
{
    //观察者对应的函数信息
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo : EventInfoBass
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

public class EventCenter : BaseManager<EventCenter>
{
    private Dictionary<EventType, EventInfoBass> _eventDic = new Dictionary<EventType, EventInfoBass>();
    
    private EventCenter(){}

    public void EventTrigger<T>(EventType eventType, T arg)
    {
        if (_eventDic.ContainsKey(eventType))
        {
            (_eventDic[eventType] as EventInfo<T>).actions?.Invoke(arg);
        }
    }
    
    public void EventTrigger(EventType eventType)
    {
        if (_eventDic.ContainsKey(eventType))
        {
            (_eventDic[eventType] as EventInfo).actions?.Invoke();
        }
    }

    public void AddEventListener<T>(EventType eventType, UnityAction<T> func)
    {
        if (_eventDic.ContainsKey(eventType))
        {
            (_eventDic[eventType] as EventInfo<T>).actions += func;
        }
        else
        {
            _eventDic.Add(eventType, new EventInfo<T>(func));
        }
    }
    
    public void AddEventListener(EventType eventType, UnityAction func)
    {
        if (_eventDic.ContainsKey(eventType))
        {
            (_eventDic[eventType] as EventInfo).actions += func;
        }
        else
        {
            _eventDic.Add(eventType, new EventInfo(func));
        }
    }

    public void RemoveEventListener<T>(EventType eventType, UnityAction<T> func)
    {
        if (_eventDic.ContainsKey(eventType))
            (_eventDic[eventType] as EventInfo<T>).actions -= func;
    }
    
    public void RemoveEventListener(EventType eventType, UnityAction func)
    {
        if (_eventDic.ContainsKey(eventType))
            (_eventDic[eventType] as EventInfo).actions -= func;
    }

    public void Clear()
    {
        _eventDic.Clear();
    }

    public void ClearEventListener(EventType eventType)
    {
        if (_eventDic.ContainsKey(eventType))
            _eventDic.Remove(eventType);
    }
}
