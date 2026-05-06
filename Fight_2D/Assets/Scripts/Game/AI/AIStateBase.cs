using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI状态基类
/// </summary>
public abstract class AIStateBase
{
    /// <summary>
    /// AI逻辑对象
    /// </summary>
    protected AILogic aiLogic;
    
    public AIStateBase(AILogic aiLogic)
    {
        this.aiLogic = aiLogic;
    }
    
    // 进入AI状态
    public abstract void EnterAIState();
    
    // 退出AI状态
    public abstract void ExitAIState();
    
    // 更新AI状态
    public abstract void UpdateAISate();
}
