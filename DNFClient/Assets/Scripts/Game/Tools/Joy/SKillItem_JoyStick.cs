using System;
using UnityEngine;
using UnityEngine.EventSystems;
public enum SKillGuideType
{
    None,
    Click,   //点击
    LongPress,//长按蓄力
    Position,//位置
    Dirction,//方向
}

/// <summary>
/// 技能释放回调
/// </summary>
/// <param name="sKillGuide">技能引导</param>
/// <param name="skillPos">技能释放位置</param>
/// <param name="skillId">技能id</param>
public delegate void OnReleaseSkillCallBack(SKillGuideType sKillGuide, Vector3 skillPos, int skillId);
/// <summary>
/// 技能引导位置回调
/// </summary>
/// <param name="sKillGuide">技能引导类型</param>
/// <param name="isCancel">释放取消</param>
/// <param name="skillPos">技能位置</param>
/// <param name="skillId">技能id</param>
/// <param name="skillDirDis">技能方向距离</param>
public delegate void OnSkillGuideCallBack(SKillGuideType sKillGuide, bool isCancel, Vector3 skillPos, int skillId, float skillDirDis);
/// <summary>
/// 技能点击或抬起的回调
/// </summary>
/// <param name="isPressDown"></param>
public delegate void OnClickOrPointerUpSkill(bool isPressDown);

