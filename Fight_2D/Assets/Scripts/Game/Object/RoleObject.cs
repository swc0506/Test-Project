using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 角色对象基类
public class RoleObject : MonoBehaviour
{
    // 移动方向
    protected Vector2 moveDir = Vector2.zero;
    protected SpriteRenderer roleSprite;
    protected Animator animator;
    public int speed = 5;
    
    protected virtual void Awake()
    {
        roleSprite = this.transform.Find("Role").GetComponent<SpriteRenderer>();
        animator = this.GetComponentInChildren<Animator>();
    }
    
    protected virtual void Update()
    {
        if (moveDir != Vector2.zero)
            transform.Translate(moveDir.normalized * (speed * Time.deltaTime));
        
        roleSprite.flipX = moveDir.x != 0 ? moveDir.x < 0 : roleSprite.flipX;

        ChangeAction(moveDir == Vector2.zero ? E_Action_Type.Idle : E_Action_Type.Walk);
    }
    
    protected void ChangeAction(E_Action_Type actType)
    {
        switch (actType)
        {
            case E_Action_Type.Idle:
                animator.SetBool("isMoving", false);
                break;
            case E_Action_Type.Walk:
                animator.SetBool("isMoving", true);
                break;
            case E_Action_Type.Atk1:
                break;
            case E_Action_Type.Atk2:
                break;
            case E_Action_Type.Atk3:
                break;
            case E_Action_Type.Jump:
                break;
            case E_Action_Type.JumpAtk:
                break;
            case E_Action_Type.Kick1:
                break;
            case E_Action_Type.Kick2:
                break;
            case E_Action_Type.Defend:
                break;
            case E_Action_Type.Pick:
                break;
            case E_Action_Type.Throw:
                break;
            case E_Action_Type.Hurt:
                break;
            case E_Action_Type.HurtFly:
                break;
            case E_Action_Type.None:
                break;
        }
    }
}

public enum E_Action_Type
{
    Idle,
    Walk,
    Atk1,
    Atk2,
    Atk3,
    Jump,
    JumpAtk,
    Kick1,
    Kick2,
    Defend,
    Pick,
    Throw,
    Hurt,
    HurtFly,
    None,
}
