using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterObject : RoleObject
{
    private AILogic aiLogic;

    // 出生位置
    [HideInInspector] public Vector2 bronPos;

    // 攻击间隔
    [Header("攻击设置")]
    // 攻击范围
    [Tooltip("攻击范围X")]
    public float atkRangeX = 1.0f;
    [Tooltip("攻击范围Y")] public float atkRangeY = 0.15f;
    [Tooltip("攻击间隔")] public float atkWaitTime = 0.5f;
    [Tooltip("攻击间隔最大值")] public float atkMax = 2f;
    [Tooltip("攻击间隔最小值")] public float atkMin = 1f;

    /// <summary>
    /// 巡逻类型 1: 按点巡逻 0: 按区域巡逻
    /// </summary>
    [Header("巡逻设置")]
    // 巡逻范围
    [Tooltip("巡逻范围X")]
    public float rangeW = 1.5f;

    [Tooltip("巡逻范围Y")] public float rangeH = 1.5f;

    [Tooltip("巡逻类型 1: 按点巡逻 0: 按区域巡逻")] public E_PatrolType ePatrolType = E_PatrolType.Area;

    // 巡逻方向
    [Tooltip("巡逻方向")]
    public E_KindIndex eKindIndex = E_KindIndex.Left;

    // 按点巡逻
    [Tooltip("按点巡逻")] public List<Vector2> patrolPoints = new List<Vector2>();
    [HideInInspector] public int patrolPointIndex = 0;
    
    // 最大活动距离
    [Header("范围设置")]
    [Tooltip("最大活动距离")]
    public float maxActiveDis = 5f;

    protected override void Awake()
    {
        base.Awake();
        aiLogic = new AILogic(this);
        bronPos = transform.position;
    }

    protected override void Update()
    {
        base.Update();
        aiLogic.UpdateAI();
    }

    protected override void CheckBodyDir()
    {
        switch (aiLogic.GetNowAIState())
        {
            case E_AI_STATE.Move:
            case E_AI_STATE.Attack:
                if (PlayerObject.Instance != null)
                {
                    if (transform.position.x - PlayerObject.Instance.transform.position.x > 0.1f)
                    {
                        roleSprite.flipX = true;
                        shadowSprite.flipX = true;
                    }
                    else if (transform.position.x - PlayerObject.Instance.transform.position.x < -0.1f)
                    {
                        roleSprite.flipX = false;
                        shadowSprite.flipX = false;
                    }
                }
                break;
            default:
                base.CheckBodyDir();
                break;
        }
    }

    public void HitFly(float speedX, float speedY)
    {
    }

    public override void Atk()
    {
        if (CanAtk())
        {
            ChangeAction(E_Action_Type.Atk);
        }
    }

    public void Move(Vector2 dir)
    {
        moveDir = dir;
    }

    public override void Death()
    {
    }

    private bool CanAtk()
    {
        return !GetisHitFly() && !GetIsHit();
    }

    public bool IsInAtkRange(Vector2 targetPos)
    {
        return Mathf.Abs(targetPos.y - transform.position.y) <= atkRangeY &&
               ((BodyIsRight && targetPos.x - transform.position.x <= atkRangeX && targetPos.x > transform.position.x) ||
                (!BodyIsRight && transform.position.x - targetPos.x <= atkRangeX && transform.position.x > targetPos.x));
    }
}