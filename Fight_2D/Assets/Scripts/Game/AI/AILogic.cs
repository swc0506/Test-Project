using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI状态
/// </summary>
public enum E_AI_STATE
{
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
    public AIPatrol aiPatrol;
    public AIMove aiMove;
    public AIAttack aiAttack;
    public AIBack aiBack;
    
    public E_AI_STATE nowAiState = E_AI_STATE.Patrol;
    
    /// <summary>
    /// 在初始化时 初始化AI逻辑对象
    /// </summary>
    /// <param name="monster"></param>
    public AILogic(MonsterObject monster)
    {
        this.monster = monster;
        aiPatrol = new AIPatrol(this);
        aiMove = new AIMove(this);
        aiAttack = new AIAttack(this);
        aiBack = new AIBack(this);
    }
    
    /// <summary>
    /// 更新AI逻辑
    /// </summary>
    public void UpdateAI()
    {
        switch (nowAiState)
        {
            case E_AI_STATE.Patrol:
                aiPatrol.UpdateAISate();
                break;
            case E_AI_STATE.Move:
                aiMove.UpdateAISate();
                break;
            case E_AI_STATE.Attack:
                aiAttack.UpdateAISate();
                break;
            case E_AI_STATE.Back:
                aiBack.UpdateAISate();
                break;
        }
    }
}
