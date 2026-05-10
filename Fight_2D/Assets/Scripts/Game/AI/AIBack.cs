using UnityEngine;

public class AIBack : AIStateBase
{
    private Vector2 targetPos;
    
    public AIBack(AILogic aiLogic) : base(aiLogic)
    {
    }

    public override void EnterAIState()
    {
        Debug.Log("进入AI脱离状态");
        targetPos = aiLogic.monster.bronPos;
    }

    public override void ExitAIState()
    {
    }

    public override void UpdateAISate()
    {
        if (Vector2.Distance(targetPos, aiLogic.monster.transform.position) < 0.1f)
        {
            aiLogic.ChangeAIState(E_AI_STATE.Patrol);
        }
        else
        {
            aiLogic.monster.Move(targetPos - (Vector2)aiLogic.monster.transform.position);
        }
    }
}