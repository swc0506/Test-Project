using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// 巡逻方向
public enum E_KindIndex
{
    Left,
    Top,
    Right,
    Bottom
}

public enum E_PatrolType
{
    Area,
    Point,
}

/// <summary>
/// AI巡逻状态
/// </summary>
public class AIPatrol : AIStateBase
{
    private float waitTime = 1f;
    // 巡逻目标位置
    private Vector2 targetPos;
    // 是否到达巡逻目标位置
    private bool isArrive = false;
    // 是否等待
    private bool isWait = false;
    
    private float attackRange = 2f;
    
    public AIPatrol(AILogic aiLogic) : base(aiLogic)
    {
        
    }

    /// <summary>
    /// 进入AI巡逻状态
    /// </summary>
    public override void EnterAIState()
    {
        //每次切换状态时，刚切换时会执行函数
        Debug.Log("进入AI巡逻状态");
    }

    /// <summary>
    /// 退出AI巡逻状态
    /// </summary>
    public override void ExitAIState()
    {
        //player = null; 
        aiLogic.monster.Move(Vector2.zero);
    }

    /// <summary>
    /// 更新AI巡逻状态
    /// </summary>
    public override void UpdateAISate()
    {
        if (isWait)
            return;
        
        // 到达巡逻目标位置，获取新的巡逻目标位置
        if (isArrive)
        {
            GetTargetPos();
            isArrive = false;
        }
        else // 移动到巡逻目标位置
        {
            aiLogic.monster.Move(targetPos - (Vector2)aiLogic.monster.transform.position);
            if (Vector2.Distance(targetPos, (Vector2)aiLogic.monster.transform.position) < 0.2f)
            {
                MonoMgr.GetInstance().StartCoroutine(Wait());
                isArrive = true;
                isWait = true;
                aiLogic.monster.Move(Vector2.zero);
            }
        }
        
        // 检测玩家是否进入攻击范围
        if (Vector3.Distance(PlayerObject.Instance.transform.position, aiLogic.monster.transform.position) < attackRange)
        {
            aiLogic.monster.Move(Vector2.zero);
            aiLogic.ChangeAIState(E_AI_STATE.Move);
        }
    }
    
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        isWait = false;
    }
    
    /// <summary>
    /// 获取巡逻目标位置
    /// </summary>
    private void GetTargetPos()
    {
        switch (aiLogic.monster.ePatrolType)
        {
            case E_PatrolType.Area:
                GetTargetType0();
                break;
            case E_PatrolType.Point:
                GetTargetType1();
                break;
        }
    }
    
    private void GetTargetType0()
    {
        float rangeW = aiLogic.monster.rangeW;
        float rangeH = aiLogic.monster.rangeH;
        switch (aiLogic.monster.eKindIndex)
        {
            case E_KindIndex.Left:
                targetPos.x = aiLogic.monster.bronPos.x - rangeW;
                targetPos.y = Random.Range(aiLogic.monster.bronPos.y - rangeH, aiLogic.monster.bronPos.y + rangeH);
                break;
            case E_KindIndex.Top:
                targetPos.x = Random.Range(aiLogic.monster.bronPos.x - rangeW, aiLogic.monster.bronPos.x + rangeW);
                targetPos.y = aiLogic.monster.bronPos.y + rangeH;
                break;
            case E_KindIndex.Right:
                targetPos.x = aiLogic.monster.bronPos.x + rangeW;
                targetPos.y = Random.Range(aiLogic.monster.bronPos.y - rangeH, aiLogic.monster.bronPos.y + rangeH);
                break;
            case E_KindIndex.Bottom:
                targetPos.x = Random.Range(aiLogic.monster.bronPos.x - rangeW, aiLogic.monster.bronPos.x + rangeW);
                targetPos.y = aiLogic.monster.bronPos.y - rangeH;
                break;
        }
        int index = (int)aiLogic.monster.eKindIndex;
        aiLogic.monster.eKindIndex = (E_KindIndex)((index + 1) % 4);
    }
    
    private void GetTargetType1()
    {
        targetPos = aiLogic.monster.patrolPoints[aiLogic.monster.patrolPointIndex];
        aiLogic.monster.patrolPointIndex = (aiLogic.monster.patrolPointIndex + 1) % aiLogic.monster.patrolPoints.Count;
    }
}