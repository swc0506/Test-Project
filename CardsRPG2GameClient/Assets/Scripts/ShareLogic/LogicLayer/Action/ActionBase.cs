using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicLayer;

public class ActionBase : LogicLayer.ILogicBehaviour
{
    public bool actionComplete = false;//行动完成
    
    public virtual void OnCreate()
    {
    }

    public virtual void OnLogicFrameUpdate()
    {
    }

    public virtual void OnDestroy()
    {
    }
}
