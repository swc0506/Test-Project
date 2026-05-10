using UnityEngine;

public class AIMove : AIStateBase
{
    private Vector2 targetPos;
    private float maxDis;
    
    public AIMove(AILogic aiLogic) : base(aiLogic)
    {
    }

    public override void EnterAIState()
    {
        Debug.Log("进入AI移动状态");
        maxDis = aiLogic.monster.maxActiveDis;
    }

    public override void ExitAIState()
    {
        aiLogic.monster.Move(Vector2.zero);
    }

    public override void UpdateAISate()
    {
        if (aiLogic.monster.IsInAtkRange(PlayerObject.Instance.transform.position))
        {
            aiLogic.monster.Move(Vector2.zero);
            aiLogic.ChangeAIState(E_AI_STATE.Attack);
            return;
        }
        else if (Vector2.Distance(aiLogic.monster.bronPos, aiLogic.monster.transform.position) > maxDis)
        {
            aiLogic.monster.Move(Vector2.zero);
            aiLogic.ChangeAIState(E_AI_STATE.Back);
            return;
        }
        
        //追击玩家
        targetPos = PlayerObject.Instance.transform.position.x > aiLogic.monster.transform.position.x
            ? PlayerObject.Instance.transform.position - Vector3.right * 1
            : PlayerObject.Instance.transform.position + Vector3.right * 1;
        aiLogic.monster.Move(targetPos - (Vector2)aiLogic.monster.transform.position);

        
    }
}