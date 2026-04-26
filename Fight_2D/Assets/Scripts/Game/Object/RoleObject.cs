using UnityEngine;

// 角色对象基类
public class RoleObject : MonoBehaviour
{
    // 移动方向
    protected Vector2 moveDir = Vector2.zero;
    protected SpriteRenderer roleSprite;
    protected SpriteRenderer shadowSprite;
    protected Animator animator;
    protected Transform roleTransform;

    protected bool CanMove
    {
        get
        {
            AnimatorStateInfo stateInfo1 = animator.GetCurrentAnimatorStateInfo(1);
            if (stateInfo1.IsName("atk1") || stateInfo1.IsName("atk2") ||
                stateInfo1.IsName("atk3") || stateInfo1.IsName("kick1") ||
                stateInfo1.IsName("kick2") || stateInfo1.IsName("df") ||
                stateInfo1.IsName("hit") || stateInfo1.IsName("down") ||
                stateInfo1.IsName("pickup") || stateInfo1.IsName("throw"))
                return false; 
            return true; 
        }
    }
    
    public int speed = 5;
    
    protected virtual void Awake()
    {
        roleTransform = this.transform.Find("Role");
        roleSprite = roleTransform.GetComponent<SpriteRenderer>();
        shadowSprite = this.transform.Find("Square").GetComponent<SpriteRenderer>();
        animator = this.GetComponentInChildren<Animator>();
    }
    
    protected virtual void Update()
    {
        if (moveDir != Vector2.zero && CanMove)
        {
            transform.Translate(moveDir.normalized * (speed * Time.deltaTime));
            
            roleSprite.flipX = moveDir.x != 0 ? moveDir.x < 0 : roleSprite.flipX;
            shadowSprite.flipX = roleSprite.flipX;
        }
        else
        {
            moveDir = Vector2.zero;
        }
        ChangeAction(moveDir == Vector2.zero ? E_Action_Type.Idle : E_Action_Type.Walk);
    }
    
    protected bool ChangeAction(E_Action_Type actType)
    {
        switch (actType)
        {
            case E_Action_Type.Idle:
                animator.SetBool("isMoving", false);
                break;
            case E_Action_Type.Walk:
                animator.SetBool("isMoving", true);
                break;
            case E_Action_Type.Atk:
                if (GetIsDefend())
                    return false;
                animator.SetTrigger("atkTrigger");
                break;
            case E_Action_Type.Jump:
                animator.SetTrigger("jumpTrigger");
                SetIsGround(false);
                break;
            case E_Action_Type.JumpAtk:
                animator.SetTrigger("jumpAtkTrigger");
                break;
            case E_Action_Type.Kick:
                if (GetIsDefend())
                    return false;
                animator.SetTrigger("kickTrigger");
                break;
            case E_Action_Type.Defend:
                if (!GetIsGround())
                    return false;
                animator.SetBool("isDefend", true);
                break;
            case E_Action_Type.DefendEnd:
                animator.SetBool("isDefend", false);
                break;
            case E_Action_Type.Pick:
                break;
            case E_Action_Type.Throw:
                break;
            case E_Action_Type.Hurt:
                if (animator.GetBool("isHitFly"))
                    return false;
                animator.SetBool("isHit", true);
                break;
            case E_Action_Type.HurtEnd:
                animator.SetBool("isHit", false);
                break;
            case E_Action_Type.HurtFly:
                if (animator.GetBool("isHitFly"))
                    return false;
                animator.SetBool("isHitFly", true);
                SetIsGround(false);
                animator.SetBool("isHit", false);
                break;
            case E_Action_Type.HurtFlyEnd:
                animator.SetBool("isHitFly", false);
                break;
            case E_Action_Type.None:
                break;
        }

        return true;
    }

    protected void SetAtkCount(E_Action_Type actType, int count)
    {
        switch (actType)
        {
            case E_Action_Type.Atk:
                animator.SetInteger("atkCount", count);
                break;
            case E_Action_Type.Kick:
                animator.SetInteger("kickCount", count);
                break;
        }
    }
    
    protected void SetIsGround(bool isGround)
    {
        animator.SetBool("isGround", isGround);
    }
    
    protected bool GetIsGround()
    {
        return animator.GetBool("isGround");
    }
    
    protected bool GetIsHit()
    {
        return animator.GetBool("isHit");
    }
    
    protected bool GetisHitFly()
    {
        return animator.GetBool("isHitFly");
    }
    
    protected bool GetIsDefend()
    {
        return animator.GetBool("isDefend");
    }
}

public enum E_Action_Type
{
    Idle,
    Walk,
    Atk,
    Jump,
    JumpAtk,
    Kick,
    Defend,
    DefendEnd,
    Pick,
    Throw,
    Hurt,
    HurtEnd,
    HurtFly,
    HurtFlyEnd,
    None,
}
