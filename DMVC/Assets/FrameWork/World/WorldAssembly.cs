using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class World
{
    public void AddLogicCtrl(ILogicBehaviour logicBehaviour)
    {
        mLogicBehaviourDic.Add(logicBehaviour.GetType().Name, logicBehaviour);
        logicBehaviour.OnCreat();
    }
    
    public void AddDataMgr(IDataBehaviour dataBehaviour)
    {
        mDataBehaviourDic.Add(dataBehaviour.GetType().Name, dataBehaviour);
        dataBehaviour.OnCreat();
    }
    
    public void AddMsgMgr(IMsgBehaviour msgBehaviour)
    {
        mMsgBehaviourDic.Add(msgBehaviour.GetType().Name, msgBehaviour);
        msgBehaviour.OnCreat();
    }
}
