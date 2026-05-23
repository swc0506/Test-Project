using System;
using My.Physics2D;
using UnityEngine;
using ZM.FixIntMath;

public class Bullet : MonoBehaviour
{
    private bool mStartMove = false;

    /// <summary>
    /// 目标位置
    /// </summary>
    private FixIntVector2 mTargetPos;

    /// <summary>
    /// 子弹移动速度
    /// </summary>
    private readonly FixInt mMoveSpeed = 1300;

    /// <summary>
    /// 上一帧的位置
    /// </summary>
    private FixIntVector2 mLastFrameLogicPos;

    /// <summary>
    /// 当前帧位置
    /// </summary>
    private FixIntVector2 mLogicPos;

    /// <summary>
    /// 当前物体碰撞体
    /// </summary>
    private FixIntBoxCollider2D mCollider2D;

    public void Init(FixIntVector2 initPos)
    {
        //初始化子弹数据
        mLogicPos = mLastFrameLogicPos = initPos;
        transform.localPosition = initPos.ToVector2();
        mTargetPos = new FixIntVector2(transform.localPosition.x, 1000);

        //获取碰撞体数据
        BoxCollider2D ColliderData = transform.GetComponent<BoxCollider2D>();
        //创建碰撞体并设置碰撞体数据
        mCollider2D = new FixIntBoxCollider2D(transform.localPosition, new FixIntVector2(ColliderData.offset),
            new FixIntVector2(ColliderData.size));
        mStartMove = true;
    }

    // 每帧调用一次更新
    private void FixedUpdate()
    {
        if (!mStartMove) return;
        //更新位置
        mLogicPos = FixIntVector2.MoveTowards(mLogicPos, mTargetPos, mMoveSpeed);
        transform.localPosition = mLogicPos.ToVector2();
        //扫描体积检测碰撞
        CCDSweptVolume2D ccdSweptVolume2D = new CCDSweptVolume2D(mCollider2D, mLogicPos, mLastFrameLogicPos);
        bool isCollider = ccdSweptVolume2D.DetectCollider(WallGroup.Instance.wallGroupCollider2DArr[0]);
        if (isCollider)
        {
            Debug.Log($"Bullet 发生碰撞");
        }

        //记录上一帧的位置
        mLastFrameLogicPos = mLogicPos;
        //移动完成
        if (FixIntVector2.Distance(mLogicPos, mTargetPos) < 0.5f)
        {
            mStartMove = false;
        }
    }
}