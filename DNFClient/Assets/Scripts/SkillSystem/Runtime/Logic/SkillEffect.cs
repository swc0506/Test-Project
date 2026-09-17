using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Skill
{
    /// <summary>
    /// 特效配置字典 key为特效配置的HashCode，Value为生成的对应特效
    /// </summary>

    private Dictionary<int, SkillEffectLogic> mEffectDic = new Dictionary<int, SkillEffectLogic>();
    public void OnLogicFrameUpdateEffect()
    {
        if (mSkillData.effectCfgList != null && mSkillData.effectCfgList.Count > 0)
        {
            foreach (var item in mSkillData.effectCfgList)
            {

                if (item.skillEffect != null && mCurLogicFrame == item.triggerFrame)
                {
                    DestroyEffect(item);
                    Transform parent = null;
                    if (item.isSetTransParent)
                    {
                        //获取机体渲染的父节点
                        parent = mSkillCreater.RenderObj.GetTransParent(item.transParent);
                    }
                    //生成特效并创建对象 
                    GameObject effectObj = GameObject.Instantiate(item.skillEffect, parent);//1.通过Editor获取当前配置的一个路径，2.给特效根改某个字符串
                    effectObj.transform.localPosition = Vector3.zero;
                    effectObj.transform.localScale = Vector3.one;
                    effectObj.transform.localRotation = Quaternion.identity;
                    //获取或者添加特效渲染器
                    SkillEffectRender effectRender = effectObj.GetComponent<SkillEffectRender>();
                    if (effectRender == null)
                        effectRender = effectObj.AddComponent<SkillEffectRender>();
                    //创建特效逻辑类
                    SkillEffectLogic effectLogic = new SkillEffectLogic(LogicObjectType.Effect, item, effectRender, mSkillCreater,this);
                    effectRender.SetLoigcObject(effectLogic,item.effectPosType!= EffectPosType.Zero);
                    mEffectDic.Add(item.GetHashCode(), effectLogic);
                }

                if (mCurLogicFrame == item.endFrame&&!item.isAttachAction)
                {
                    //销毁特效，重新开始播放
                    DestroyEffect(item);
                    continue;
                }
                SkillEffectLogic effectLogicObj = null;
                //调用特效逻辑的逻辑帧
                if (mEffectDic.TryGetValue(item.GetHashCode(),out effectLogicObj)&& effectLogicObj!=null)
                {
                    effectLogicObj.OnLogicFrameEffectUpdate(this,mCurLogicFrame);
                }
            }
        }
    }

    /// <summary>
    /// 销毁对应配置生成的特效
    /// </summary>
    /// <param name="item"></param>
    public void DestroyEffect(SkillEffectConfig item)
    {
        SkillEffectLogic effect = null;
        int hashCode = item.GetHashCode();
        mEffectDic.TryGetValue(hashCode, out effect);
        if (effect != null)
        {
            mEffectDic.Remove(hashCode);
            effect.OnDestroy();
        }
    }
    /// <summary>
    /// 释放所有特效资源
    /// </summary>
    public void ReleaseAllEffect()
    {
        foreach (var item in mSkillData.effectCfgList)
        {
            if (!item.isAttachAction)
            {
                DestroyEffect(item);
            }
        }
    }
}
