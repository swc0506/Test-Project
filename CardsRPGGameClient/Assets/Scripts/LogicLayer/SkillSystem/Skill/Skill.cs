using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    public void CreatBullet()
    {
        mSkillTarget = BattleRule.GetNormalAttackTarget(
            WorldManager.BattleWorld.heroLogicCtrl.GetHeroListByTeam(mSkillOwner,
                (HeroTeamEnum)mSkillCfg.roleTargetType), mSkillOwner.HeroData.seatid);
        BulletManager.Instance.CreateBullet(mSkillCfg.bullet, mSkillOwner, mSkillTarget, mSkillCfg.skillAttackDurationMS, SkillTrigger);
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
                WorldManager.BattleWorld.heroLogicCtrl.GetHeroListByTeam(mSkillOwner,
                    (HeroTeamEnum)mSkillCfg.roleTargetType), mSkillOwner.HeroData.seatid);
            targetPos = new VInt3(mSkillTarget.LogicPosition.x, mSkillTarget.LogicPosition.y,
                mSkillTarget.LogicPosition.z);
            VInt z = mSkillOwner.TeamEnum == HeroTeamEnum.Enemy ? new VInt(-3).Int : new VInt(3).Int;
            targetPos.z -= z.RawInt;
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
            mSkillOwner.UpdateAnger(mSkillOwner.HeroData.atkRage);
        }
        
        CreateSkillEffect(CauseDamage());
        AdditionBuff();
        SkillShakeAfter();
        if (mSkillCfg.skillAttackDurationMS > 0)
        {
            LogicTimerManager.Instance.DelayCall((VInt)mSkillCfg.skillAttackDurationMS, () => { MoveToSet(SkillEnd);});
        }
        else
        {
            MoveToSet(SkillEnd);
        }
    }

    /// <summary>
    /// 创建技能特效
    /// </summary>
    private void CreateSkillEffect(List<HeroLogic> heroList)
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

#endif
    }

    /// <summary>
    /// 造成伤害
    /// </summary>
    private List<HeroLogic> CauseDamage()
    {
        //根据攻击的类型计算
        List<HeroLogic> heroList =
            WorldManager.BattleWorld.heroLogicCtrl.GetHeroListByTeam(mSkillOwner,
                (HeroTeamEnum)mSkillCfg.roleTargetType);
        List<HeroLogic> attackList =
            BattleRule.GetAttackListByAttackType(mSkillCfg.skillAttackType, heroList, mSkillOwner.HeroData.seatid);
        foreach (var hero in attackList)
        {
            VInt damage = BattleRule.CalDamage(mSkillCfg, mSkillOwner, hero);
            hero.UpdateAnger(hero.HeroData.takeDamageRage);
            mSkillOwner.UpdateAnger(0);
            if (damage != 0)
            {
                if (mSkillCfg.roleTargetType == RoleTargetType.Teammate)
                {
                    hero.DamageHp(damage);
                }
                else
                {
                    hero.DamageHp(damage);
                }

                Debugger.Log("damage: " + damage.RawInt);
            }
        }

        return attackList;
    }

    /// <summary>
    /// 附加buff
    /// </summary>
    private void AdditionBuff()
    {
    }

    /// <summary>
    /// 技能后摇
    /// </summary>
    public void SkillShakeAfter()
    {
        SkillState = SkillState.ShakeAfter;
    }

    /// <summary>
    /// 移动到座位
    /// </summary>
    public void MoveToSet(Action moveFinish)
    {
        VInt3 seatPos = VInt3.zero;
#if CLIENT_LOGIC
        
        Transform[] seatArr = mSkillOwner.TeamEnum == HeroTeamEnum.Enemy
            ? BattleWorldNodes.Instance.enemyRootArr
            : BattleWorldNodes.Instance.heroRootArr;

#endif
        seatPos = new VInt3(seatArr[mSkillOwner.HeroData.seatid].position);
        MoveToAction action =
            new MoveToAction(mSkillOwner, seatPos, (VInt)mSkillCfg.skillShakeAfterTimeMS, moveFinish);
        ActionManager.Instance.RunAction(action);
    }

    /// <summary>
    /// 技能结束
    /// </summary>
    public void SkillEnd()
    {
        Debugger.Log("SkillEnd Id:" + mSkillCfg.skillId);
        mSkillOwner.EndAction();
    }
}