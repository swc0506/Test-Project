using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObject : RoleObject
{
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
