using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourExecution
{
    Type[] GetLogicBehaviourExecutions();
    Type[] GetDataBehaviourExecutions();
    Type[] GetMsgBehaviourExecutions();
}
