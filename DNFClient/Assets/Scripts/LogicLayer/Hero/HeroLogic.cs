using Fantasy;
using FixMath;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using ZMGC.Battle;

public class HeroLogic : LogicActor
{
    /// <summary>
    /// 英雄id
    /// </summary>
    public int Heroid { get; private set; }

    public long PlayerId { get; private set; }

    public HeroLogic(int heroid,RenderObject renderObj, long playerid)
    {
        Heroid = heroid;
        RenderObj = renderObj;
        ObjectType = LogicObjectType.Hero;
        PlayerId = playerid;
    }
    public override void OnCreate()
    {
        base.OnCreate();
        InitActorSkill(Heroid);
        InitHeroAttribute();
    }
    public void InputFrameOperateData(FrameOperateData frameData)
    {
        //如果是移动输入 
        if ((OperateType)frameData.operateType == OperateType.InputMove)
        {
            //处理移动输入数据
            CSFixIntVector3 inputCSVector3 = frameData.inputDir;
            FixIntVector3 inputDir = new FixIntVector3(new FixInt((long)inputCSVector3.x), new FixInt((long)inputCSVector3.y), new FixInt((long)inputCSVector3.z));
            //输入玩家方向
            InputLoigcFrameEvent(inputDir, frameData.playerid);
            //更新渲染层其他玩家方向输入
            RenderObj.UpdateNetInputDir(inputDir);

        }
        else if ((OperateType)frameData.operateType == OperateType.ReleaseSkill)
        {
            Debuger.Log($"Input SKill Operate Skill ID:{frameData.skillid}");
            switch ((OperateSkillType)frameData.skillType)
            {
                case OperateSkillType.None:
                    Debuger.LogError("Net set SkillType, FrameOperate Failed!");
                    break;
                case OperateSkillType.ClickSkill: //处理瞬发技能攻击技能释放
                    ReleaseSKill(frameData.skillid);
                    break;
                case OperateSkillType.GuideSkill: //处理引导类型技能
                    FixIntVector3 skillGuidePos = new FixIntVector3(new FixInt((long)frameData.skillPos.x), new FixInt((long)frameData.skillPos.y), new FixInt((long)frameData.skillPos.z));
                    ReleaseSKill(frameData.skillid, skillGuidePos);
                    break;
                case OperateSkillType.StockPileTriggerSkill:
                    TriggerStockPileSkill(frameData.skillid);
                    break;
            }
        }
   
    }
    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();

    }
    /// <summary>
    /// 初始化英雄属性
    /// </summary>
    private void InitHeroAttribute()
    {
        HeroDataCfg data= ConfigCenter.Instance.GetHeroCfgById(Heroid);
        if (data==null)
        {
            Debug.LogError(Heroid+ " HeroDataCfg is Null! Return...");
            return;
        }
        hp = data.hp;
        mp = data.mp;
        ap = data.ap;
        ad = data.ad;
        adDef = data.adDef;
        apDef = data.apDef;
        pct = data.pct;
        mct = data.mct;
        adPctRate = data.adPctRate;
        apMctRate=data.apMctRate;
        str = data.str;
        sta = data.sta;
        Int = data.Int;
        spi = data.spi;
        agl = data.agl;
        Debug.Log("InitHeroAttribute Success..");
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        RenderObj.OnRelease();
    }
}
