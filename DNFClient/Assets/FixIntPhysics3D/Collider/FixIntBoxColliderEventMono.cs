using FixIntPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixIntBoxColliderEventMono : MonoBehaviour
{
    /// <summary>
    /// 所有检测目标碰撞体
    /// </summary>
    private List<FixIntBoxCollider> mTargetBoxColliderList = new List<FixIntBoxCollider>();
    /// <summary>
    /// 已经发生碰撞的碰撞体列表
    /// </summary>
    private List<FixIntBoxCollider> mAlreadyCccurredBoxColliderList = new List<FixIntBoxCollider>();
    /// <summary>
    /// 当前碰撞体
    /// </summary>
    FixIntBoxCollider mBoxCollider;
    /// <summary>
    /// 是否是逻辑帧更新，游戏内使用
    /// </summary>
    public bool isLogicFrameUpdate;
    void Start()
    {
        BoxColliderGizmo editorCollider = GetComponent<BoxColliderGizmo>();
        editorCollider.enabled = false;
        //创建碰撞体
        mBoxCollider= new FixIntBoxCollider(editorCollider.mSize,editorCollider.mConter+transform.position);
        mBoxCollider.SetBoxData(editorCollider.mConter + transform.position, editorCollider.mSize);
    }
    /// <summary>
    /// 添加碰撞检测目标
    /// </summary>
    /// <param name="collider"></param>
    public void AddCollider(FixIntBoxCollider collider)
    {
        mTargetBoxColliderList.Add(collider);
    }
    /// <summary>
    /// 移除一个碰撞检测目标
    /// </summary>
    /// <param name="collider"></param>
    public void RemoveCollider(FixIntBoxCollider collider)
    {
        mTargetBoxColliderList.Remove(collider);
    }
    void Update()
    {
        //检测当前碰撞是否与其他的碰撞体发生碰撞 并去判断是首次碰撞，退出碰撞，或者是持续碰撞中
        if (!isLogicFrameUpdate) 
        {
            UpdateCollider();
        }
    }
    void OnLogicFrameUpdate()
    {
        UpdateCollider();
    }
    private void UpdateCollider()
    {
        //使用反向循环的原因，保证在遍历的过程中代码的鲁棒性，在遍历过程中可能发生移除的情况
        for (int i = mTargetBoxColliderList.Count-1; i>=0; i--) 
        {
            FixIntBoxCollider targetCollider= mTargetBoxColliderList[i];
            bool isCollision= PhysicsManager.IsCollision(mBoxCollider, targetCollider);
            if (isCollision)
            {
                //该碰撞体是否已经发生碰撞  
                if (mAlreadyCccurredBoxColliderList.Contains(targetCollider))
                {
                    //碰撞停留
                    targetCollider.OnCollisionStayAction?.Invoke(mBoxCollider);
                }
                else
                {
                    //首次碰撞
                    mAlreadyCccurredBoxColliderList.Add(targetCollider);
                    targetCollider.OnCollisionEnterAction?.Invoke(mBoxCollider,this);
                }
            }
            else
            {
                //碰撞离开
                if (mAlreadyCccurredBoxColliderList.Contains(targetCollider))
                {
                    mAlreadyCccurredBoxColliderList.Remove(targetCollider);
                    targetCollider.OnCollisionExitAction?.Invoke(mBoxCollider);
                }
            }
        }
    }

    public void ReleaseCollider()
    {
        mBoxCollider?.OnRelease();
    }
}