/// <summary>
/// 技能摇杆按钮
/// </summary>
public class SKillItem_JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region 拖拽赋值
    [Header("摇杆摇点")]
    public Transform mJoyPointTrans;//摇杆摇点
    [Header("摇杆背景")]
    public Transform mJoyBackTrans;//摇杆背景
    [Header("技能取消图标对象")]
    public Transform mCancelSkillTrans;//技能取消图标对象
    [Header("摇杆最大移动距离(编译器指定)")]
    public int mJoyPointMaxDir;//摇杆最大移动距离(编译器指定)
    #endregion

    #region 代码初始化
    private float mSKillCancelRadius = 1000;//技能取消最大向量(配置)
    private SKillGuideType mSkillGuideType;//技能引导方式
    private float mSkillDirRnage;//技能范围
    private int mSkillid;//技能id
    private Vector2 mCenterPos;
    private RectTransform mRectTrans;
    #endregion

    #region 回调
    /// <summary>
    /// 释放技能回调
    /// </summary>
    public OnReleaseSkillCallBack OnReleaseSkill;
    /// <summary>
    /// 技能引导回调 拖拽中持续调用
    /// </summary>
    public OnSkillGuideCallBack OnSkillGuide;
    /// <summary>
    /// 技能点击和抬起回调
    /// </summary>
    public OnClickOrPointerUpSkill OnClickAnPointerUpSkill;
    #endregion
  
 
    public void InitSkillData(SKillGuideType sKillGuideType, int skillid, float skillRadius)
    {
        this.mSkillid = skillid;
        this.mSkillGuideType = sKillGuideType;
        this.mSkillDirRnage = skillRadius;
        SetJoyActiveState(false);
        mSKillCancelRadius = Screen.height * 1.0f / 1080 * mSKillCancelRadius;
        mCenterPos = transform.localPosition;
        mRectTrans = transform as RectTransform;
    }
    /// <summary>
    /// 手指按下
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        SetJoyActiveState(mSkillGuideType != SKillGuideType.Click && mSkillGuideType != SKillGuideType.LongPress);
        OnSkillGuide?.Invoke(mSkillGuideType, true, Vector3.zero, mSkillid, mSkillDirRnage);
        OnClickAnPointerUpSkill?.Invoke(true);
    }
    /// <summary>
    /// 手指移动
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        eventData.position = MousePosToUGUIPosition(eventData.position);
        Vector2 dir = eventData.position - mCenterPos;
        float dirlen = dir.magnitude;
        //如果超过最大移动范围
        if (dirlen > mJoyPointMaxDir)
        {
            Vector2 clampDir = Vector2.ClampMagnitude(dir, mJoyPointMaxDir);
            mJoyPointTrans.transform.localPosition = mCenterPos + clampDir;
        }
        else
        {
            mJoyPointTrans.transform.localPosition = eventData.position;
        }
        Debug.Log("JoyPointTrans LocalPos:"+mJoyPointTrans.transform.localPosition);
        if (mSkillGuideType == SKillGuideType.Position)  //位置性技能
        {
            if (dir == Vector2.zero) return;
            OnSkillGuide?.Invoke(mSkillGuideType, true, GetSkillPosition(dir), mSkillid, mSkillDirRnage);
        }
        else if (mSkillGuideType == SKillGuideType.Dirction)//指向性技能
        {
            OnSkillGuide?.Invoke(mSkillGuideType, true, GetSkillDirction(dir), mSkillid, mSkillDirRnage);
        }
        //显示取消 或 关闭
        SetCancelIconActiveState(dirlen > mSKillCancelRadius);

    }
    /// <summary>
    /// 手指抬起
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        eventData.position = MousePosToUGUIPosition(eventData.position);
        SetJoyActiveState(false);
        OnSkillGuide?.Invoke(mSkillGuideType, false, Vector3.zero, mSkillid, mSkillDirRnage);

        if (mSkillGuideType == SKillGuideType.Click || mSkillGuideType == SKillGuideType.LongPress)
        {
            OnClickAnPointerUpSkill?.Invoke(false);//按钮抬起
            OnReleaseSkill(mSkillGuideType, Vector3.zero, mSkillid);
            return;
        }

        SetCancelIconActiveState(false);
        mJoyPointTrans.position = transform.position;
        Vector2 dir = eventData.position - mCenterPos;
        //抬起时 移动向量的长度大于施法向量则技能取消
        if (dir.magnitude > mSKillCancelRadius)
        {
            Debug.Log("取消技能释放");
            return;
        }
        //释放技能
        if (mSkillGuideType == SKillGuideType.Click)//点击性技能
        {
            OnReleaseSkill?.Invoke(mSkillGuideType, Vector3.zero, mSkillid);
        }
        else if (mSkillGuideType == SKillGuideType.Position)//位置性技能
        {
            if (dir == Vector2.zero) return;
            OnReleaseSkill(mSkillGuideType, GetSkillPosition(dir), mSkillid);
        }
        else if (mSkillGuideType == SKillGuideType.Dirction)//指向性技能
        {
            if (dir == Vector2.zero) return;
            OnReleaseSkill?.Invoke(mSkillGuideType, GetSkillDirction(dir).normalized, mSkillid);
        }
        else
        {
            Debug.LogError("Skill Type is None !");
        }
        //按钮抬起
        OnClickAnPointerUpSkill?.Invoke(false);
    }
    /// <summary>
    /// 获取技能位置
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Vector3 GetSkillPosition(Vector2 dir)
    {
        dir = dir * 0.025f;// 0.025f代表本地本地坐标与世界坐标的缩放插值
        Vector2 clampDir = Vector2.ClampMagnitude(dir, mSkillDirRnage);//限制施法范围
        Vector3 skillPos = new Vector3(clampDir.x, 0, clampDir.y);//把平面向量转为3D坐标
        skillPos = Quaternion.Euler(20, 0, 0) * skillPos;//处理相机的旋转偏移值
        return skillPos;
    }
    /// <summary>
    /// 获取技能朝向
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Vector3 GetSkillDirction(Vector2 dir)
    {
        Vector3 dirction = new Vector3(dir.x, 0, dir.y);
        dirction = Quaternion.Euler(20, 0, 0) * dirction;//处理相机的旋转偏移值
        return dirction;
    }
    /// <summary>
    /// 设置摇杆可见性
    /// </summary>
    /// <param name="active"></param>
    private void SetJoyActiveState(bool active)
    {
        mJoyPointTrans.gameObject.SetActive(active);
        mJoyBackTrans.gameObject.SetActive(active);
    }
    /// <summary>
    /// 设置取消技能图标可见性
    /// </summary>
    /// <param name="active"></param>
    private void SetCancelIconActiveState(bool active)
    {
        mCancelSkillTrans.gameObject.SetActive(active);
    }
    /// <summary>
    /// 获取鼠标相对于当前对象的本地坐标
    /// </summary>
    /// <returns></returns>
    private Vector2 MousePosToUGUIPosition(Vector3 mousePosition)
    {
        //获取鼠标屏幕坐标
        Vector2 ConvertTomousePosition;
        //转换为Canvas针对物体的局部坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mRectTrans, mousePosition, UIModule.Instance.mUICamera, out ConvertTomousePosition);
        return ConvertTomousePosition;
    }
}
