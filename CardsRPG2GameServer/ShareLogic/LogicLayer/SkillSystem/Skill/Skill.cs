using System;
using System.Collections;
using System.Collections.Generic;
using LogicLayer;
using UnityEngine;

public enum SkillState
{
    None,
    ShakeBefore,
    ShakeAfter,
}

public class Skill
{
    public int SkillId { get; private set; }
    public SkillState SkillState { get; private set; }
    private SkillConfig mSkillCfg;
    private HeroLogic mSkillOwner;
    private HeroLogic mSkillTarget;
    private bool mIsNormalAtk;

    public Skill(int skillId, LogicObject skillOwner, bool isNormalAtk)
    {
        SkillId = skillId;
        mSkillOwner = (HeroLogic)skillOwner;
        mIsNormalAtk = isNormalAtk;
        mSkillCfg = SkillConfigCenter.LoadSkillConfig(SkillId);
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    public void ReleaseSkill()
    {
        Debugger.Log("ReleaseSkill id:" + mSkillCfg.skillId);
        SkillShakeBefore();
        PlaySkillAnim();

        if (mSkillCfg.skillType == SkillType.MoveToAttack || mSkillCfg.skillType == SkillType.MoveToCenter ||
            mSkillCfg.skillType == SkillType.MoveToEnemyCenter)
        {
            MoveToTarget(SkillTrigger);
        }
        else if (mSkillCfg.skillType == SkillType.Chant)
        {
            SkillChant(SkillTrigger);
        }
        else if (mSkillCfg.skillType == SkillType.Ballistic)
        {
            LogicTimerManager.Instance.DelayCall(mSkillCfg.skillShakeBeforeTimeMS, CreatBullet);
        }
    }

    /// <summary>
    /// 技能前摇
    /// </summary>
    public void SkillShakeBefore()
    {
        SkillState = SkillState.ShakeBefore;
    }

    public void PlaySkillAnim()
    {
        mSkillOwner.PlayAnim(mSkillCfg.skillAnim);

#if CLIENT_LOGIC
        if (!mIsNormalAtk)
            //BattleWorldNodes.Instance.skillWindow.PlayAnim(mSkillCfg, mSkillOwner.Id);
            UIModule.Instance.GetWindow<ZM.UI.SkillWindow>().PlayAnim(mSkillCfg, mSkillOwner.Id);
        if (mSkillCfg.skillAudio != null)
            AudioController.GetInstance().PlaySoundByAudioClip(mSkillCfg.skillAudio, false, 40);
#endif
    }

    public void CreatBullet()
    {
        mSkillTarget = BattleRule.GetNormalAttackTarget(
            LogicLayer.BattleWorldManager.BattleWorld.heroLogicCtrl.GetHeroListByTeam(mSkillOwner,
                (HeroTeamEnum)mSkillCfg.roleTargetType), mSkillOwner.HeroData.seatId);
        BulletManager.Instance.CreateBullet(mSkillCfg.bullet, mSkillOwner, mSkillTarget,
            mSkillCfg.skillAttackDurationMS, SkillTrigger);
    }

    /// <summary>
    /// 技能吟唱
    /// </summary>
    public void SkillChant(Action chantFinish)
    {
        LogicTimerManager.Instance.DelayCall((VInt)mSkillCfg.skillShakeBeforeTimeMS, chantFinish);
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    private void MoveToTarget(Action moveFinish)
    {
        VInt3 targetPos = VInt3.zero;

#if CLIENT_LOGIC

        if (mSkillCfg.skillType == SkillType.MoveToAttack)
        {
            mSkillTarget = BattleRule.GetNormalAttackTarget(
                LogicLayer.BattleWorldManager.BattleWorld.heroLogicCtrl.GetHeroListByTeam(mSkillOwner,
                    (HeroTeamEnum)mSkillCfg.roleTargetType), mSkillOwner.HeroData.seatId);
            targetPos = new VInt3(mSkillTarget.LogicPosition.x, mSkillTarget.LogicPosition.y,
                mSkillTarget.LogicPosition.z);
            VInt x = mSkillOwner.TeamEnum == HeroTeamEnum.Enemy ? new VInt(-1).Int : new VInt(1).Int;
            targetPos.x -= x.RawInt;
        }
        else if (mSkillCfg.skillType == SkillType.MoveToEnemyCenter)
        {
            targetPos = new VInt3(mSkillOwner.TeamEnum == HeroTeamEnum.Enemy
                ? BattleWorldManager.BattleWorld.Root3D.conterTrans.position
                : BattleWorldManager.BattleWorld.Root3D.enemysConter.position);
        }
        else if (mSkillCfg.skillType == SkillType.MoveToCenter)
        {
            targetPos = new VInt3(BattleWorldManager.BattleWorld.Root3D.conterTrans.position);
        }

#endif

        MoveToAction action =
            new MoveToAction(mSkillOwner, targetPos, (VInt)mSkillCfg.skillShakeBeforeTimeMS, moveFinish);
        ActionManager.Instance.RunAction(action);
    }

    /// <summary>
    /// 技能触发
    /// </summary>
    private void SkillTrigger()
    {
        // 普通攻击增加怒气
        if (mIsNormalAtk)
        {
            mSkillOwner.UpdateAnger(mSkillOwner.HeroData.atkRange);
        }

        var targetHeroList = CauseDamage();
        SetSkillMask(targetHeroList, true);
        CreateSkillEffect(targetHeroList);
        AdditionBuff(targetHeroList);
        SkillShakeAfter();
        if (mSkillCfg.skillAttackDurationMS > 0)
        {
            LogicTimerManager.Instance.DelayCall((VInt)mSkillCfg.skillAttackDurationMS,
                () =>
                {
                    SetSkillMask(targetHeroList, false);
                    MoveToSeat(SkillEnd);
                });
        }
        else
        {
            SetSkillMask(targetHeroList, false);
            MoveToSeat(SkillEnd);
        }
    }

    /// <summary>
    /// 创建技能特效
    /// </summary>
    private void CreateSkillEffect(List<HeroLogic> heroList)
    {
#if RENDER_LOGIC
        TriggerSkillEffectCfgList();
        CreateSkillHitEffect(heroList);
#endif
    }

    private void CreateSkillHitEffect(List<HeroLogic> heroList)
    {
#if RENDER_LOGIC
        //击中特效
        if (!string.IsNullOrEmpty(mSkillCfg.skillHitEffect))
        {
            for (int i = 0; i < heroList.Count; i++)
            {
                SkillEffect skillEffect =
                    ResourcesManager.Instance.LoadObject<SkillEffect>(
                        AssetPathConfig.SKILL_EFFECT + mSkillCfg.skillHitEffect);
                skillEffect.SetEffectPos(heroList[i].LogicPosition);
            }
        }

        if (mSkillCfg.skillEffectDataCfgList == null || mSkillCfg.skillEffectDataCfgList.Count == 0)
            return;

        foreach (var effectCfg in mSkillCfg.skillEffectDataCfgList)
        {
            if (string.IsNullOrEmpty(effectCfg.hitEffectName))
            {
                Debugger.Log("hitEffectName is null");
                continue;
            }

            foreach (var hero in heroList)
            {
                LogicTimerManager.Instance.DelayCall(effectCfg.delayTimeMs, () =>
                {
                    SkillEffect skillEffect =
                        ResourcesManager.Instance.LoadObject<SkillEffect>(
                            AssetPathConfig.SKILL_EFFECT + effectCfg.hitEffectName);
                    skillEffect.SetEffectPos(hero.LogicPosition);
                });
            }
        }
#endif
    }

    /// <summary>
    /// 触发技能遮罩
    /// </summary>
    /// <param name="heroList"></param>
    /// <param name="isShow"></param>
    private void SetSkillMask(List<HeroLogic> heroList, bool isShow)
    {
#if RENDER_LOGIC

        isShow = !isShow;
        switch (mSkillCfg.attackMaskType)
        {
            case SkillMaskEnum.NoMask:
                return;
            case SkillMaskEnum.HideTeamMask:
                BattleWorldManager.BattleWorld.heroLogicCtrl.SetSelfTeamMask(mSkillOwner, isShow);
                break;
            case SkillMaskEnum.HideSelfAllMask:
                BattleWorldManager.BattleWorld.heroLogicCtrl.SetSelfAllMask(mSkillOwner, isShow);
                break;
            case SkillMaskEnum.HideOutsideOfTargetMask:
                BattleWorldManager.BattleWorld.heroLogicCtrl.SetOutsideOfTargetMask(mSkillOwner, heroList, isShow);
                break;
        }

#endif
    }

    /// <summary>
    /// 触发技能效果配置列表
    /// </summary>
    public void TriggerSkillEffectCfgList()
    {
#if RENDER_LOGIC
        //技能特效
        if (!string.IsNullOrEmpty(mSkillCfg.skillEffect))
        {
            SkillEffect skillEffect =
                ResourcesManager.Instance.LoadObject<SkillEffect>(
                    AssetPathConfig.SKILL_EFFECT + mSkillCfg.skillEffect);
            if (mSkillOwner.TeamEnum == HeroTeamEnum.Enemy)
            {
                Vector3 angle = skillEffect.transform.eulerAngles;
                angle.y = 180;
                skillEffect.transform.eulerAngles = angle;
            }

            if (mSkillCfg.skillAttackType == SkillAttackType.AllHero)
            {
                skillEffect.SetEffectPos(VInt3.zero);
            }
            else
            {
                skillEffect.SetEffectPos(mSkillOwner.LogicPosition);
            }
        }

        if (mSkillCfg.skillEffectDataCfgList == null || mSkillCfg.skillEffectDataCfgList.Count == 0)
            return;

        foreach (var cfg in mSkillCfg.skillEffectDataCfgList)
        {
            if (string.IsNullOrEmpty(cfg.effectName))
            {
                Debugger.Log("effectName is empty");
                continue;
            }

            LogicTimerManager.Instance.DelayCall(cfg.delayTimeMs, () =>
            {
                // 创建特效
                SkillEffect skillEffect =
                    ResourcesManager.Instance.LoadObject<SkillEffect>(
                        AssetPathConfig.SKILL_EFFECT + cfg.effectName);

                Vector3 effectScale = mSkillOwner.TeamEnum == HeroTeamEnum.Enemy ? new Vector3(-1, 1, 1) : Vector3.one;
                skillEffect.SetScale(effectScale);
                BattleRoot3D battleRoot3D = BattleWorldManager.BattleWorld.Root3D;
                Vector3 targetPos = mSkillTarget != null ? mSkillTarget.LogicPosition.vec3 : Vector3.zero;
                switch (cfg.effectPos)
                {
                    case SkillEffectPosEnum.SkillOwner:
                        targetPos = mSkillOwner.LogicPosition.vec3;
                        break;
                    case SkillEffectPosEnum.EnemyCenter:
                        targetPos = mSkillOwner.TeamEnum == HeroTeamEnum.Enemy
                            ? battleRoot3D.herosConter.position
                            : battleRoot3D.enemysConter.position;
                        break;
                    case SkillEffectPosEnum.SelfCenter:
                        targetPos = mSkillOwner.TeamEnum == HeroTeamEnum.Self
                            ? battleRoot3D.herosConter.position
                            : battleRoot3D.enemysConter.position;
                        break;
                    case SkillEffectPosEnum.MapCenter:
                        targetPos = battleRoot3D.conterTrans.position;
                        break;
                }

                skillEffect.SetEffectPos(new VInt3(targetPos), cfg.durationTimeMs);
                // 触发摄像机动画
                TriggerCameraAnim(cfg, skillEffect.gameObject);
                if (cfg.audiDataCfgList != null && cfg.audiDataCfgList.Count > 0)
                {
                    foreach (var audiCfg in cfg.audiDataCfgList)
                    {
                        LogicTimerManager.Instance.DelayCall(audiCfg.delayTimeMs,
                            () =>
                            {
                                AudioController.GetInstance().PlaySoundByAudioClip(audiCfg.audioName, false, 60);
                            });
                    }
                }
            });
        }
#endif
    }

    /// <summary>
    ///  触发摄像机动画
    /// </summary>
    private void TriggerCameraAnim(SkillEffectDataCfg cfg, GameObject effectObj)
    {
#if RENDER_LOGIC

        if (cfg.useCameraAnim)
        {
            EffectCamera effectCamera = effectObj.GetComponent<EffectCamera>();
            if (effectCamera == null)
            {
                return;
            }

            // 设置战斗摄像机的父节点
            BattleWorldManager.BattleWorld.Root3D.battleCamera.transform.SetParent(effectCamera.effectCamera);
            LogicTimerManager.Instance.DelayCall(cfg.durationTimeMs - LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL_MS,
                () => { BattleWorldManager.BattleWorld.Root3D.RevertCamera(); });
        }

#endif
    }

    /// <summary>
    /// 造成伤害
    /// </summary>
    private List<HeroLogic> CauseDamage()
    {
        //根据攻击的类型计算
        List<HeroLogic> heroList =
            LogicLayer.BattleWorldManager.BattleWorld.heroLogicCtrl.GetHeroListByTeam(mSkillOwner,
                (HeroTeamEnum)mSkillCfg.roleTargetType);
        var attackTargetList =
            BattleRule.GetAttackListByAttackType(mSkillCfg.skillAttackType, heroList, mSkillOwner.HeroData.seatId);
        foreach (var hero in attackTargetList)
        {
            VInt damage = BattleRule.CalDamage(mSkillCfg, mSkillOwner, hero);
            hero.UpdateAnger(hero.HeroData.takeDamageRange);
            mSkillOwner.UpdateAnger(0);
            if (damage != 0)
            {
                if (mSkillCfg.roleTargetType == RoleTargetType.Teammate)
                {
                    hero.DamageHp(-damage);
                }
                else
                {
                    hero.DamageHp(damage);
                }

                Debugger.Log("damage: " + damage.RawInt);
            }
        }

        return attackTargetList;
    }

    /// <summary>
    /// 附加buff
    /// </summary>
    private void AdditionBuff(List<HeroLogic> attackTargetList)
    {
        if (mSkillCfg.addBuffs != null && mSkillCfg.addBuffs.Length > 0)
        {
            foreach (var atkTarHero in attackTargetList)
            {
                for (int i = 0; i < mSkillCfg.addBuffs.Length; i++)
                {
                    BuffManager.Instance.CreateBuff(mSkillCfg.addBuffs[i], mSkillOwner, atkTarHero);
                }
            }
        }
    }

    /// <summary>
    /// 技能后摇
    /// </summary>
    private void SkillShakeAfter()
    {
        SkillState = SkillState.ShakeAfter;
    }

    /// <summary>
    /// 移动到座位
    /// </summary>
    private void MoveToSeat(Action moveFinish)
    {
        if (mSkillCfg.skillType == SkillType.Chant || mSkillCfg.skillType == SkillType.Ballistic)
        {
            LogicTimerManager.Instance.DelayCall((VInt)mSkillCfg.skillShakeAfterTimeMS, moveFinish);
        }
        else
        {
            VInt3 seatPos = VInt3.zero;
#if CLIENT_LOGIC

            Transform[] seatArr = mSkillOwner.TeamEnum == HeroTeamEnum.Enemy
                ? BattleWorldManager.BattleWorld.Root3D.rightSeatTransArr
                : BattleWorldManager.BattleWorld.Root3D.leftSeatTransArr;
            seatPos = new VInt3(seatArr[mSkillOwner.HeroData.seatId].position);
#endif
            MoveToAction action =
                new MoveToAction(mSkillOwner, seatPos, (VInt)mSkillCfg.skillShakeAfterTimeMS, moveFinish);
            ActionManager.Instance.RunAction(action);
        }
    }

    /// <summary>
    /// 技能结束
    /// </summary>
    private void SkillEnd()
    {
        Debugger.Log("SkillEnd Id:" + mSkillCfg.skillId);
        mSkillOwner.EndAction();
    }
}