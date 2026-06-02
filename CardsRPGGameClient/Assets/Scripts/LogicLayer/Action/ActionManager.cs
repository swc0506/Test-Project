using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : Singleton<ActionManager>, ILogicBehaviour
{
    /// <summary>
    /// 所有行动操作
    /// </summary>
    private List<ActionBase> actionList = new List<ActionBase>();

    public void OnCreate()
    {
    }

    public void RunAction(ActionBase action)
    {
#if CLIENT_LOGIC
        actionList.Add(action);
#else
        OnLogicFrameUpdate();
#endif
    }

    public void OnLogicFrameUpdate()
    {
        foreach (var t in actionList)
        {
            t.OnLogicFrameUpdate();
        }

        for (int i = actionList.Count - 1; i >= 0; i--)
        {
            if (actionList[i].actionComplete)
            {
                actionList.Remove(actionList[i]);
            }
        }
    }

    public void OnDestroy()
    {
    }
}