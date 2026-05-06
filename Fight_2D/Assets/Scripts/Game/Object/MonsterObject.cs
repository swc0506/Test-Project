using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObject : RoleObject
{
    private AILogic aiLogic;

    protected override void Awake()
    {
        base.Awake();
        aiLogic = new AILogic(this);
    }
    
    protected override void Update()
    {
        base.Update();
        aiLogic.UpdateAI();
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
}
