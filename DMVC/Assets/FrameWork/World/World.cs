using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class World
{
    /// <summary>
    /// 逻辑层
    /// </summary>
    private static Dictionary<string, ILogicBehaviour> mLogicBehaviourDic = new Dictionary<string, ILogicBehaviour>();
    /// <summary>
    /// 数据层
    /// </summary>
    private static Dictionary<string, IDataBehaviour> mDataBehaviourDic = new Dictionary<string, IDataBehaviour>();
    /// <summary>
    /// 消息层
    /// </summary>
    private static Dictionary<string, IMsgBehaviour> mMsgBehaviourDic = new Dictionary<string, IMsgBehaviour>();

    /// <summary>
    /// 创建
    /// </summary>
    public virtual void OnCreat()
    {
        
    }
    
    public virtual void OnUpdate()
    {
        
    }
    
    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void OnDestroy()
    {
        
    }
    
    public void DestroyWorld(string nameSpace, object args = null)
    {
        List<string> needRemoveList = new List<string>();
        foreach (var item in mLogicBehaviourDic)
        {
            if (string.Equals(item.Value.GetType().Namespace, nameSpace)) 
                needRemoveList.Add(item.Key);
        }
        foreach (var item in needRemoveList)
        {
            mLogicBehaviourDic[item].OnDestroy();
            mLogicBehaviourDic.Remove(item);
        }
        
        needRemoveList.Clear();
        foreach (var item in mDataBehaviourDic)
        {
            if (string.Equals(item.Value.GetType().Namespace, nameSpace)) 
                needRemoveList.Add(item.Key);
        }
        foreach (var item in needRemoveList)
        {
            mDataBehaviourDic[item].OnDestroy();
            mDataBehaviourDic.Remove(item);
        }
        
        needRemoveList.Clear();
        foreach (var item in mMsgBehaviourDic)
        {
            if (string.Equals(item.Value.GetType().Namespace, nameSpace)) 
                needRemoveList.Add(item.Key);
        }
        foreach (var item in needRemoveList)
        {
            mMsgBehaviourDic[item].OnDestroy();
            mMsgBehaviourDic.Remove(item);
        }
        
        OnDestroy();
        OnDestroyPostProcess(args);
    }

    /// <summary>
    /// 销毁后处理
    /// </summary>
    /// <param name="args"></param>
    public virtual void OnDestroyPostProcess(object args)
    {
        
    }
    
    /// <summary>
    /// 获取逻辑层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetExitsLogicCtrl<T>() where T : ILogicBehaviour
    {
        if (mLogicBehaviourDic.TryGetValue(typeof(T).Name, out ILogicBehaviour logicBehaviour))
        {
            return (T)logicBehaviour;
        }
        
        Debug.LogError($"{typeof(T).Name} 不存在");
        return default;
    }
    
    /// <summary>
    /// 获取数据层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetExitsDataMgr<T>() where T : IDataBehaviour
    {
        if (mDataBehaviourDic.TryGetValue(typeof(T).Name, out IDataBehaviour dataBehaviour))
        {
            return (T)dataBehaviour;
        }
        
        Debug.LogError($"{typeof(T).Name} 不存在");
        return default;
    }
    
    /// <summary>
    /// 获取消息层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetExitsMsgMgr<T>() where T : IMsgBehaviour
    {
        if (mMsgBehaviourDic.TryGetValue(typeof(T).Name, out IMsgBehaviour msgBehaviour))
        {
            return (T)msgBehaviour;
        }
        
        Debug.LogError($"{typeof(T).Name} 不存在");
        return default;
    }
}
