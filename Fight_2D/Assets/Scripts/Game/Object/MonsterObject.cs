using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterObject : RoleObject
{
    private AILogic aiLogic;
    public int aiID = 1; // 怪物AI ID
    private AIInfo aiInfo;

    // 出生位置
    [HideInInspector] public Vector2 bronPos;

    // 攻击间隔
    [Header("攻击设置")]
    // 攻击范围
    [Tooltip("攻击范围X")]
    [HideInInspector]
    public float atkRangeX = 1.0f;

    [HideInInspector] [Tooltip("攻击范围Y")] public float atkRangeY = 0.15f;
    [HideInInspector] [Tooltip("攻击间隔")] public float atkWaitTime = 0.5f;
    [HideInInspector] [Tooltip("攻击间隔最大值")] public float atkMax = 2f;
    [HideInInspector] [Tooltip("攻击间隔最小值")] public float atkMin = 1f;

    /// <summary>
    /// 巡逻类型 1: 按点巡逻 0: 按区域巡逻
    /// </summary>
    [Header("巡逻设置")]
    // 巡逻范围
    [Tooltip("巡逻范围X")]
    [HideInInspector]
    public float rangeW = 1.5f;

    [HideInInspector] [Tooltip("巡逻范围Y")] public float rangeH = 1.5f;

    [HideInInspector] [Tooltip("巡逻类型 1: 按点巡逻 0: 按区域巡逻")]
    public E_PatrolType ePatrolType = E_PatrolType.Area;

    // 巡逻方向
    [Tooltip("巡逻方向")] public E_KindIndex eKindIndex = E_KindIndex.Left;

    // 按点巡逻
    [HideInInspector] [Tooltip("按点巡逻")] public List<Vector2> patrolPoints = new List<Vector2>();
    [HideInInspector] public int patrolPointIndex = 0;

    // 最大活动距离
    [Header("范围设置")] [Tooltip("最大活动距离")] [HideInInspector]
    public float maxActiveDis = 5f;

    protected override void Awake()
    {
        base.Awake();
        aiLogic = new AILogic(this);
        bronPos = transform.position;
        aiInfo = BinaryDataMgr.Instance.GetTable<AIInfoContainer>().dataDic[aiID];
        SetMonsterInfo();
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
               ((BodyIsRight && targetPos.x - transform.position.x <= atkRangeX &&
                 targetPos.x > transform.position.x) ||
                (!BodyIsRight && transform.position.x - targetPos.x <= atkRangeX &&
                 transform.position.x > targetPos.x));
    }

    private void SetMonsterInfo()
    {
        patrolPoints.Clear();
        if (!string.IsNullOrEmpty(aiInfo.patrolPoint))
        {
            string[] points = aiInfo.patrolPoint.Split(";");
            for (int i = 0; i < points.Length; i++)
            {
                string[] point = points[i].Split(",");
                patrolPoints.Add(new Vector2(float.Parse(point[0]), float.Parse(point[1])));
            }
        }

        string[] range = aiInfo.patrolRange.Split(",");
        if (range.Length > 1)
        {
            rangeW = float.Parse(range[0]);
            rangeH = float.Parse(range[1]);
        }

        ePatrolType = (E_PatrolType)aiInfo.patrolType;

        string[] atkRange = aiInfo.atkRange.Split(",");
        if (atkRange.Length > 1)
        {
            atkRangeX = float.Parse(atkRange[0]);
            atkRangeY = float.Parse(atkRange[1]);
        }
        
        string[] atkCdTime= aiInfo.atkCdTime.Split(",");
        if (atkCdTime.Length > 1)
        {
            atkMin = float.Parse(atkCdTime[0]);
            atkMax = float.Parse(atkCdTime[1]);
        }

        atkWaitTime = aiInfo.atkWaitTime;
        maxActiveDis = aiInfo.backMaxDis;
    }
}