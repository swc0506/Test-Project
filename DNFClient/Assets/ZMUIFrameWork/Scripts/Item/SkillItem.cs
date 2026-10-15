using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FixMath;

public class SkillItem : MonoBehaviour
{
    public Text cdText;
    public Image iconImage;

    public Image cdMaskImage;

    //技能摇杆脚本
    public SKillItem_JoyStick skillJoyStick;

    private Skill mSkillData;
    private LogicActor mSkillCreater;

    private HeroRender mHeroRender;

    //是否进入技能CD
    private bool mIsEnterSkillCD;

    //已经冷却的时间
    private float mAlreadyCDTime;

    //技能冷却时间
    private float mSkillCDTime;

    /// <summary>
    /// 设置技能数据
    /// </summary>
    /// <param name="skillData">技能数据</param>
    /// <param name="logicActor">技能释放者</param>
    public void SetItemSkillData(Skill skillData, LogicActor logicActor)
    {
        mSkillData = skillData;
        mSkillCreater = logicActor;
        mHeroRender = logicActor.RenderObj as HeroRender;
        //初始化技能摇杆数据
        skillJoyStick.InitSkillData(GetSkillGuideType(skillData.SKillCfg.skillType), skillData.skillid,
            skillData.SKillCfg.skillGuideRange);
        skillJoyStick.OnReleaseSkill += OnTriggerSKill;
        skillJoyStick.OnSkillGuide += OnUpdateSkillGuide;
        iconImage.sprite = skillData.SKillCfg.skillIcon;
        cdText.gameObject.SetActive(false);
        cdMaskImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 触发对应的技能 当释放摇杆时调用
    /// </summary>
    /// <param name="sKillGuide">技能引导类型</param>
    /// <param name="skillPos">技能触发位置</param>
    /// <param name="skillId">技能触发id</param>
    public void OnTriggerSKill(SKillGuideType sKillGuide, Vector3 skillPos, int skillId)
    {
        if (sKillGuide == SKillGuideType.Click)
        {
            mSkillCreater.ReleaseSKill(skillId, releaseSkillCallBack: OnReleaseSkillCallBack);
        }
        else if (sKillGuide == SKillGuideType.LongPress)
        {
            //触发蓄力技能
            mSkillCreater.TriggerStockPileSkill(skillId);
        }
        else if (sKillGuide == SKillGuideType.Position)
        {
            //确保技能释放位置一定在地面上
            skillPos.y = 0;
            //指定位置释放技能 TODO 释放当前的技能
            mSkillCreater.ReleaseSKill(skillId, mSkillCreater.LogicPos + new FixIntVector3(skillPos),
                OnReleaseSkillCallBack);
            mHeroRender.OnGuideRelease();
        }
    }

    /// <summary>
    /// 更新技能引导回调 (当摇杆一直按着的时候调用)
    /// </summary>
    /// <param name="sKillGuide">技能引导类型</param>
    /// <param name="isCancel">是否取消</param>
    /// <param name="skillPos">技能释放位置</param>
    /// <param name="skillId">技能id</param>
    /// <param name="skillDirDis">技能方向的距离</param>
    public void OnUpdateSkillGuide(SKillGuideType sKillGuide, bool isCancel, Vector3 skillPos, int skillId,
        float skillDirDis)
    {
        if (sKillGuide == SKillGuideType.LongPress)
        {
            //释放技能
            mSkillCreater.ReleaseSKill(skillId, releaseSkillCallBack: OnReleaseSkillCallBack);
        }
        else if (sKillGuide == SKillGuideType.Position)
        {
            //TODO 角色
            mHeroRender.UpdateSkillGuide(sKillGuide, skillId, isCancel, skillPos, skillDirDis);
        }
    }

    /// <summary>
    /// 释放技能回调
    /// </summary>
    /// <param name="isReleaseSuccess"></param>
    public void OnReleaseSkillCallBack(bool isReleaseSuccess)
    {
        if (isReleaseSuccess)
        {
            EnterSKillCD();
        }
    }

    /// <summary>
    /// 进入技能CD
    /// </summary>
    public void EnterSKillCD()
    {
        cdText.gameObject.SetActive(true);
        cdMaskImage.gameObject.SetActive(true);
        mIsEnterSkillCD = true;
        //获取技能冷却时间
        mSkillCDTime = mAlreadyCDTime = mSkillData.SKillCfg.skillCDTimeMs / 1000;
        cdText.text = mSkillCDTime.ToString();
        int cdTime = mSkillData.SKillCfg.skillCDTimeMs / 1000;
        //通过逻辑帧定时器来更新当前技能冷却时间
        LogicTimerManager.Instance.DelayCall(1, () =>
        {
            //更新冷却逻辑
            cdTime--;
            if (cdTime <= 0)
            {
                //技能冷却结束
                cdText.gameObject.SetActive(false);
                cdMaskImage.gameObject.SetActive(false);
                mIsEnterSkillCD = false;
            }
            else
            {
                cdText.text = cdTime.ToString();
            }
        }, cdTime);
    }

    private void Update()
    {
        if (mIsEnterSkillCD)
        {
            cdMaskImage.fillAmount = (mAlreadyCDTime -= Time.deltaTime) / mSkillCDTime;
        }
    }

    /// <summary>
    /// 根据技能类型获取技能引导类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public SKillGuideType GetSkillGuideType(SKillType type)
    {
        SKillGuideType sKillGuide = SKillGuideType.Click;
        if (type == SKillType.StockPile)
        {
            sKillGuide = SKillGuideType.LongPress;
        }
        else if (type == SKillType.Ballistic || type == SKillType.Chant || type == SKillType.None)
        {
            sKillGuide = SKillGuideType.Click;
        }
        else if (type == SKillType.PosGuide)
        {
            sKillGuide = SKillGuideType.Position;
        }

        return sKillGuide;
    }

    public void OnDestroy()
    {
        skillJoyStick.OnReleaseSkill -= OnTriggerSKill;
        skillJoyStick.OnSkillGuide -= OnUpdateSkillGuide;
    }
}