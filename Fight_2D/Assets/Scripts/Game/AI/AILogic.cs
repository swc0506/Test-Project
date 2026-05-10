using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI状态
/// </summary>
public enum E_AI_STATE
{
    Null,//空状态
    Patrol,//巡逻
    Move,//移动
    Attack,//攻击
    Back,//脱离
}

/// <summary>
/// AI逻辑
/// </summary>
public class AILogic
{
    // 怪物对象
    public MonsterObject monster;
    
    // AI状态逻辑对象
    private Dictionary<E_AI_STATE, AIStateBase> stateDic = new Dictionary<E_AI_STATE, AIStateBase>();
    private AIStateBase nowStateBase;

    private E_AI_STATE nowAiState = E_AI_STATE.Null;
    
    /// <summary>
    /// 在初始化时 初始化AI逻辑对象
    /// </summary>
    /// <param name="monster"></param>
    public AILogic(MonsterObject monster)
    {
        this.monster = monster;
        stateDic.Add(E_AI_STATE.Patrol, new AIPatrol(this));
        stateDic.Add(E_AI_STATE.Move, new AIMove(this));
        stateDic.Add(E_AI_STATE.Attack, new AIAttack(this));
        stateDic.Add(E_AI_STATE.Back, new AIBack(this));
        
        ChangeAIState(E_AI_STATE.Patrol);
    }
    
    public E_AI_STATE GetNowAIState()
    {
        return nowAiState;
    }
    
    /// <summary>
    /// 更新AI逻辑
    /// </summary>
    public void UpdateAI()
    {
        nowStateBase.UpdateAISate();
    }
    
    /// <summary>
    /// 改变AI状态
    /// </summary>
    /// <param name="aiState"></param>
    public void ChangeAIState(E_AI_STATE aiState)
    {
        if (aiState != E_AI_STATE.Null && stateDic.TryGetValue(nowAiState, out AIStateBase aiStateBase))
        {
            aiStateBase.ExitAIState();
        }
        
        if (stateDic.TryGetValue(aiState, out AIStateBase newAiState))
        {
            newAiState.EnterAIState();
            nowStateBase = newAiState;
        }
        nowAiState = aiState;
    }
}
