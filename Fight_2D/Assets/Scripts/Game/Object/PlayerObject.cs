using UnityEngine;

public class PlayerObject : RoleObject
{
    private float upForce = 12;
    private int atkCount = 0;
    private int kickCount = 0;
    
    protected override void Awake()
    {
        base.Awake();
        InputMgr.GetInstance().StartOrEndCheck(true);
        AddListener();
    }
    
    void OnDestroy()
    {
        RemoveListener();
    }

    private void Jump()
    {
        var stateInfo1 = animator.GetCurrentAnimatorStateInfo(1);
        if (!GetIsGround() || !stateInfo1.IsName("Null"))
            return;
        ChangeAction(E_Action_Type.Jump);
        jumpSpeed = upForce;
    }
    
    private void JumpAtk()
    {
        if (CheckJumpAtk())
        {
            ChangeAction(E_Action_Type.JumpAtk);
        }
    }
    
    public override void Atk()
    {
        if (!ChangeAction(E_Action_Type.Atk) || !CanAtk())
        {
            atkCount = 0;
            return;
        }
            
        CancelInvoke("DelayClearAtkCount");
        atkCount++;
        SetAtkCount(E_Action_Type.Atk, atkCount);
        Invoke("DelayClearAtkCount", 0.3f);
        kickCount = 0;
        SetAtkCount(E_Action_Type.Kick, kickCount);
    }

    public override void Death()
    {
        
    }

    private void Kick()
    {
        if (!CanAtk())
        {
            atkCount = 0;
            return;
        }

        ChangeAction(E_Action_Type.Kick);
        CancelInvoke("DelayClearKickCount");
        kickCount++;
        SetAtkCount(E_Action_Type.Kick, kickCount);
        Invoke("DelayClearKickCount", 0.3f);
        atkCount = 0;
        SetAtkCount(E_Action_Type.Atk, atkCount);
    }
    
    private void DelayClearAtkCount()
    {
        atkCount = 0;
        SetAtkCount(E_Action_Type.Atk, atkCount);
    }
    
    private void DelayClearKickCount()
    {
        kickCount = 0;
        SetAtkCount(E_Action_Type.Kick, kickCount);
    }
    
    private void CheckX(float value)
    {
        moveDir.x = value;
    }
    
    private void CheckY(float value)
    {
        moveDir.y = value;
    }

    private void Defend(bool isDefend)
    {
        ChangeAction(isDefend ? E_Action_Type.Defend : E_Action_Type.DefendEnd);
    }

    private void PickUp()
    {
        var stateInfo1 = animator.GetCurrentAnimatorStateInfo(1);
        if (!stateInfo1.IsName("Null"))
            return;
        animator.SetTrigger("pickUpTrigger");
    }
    
    private void Throw()
    {
        var stateInfo1 = animator.GetCurrentAnimatorStateInfo(1);
        if (!stateInfo1.IsName("Null"))
            return;
        animator.SetTrigger("throwTrigger");
    }
    
    private void CheckKeyDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.J:
                if (!GetIsGround())
                {
                    JumpAtk();
                }
                else
                {
                    Atk();
                }
                break;
            case KeyCode.K: 
                if (!GetIsGround())
                {
                    JumpAtk();
                }
                else
                {
                    Kick();
                }
                break;
            case KeyCode.L:
                Defend(true);
                break;
            case KeyCode.Space:
                Jump();
                break;
            case KeyCode.E:
                Wound(1f);
                break;
            case KeyCode.R:
                HitDown(-5, upForce);
                break;
            case KeyCode.Alpha1:
                PickUp();
                break;
            case KeyCode.Alpha2:
                Throw();
                break;
        }
    }
    
    private void CheckKeyUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.L:
                Defend(false);
                break;
        }
    }
    
    private bool CheckJumpAtk()
    {
        return jumpSpeed > -1.5 && !animator.GetCurrentAnimatorStateInfo(1).IsName("jumpKick") && CanAtk();
    }

    private bool CanAtk()
    {
        return !GetisHitFly() && !GetIsHit() && !GetIsDefend();
    }

    private void AddListener()
    {
        EventCenter.GetInstance().AddEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().AddEventListener<float>("Vertical", CheckY);
        EventCenter.GetInstance().AddEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
        EventCenter.GetInstance().AddEventListener<KeyCode>("SomeKeyUp", CheckKeyUp);
    }
    
    private void RemoveListener()
    {
        EventCenter.GetInstance().RemoveEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().RemoveEventListener<float>("Vertical", CheckY);
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("SomeKeyUp", CheckKeyUp);
    }
}
