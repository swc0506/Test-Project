using UnityEngine;

// 角色对象基类
public abstract class RoleObject : MonoBehaviour
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
                stateInfo1.IsName("pickup") || stateInfo1.IsName("throw") || animator.GetBool("isDead"))
                return false; 
            return true; 
        }
    }
    
    protected float jumpSpeed;
    protected float xSpeed;
    protected float gravity = 50f;
    
    public int speed = 4;
    
    protected virtual void Awake()
    {
        roleTransform = this.transform.Find("Role");
        roleSprite = roleTransform.GetComponent<SpriteRenderer>();
        shadowSprite = this.transform.Find("Square").GetComponent<SpriteRenderer>();
        animator = this.GetComponentInChildren<Animator>();
    }
    
    protected virtual void Update()
    {
        CheckMove();

        CheckJumpOrHitFly();
    }

    protected void CheckMove()
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

    protected void CheckJumpOrHitFly()
    {
        if (!GetIsGround())
        {
            roleTransform.Translate(Vector2.up * Time.deltaTime * jumpSpeed);
            jumpSpeed -= gravity * Time.deltaTime;
            if (roleTransform.localPosition.y <= 0)
            {
                roleTransform.localPosition = Vector3.zero;
                xSpeed = 0;
                SetIsGround(true);
                if (GetisHitFly()) 
                    Invoke("DelayClearDown", 0.5f);
            }
        }

        if (xSpeed != 0)
        {
            transform.Translate(Vector2.right * Time.deltaTime * xSpeed);
        }
    }
    
    public abstract void Atk();
    
    public abstract void Death();
    
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
            case E_Action_Type.Death:
                animator.SetBool("isDead", true);
                break;
            case E_Action_Type.None:
                break;
        }

        return true;
    }
    
    /// <summary>
    /// 受击
    /// </summary>
    public virtual void Wound(float hitTime)
    {
        if (!ChangeAction(E_Action_Type.Hurt))
            return;
        
        CancelInvoke("DelayClearHit");
        Invoke("DelayClearHit", hitTime);
    }
    
    private void DelayClearHit()
    {
        ChangeAction(E_Action_Type.HurtEnd);
    }
    
    /// <summary>
    /// 击飞
    /// </summary>
    /// <param name="xSpeed"></param>
    /// <param name="ySpeed"></param>
    public virtual void HitDown(float xSpeed, float ySpeed)
    {
        if (!ChangeAction(E_Action_Type.HurtFly))
            return;
        
        CancelInvoke("DelayClearHit");
        jumpSpeed = ySpeed;
        this.xSpeed = xSpeed;
    }
    
    /// <summary>
    /// 延迟起身
    /// </summary>
    private void DelayClearDown()
    {
        ChangeAction(E_Action_Type.HurtFlyEnd);
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
    Death,
    None,
}
