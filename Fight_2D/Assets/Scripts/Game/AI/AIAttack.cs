using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class AIAttack : AIStateBase
{
    private float attackWaitTime;
    private float attackMin;
    private float attackMax;
    private float atkRangeX;
    private float atkRangeY;
    
    private bool canAtk = false;

    public AIAttack(AILogic aiLogic) : base(aiLogic)
    {
    }

    public override void EnterAIState()
    {
        Debug.Log("进入AI攻击状态");
        attackWaitTime = aiLogic.monster.atkWaitTime;
        attackMin = aiLogic.monster.atkMin;
        attackMax = aiLogic.monster.atkMax;
        atkRangeX = aiLogic.monster.atkRangeX;
        atkRangeY = aiLogic.monster.atkRangeY;
        canAtk = false;
        aiLogic.monster.Move(Vector2.zero);
        MonoMgr.GetInstance().StartCoroutine(AtkWaitTime());
    }

    private IEnumerator AtkWaitTime()
    {
        yield return new WaitForSeconds(attackWaitTime);
        canAtk = true;
    }

    private IEnumerator AtkCd()
    {
        yield return new WaitForSeconds(Random.Range(attackMin, attackMax));
        aiLogic.monster.Move(Vector2.zero);
        canAtk = true;
    }
    
    public override void ExitAIState()
    {
        canAtk = false;
    }

    public override void UpdateAISate()
    {
        //攻击玩家
        if (canAtk)
        {
            aiLogic.monster.Atk();
            canAtk = false;
            MonoMgr.GetInstance().StartCoroutine(AtkCd());
        }

        if (!aiLogic.monster.IsInAtkRange(PlayerObject.Instance.transform.position))
        {
            aiLogic.ChangeAIState(E_AI_STATE.Move);
        }
    }
}