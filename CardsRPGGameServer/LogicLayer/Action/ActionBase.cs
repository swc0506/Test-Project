using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBase : ILogicBehaviour
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
