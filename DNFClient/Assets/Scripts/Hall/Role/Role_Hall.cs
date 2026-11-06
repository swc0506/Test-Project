using Fantasy;
using UnityEngine;
using ZM.AssetFrameWork;
using ZMGC.Hall;

public enum RoleSource
{
    Self,
    OtherPlayer,
}

/// <summary>
/// 管理大厅角色的位置和表现
/// </summary>
public partial class Role_Hall : MonoBehaviour
{
    private Animation mAnim;

    /// <summary>
    /// 摇杆输入方向
    /// </summary>
    private Vector3 mInputDir;

    /// <summary>
    /// 平滑值
    /// </summary>
    public float smoothPosSeed = 2;

    /// <summary>
    /// 角色渲染方向
    /// </summary>
    private Vector3 mRenderDir;

    /// <summary>
    /// 校正z轴移速倍率
    /// </summary>
    private float mAdjustSpeedMultiplierZ = 2;

    /// <summary>
    /// 激活移动状态
    /// </summary>
    private bool mActiveMoveState = false;

    /// <summary>
    /// 角色来源
    /// </summary>
    public RoleSource roleSource;

    /// <summary>
    /// 角色id
    /// </summary>
    public int RoleId { get; private set; }

    private HallRoleLogicCtrl mRoleLogicLayer;

    #region 状态同步

    /// <summary>
    /// 当前状态同步计数
    /// </summary>
    private int mSyncStateCurCount;

    /// <summary>
    /// 同步的目标位置
    /// </summary>
    private Vector3 mSyncTargetPos;

    /// <summary>
    /// 同步的朝向
    /// </summary>
    private Vector3 mSyncDir;

    /// <summary>
    /// 上一次摇杆的输入
    /// </summary>
    private Vector3 mLastInput;

    /// <summary>
    /// 状态数据包id
    /// </summary>
    private long mSyncPacketId;

    #endregion

    // Start is called before the first frame update
    public void Initlizate(RoleSource source, int roleid)
    {
        RoleId = roleid;
        roleSource = source;
        mAnim = GetComponent<Animation>();
        PlayAnim(AnimationName.Anim_Idle02);
        //监听摇杆事件输入
        if (roleSource == RoleSource.Self)
            JoystickUGUI.OnMoveCallBack += OnJoyStickMove;
        mRoleLogicLayer = HallWorld.GetExitsLogicCtrl<HallRoleLogicCtrl>();
        smoothPosSeed = 10;
    }

    /// <summary>
    /// 初始化玩家状态
    /// </summary>
    /// <param name="initPos"></param>
    public void InitPlayerStae(Vector3 initPos)
    {
        //初始化角色位置
        transform.position = initPos;
        PlayAnim(AnimationName.Anim_Idle02);
        //角色朝向
        mRenderDir.y = 0;
        transform.localEulerAngles = mRenderDir;
    }

    /// <summary>
    /// 以0.02秒的固定频率调用
    /// </summary>
    private void FixedUpdate()
    {
        //移动关闭
        if (!mActiveMoveState) return;

        //同步计数频率++
        mSyncStateCurCount++;
        //每隔100毫秒同步一下客户端的位置输入 即同步玩家状态
        if (mSyncStateCurCount == StateSyncConfig.MaxSyncStateCount)
        {
            mSyncStateCurCount = 0;
            //状态没有发生改变时，不需要进行同步 
            if (Vector3.Equals(mInputDir, Vector3.zero) && Vector3.Equals(mLastInput, Vector3.zero))
            {
                //Debuger.Log("状态没有发生改变，不需要进行同步 ");
                return;
            }

            mSyncPacketId++;
            //状态同步数据
            StateSyncData syncData = new StateSyncData() { inputDir = mInputDir.ToCSVector3() };
            //发送状态同步请求
            mRoleLogicLayer.SyncRoleState(syncData, mSyncPacketId);
            //记录上一次的输入
            mLastInput = mInputDir;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!mActiveMoveState) return;
        //更新位置
        UpdatePos();
        //更新方向
        UpdateDir();
        //更新状态
        UpdateState();
        //更新碰撞体
        UpdateCollider();
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    public void UpdatePos()
    {
        //根据摇杆的输入计算要移动到的位置  状态同步
        //Vector3 targetPos = transform.position + mInputDir;
        //mSyncTargetPos= targetPos;
        //做一个平滑移动
        transform.position = Vector3.Lerp(transform.position, mSyncTargetPos, Time.deltaTime * smoothPosSeed);
    }

    /// <summary>
    /// 更新方向
    /// </summary>
    public void UpdateDir()
    {
        //判断摇杆是否存在输入
        if (mSyncDir.x != 0)
        {
            mRenderDir.y = mSyncDir.x >= 0 ? 0 : 180;
            transform.localEulerAngles = mRenderDir;
        }
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    public void UpdateState()
    {
        if (mSyncDir != Vector3.zero)
        {
            PlayAnim(AnimationName.Anim_Walk);
        }
        else
        {
            PlayAnim(AnimationName.Anim_Idle02);
        }
    }

    /// <summary>
    /// 同步位置
    /// </summary>
    /// <param name="position"></param>
    /// <param name="inputDir"></param>
    public void SyncPosition(CSVector3 position, CSVector3 inputDir)
    {
        mSyncTargetPos = position.ToVector3();
        mSyncDir = inputDir.ToVector3();
    }

    /// <summary>
    /// 激活移动
    /// </summary>
    /// <param name="activeMove"></param>
    public void ActiveMove(bool activeMove)
    {
        mActiveMoveState = activeMove;
        if (!mActiveMoveState)
        {
            PlayAnim(AnimationName.Anim_Idle02);
        }
    }

    /// <summary>
    /// 摇杆移动输入
    /// </summary>
    /// <param name="inputDir"></param>
    public void OnJoyStickMove(Vector3 inputDir)
    {
        mInputDir = inputDir;
        mInputDir.z *= mAdjustSpeedMultiplierZ;
        //mSyncDir=mInputDir; 
    }

    /// <summary>
    /// 播放指定动画
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnim(string animName)
    {
        mAnim?.CrossFade(animName, 0.2f);
    }

    void OnDestroy()
    {
        if (roleSource == RoleSource.Self)
            JoystickUGUI.OnMoveCallBack -= OnJoyStickMove;
    }

    public void Release()
    {
        ZMAsset.Release(gameObject);
    }
}