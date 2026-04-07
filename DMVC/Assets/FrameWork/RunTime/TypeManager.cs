using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TypeManager
{
    private static IBehaviourExecution execution;
    
    public static void InitializeWorldAssemblies(World world, IBehaviourExecution behaviourExecution)
    {
        execution = behaviourExecution;
        
        //获取Unity和我们创建的脚本所在的程序集
        Assembly[] assemblyArr = AppDomain.CurrentDomain.GetAssemblies();
        Assembly worldAssembly = null;

        // 找到Assembly-CSharp程序集
        foreach (var assembly in assemblyArr)
        {
            if (assembly.GetName().Name == "Assembly-CSharp")
            {
                worldAssembly = assembly;
                break;
            }
        }

        if (worldAssembly == null)
        {
            Debug.LogError("未找到程序集Assembly-CSharp");
            return;
        }
        
        //获取程序集的命名空间
        //获取该命名空间下的所有脚本
        //判断当前脚本是否继承了Behaviour
        string nameSpace = world.GetType().Namespace;
        
        Type logicType = typeof(ILogicBehaviour);
        Type dataType = typeof(IDataBehaviour);
        Type msgType = typeof(IMsgBehaviour);
        
        Type[] types = worldAssembly.GetTypes();
        List<TypeOrder> logicTypes = new List<TypeOrder>();
        List<TypeOrder> dataTypes = new List<TypeOrder>();
        List<TypeOrder> msgTypes = new List<TypeOrder>();
        foreach (Type type in types)
        {
            string space = type.Namespace;
            if (string.Equals(space, nameSpace))
            {
                if (type.IsAbstract)
                    continue;
                
                if (logicType.IsAssignableFrom(type))
                {
                    TypeOrder typeOrder = new TypeOrder(GetLogicBehaviourOrderIndex(type), type);
                    logicTypes.Add(typeOrder);
                }
                else if (dataType.IsAssignableFrom(type))
                {
                    TypeOrder typeOrder = new TypeOrder(GetDataBehaviourOrderIndex(type), type);
                    dataTypes.Add(typeOrder);
                }
                else if (msgType.IsAssignableFrom(type))
                {
                    TypeOrder typeOrder = new TypeOrder(GetMsgBehaviourOrderIndex(type), type);
                    msgTypes.Add(typeOrder);
                }
            }
        }
        
        logicTypes.Sort((a, b) => a.order.CompareTo(b.order));
        dataTypes.Sort((a, b) => a.order.CompareTo(b.order));
        msgTypes.Sort((a, b) => a.order.CompareTo(b.order));
        
        foreach (var item in dataTypes)
        {
            IDataBehaviour dataBehaviour = Activator.CreateInstance(item.type) as IDataBehaviour; 
            world.AddDataMgr(dataBehaviour);
        }
        
        foreach (var item in msgTypes)
        {
            IMsgBehaviour msgBehaviour = Activator.CreateInstance(item.type) as IMsgBehaviour;
            world.AddMsgMgr(msgBehaviour);
        }
        
        foreach (var item in logicTypes)
        {
            ILogicBehaviour logicBehaviour = Activator.CreateInstance(item.type) as ILogicBehaviour;
            world.AddLogicCtrl(logicBehaviour);
        }
        
        logicTypes.Clear();
        dataTypes.Clear();
        msgTypes.Clear();
        execution = null;
    }
    
    private static int GetLogicBehaviourOrderIndex(Type type)
    {
        Type[] logicTypes = execution.GetLogicBehaviourExecutions();
        for (int i = 0; i < logicTypes.Length; i++)
        {
            if (logicTypes[i] == type)
                return i;
        }
        return 999;
    }
    
    private static int GetDataBehaviourOrderIndex(Type type)
    {
        Type[] logicTypes = execution.GetDataBehaviourExecutions();
        for (int i = 0; i < logicTypes.Length; i++)
        {
            if (logicTypes[i] == type)
                return i;
        }
        return 999;
    }
    
    private static int GetMsgBehaviourOrderIndex(Type type)
    {
        Type[] logicTypes = execution.GetMsgBehaviourExecutions();
        for (int i = 0; i < logicTypes.Length; i++)
        {
            if (logicTypes[i] == type)
                return i;
        }
        return 999;
    }
}
