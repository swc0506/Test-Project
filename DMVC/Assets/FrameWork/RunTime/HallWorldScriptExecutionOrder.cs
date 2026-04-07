using System;
using System.Collections;
using System.Collections.Generic;
using GC.Hall;
using UnityEngine;

public class HallWorldScriptExecutionOrder : IBehaviourExecution
{
    private static Type[] dataBehaviourExecutions = new Type[]
    {
        typeof(UserDataMgr),
    };
    private static Type[] logicBehaviourExecutions = new Type[] { };
    private static Type[] msgBehaviourExecutions = new Type[] { };
    
    
    public Type[] GetLogicBehaviourExecutions()
    {
        return logicBehaviourExecutions;
    }

    public Type[] GetDataBehaviourExecutions()
    {
        return dataBehaviourExecutions;
    }

    public Type[] GetMsgBehaviourExecutions()
    {
        return msgBehaviourExecutions;
    }
}
