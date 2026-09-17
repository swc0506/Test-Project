using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

/// <summary>
/// 渲染对象
/// </summary>
public class RenderObject : MonoBehaviour
{
    /// <summary>
    /// 逻辑对象
    /// </summary>
    public LogicObject logicObject;
    /// <summary>
    /// 位置插值速度
    /// </summary>
    protected float mSmoothPosSpeed = 10;

    protected bool mIsUpdatePosAndRotation = true;
    protected Vector2 mRenderDir;
    public void SetLoigcObject(LogicObject logicObj,bool isUpdatePosAndRotation=true)
    {
        logicObject = logicObj;
        mIsUpdatePosAndRotation = isUpdatePosAndRotation;
        //初始化位置
        transform.position = logicObj.LogicPos.ToVector3();
        if (mIsUpdatePosAndRotation == false)
            transform.localPosition = Vector3.zero;
        UpdateDir();
    }
    /// <summary>
    /// 渲染层脚本创建
    /// </summary>
    public virtual void OnCreate()
    {

    }
    /// <summary>
    /// 渲染层脚本释放
    /// </summary>
    public virtual void OnRelease()
    {

    }
    /// <summary>
    /// Unity引擎渲染帧，根据程序配置，渲染帧一般一秒为30帧、和60帧以及120帧 
    /// </summary>
    public virtual void Update()
    {
        UpdatePosition();
        UpdateDir();
    }
    /// <summary>
    ///通用的位置更新逻辑
    /// </summary>
    public virtual void UpdatePosition()
    {
        if (mIsUpdatePosAndRotation == false)
        {
            return;
        }
        //对逻辑位置做插值动画，流畅渲染对象移动
        transform.position = Vector3.Lerp(transform.position, logicObject.LogicPos.ToVector3(), Time.deltaTime * mSmoothPosSpeed);
    }
    /// <summary>
    /// 通用的方向更新逻辑
    /// </summary>
    public virtual void UpdateDir()
    {
        if (mIsUpdatePosAndRotation == false)
        {
            return;
        }
        //mRenderDir.x = logicObject.LogicXAxis >= 0 ? 0 : -20;
        mRenderDir.y = logicObject.LogicXAxis >= 0 ? 0 : 180;
        transform.localEulerAngles = mRenderDir;
    }

    public virtual void PlayAnim(AnimationClip clip)
    {

    }
    public virtual void PlayAnim(string name)
    {

    }

    public virtual string GetCurAnimName()
    {
        return "";
    }
    /// <summary>
    /// 伤害
    /// </summary>
    /// <param name="damageValue">伤害值</param>
    /// <param name="source">伤害来源</param>
    public virtual void Damage(int damageValue, DamageSource source)
    {
        GameObject damageItemObj = ZMAssetsFrame.Instantiate(AssetPathConfig.GAME_PREFABS + "DamageItem/DamageText", null);
        DamageTextItem item = damageItemObj.GetComponent<DamageTextItem>();
        item.ShowDamageText(damageValue, this);
    }
    public virtual void OnHit(GameObject effectHitObj, int survivalTimems, LogicObject source)
    {
        if (effectHitObj!=null)
        {
            GameObject hitEffctObj= GameObject.Instantiate(effectHitObj);
            hitEffctObj.transform.position = source.RenderObj.transform.position; //纯表现逻辑，为了表现统一可以直接使用渲染位置
            hitEffctObj.transform.localScale = source.LogicXAxis > 0 ? Vector3.one : new Vector3(-1,1,1);
            GameObject.Destroy(hitEffctObj, survivalTimems*1.0f/1000);
        }
    }
    public virtual Transform GetTransParent(TransParentType parentType) { return null; }
}
