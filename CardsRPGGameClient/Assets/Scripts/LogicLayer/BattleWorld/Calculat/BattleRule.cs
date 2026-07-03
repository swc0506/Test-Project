using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRule
{
    public static VInt CalDamage(SkillConfig skillConfig, HeroLogic attacker, HeroLogic target)
    {
        VInt rawDamage = new VInt(0);
        switch (skillConfig.damageType)
        {
            case DamageType.NormalDamage:
                //伤害减免比率 = 护甲 / (伤害 + 护甲) * 100%; atk = 1000 def = 300 300/(1000+300)==300/1300 = 0.23
                //攻击力 - 攻击力*伤害减免比率
                VInt damageRate = target.Def / (attacker.Atk + target.Def);
                rawDamage = attacker.Atk - (attacker.Atk * damageRate);
                break;
            case DamageType.RealDamage:
                //无视护盾和减伤
                rawDamage = attacker.Atk;
                break;
            case DamageType.AtkPercentage:
                //计算伤害比率
                damageRate = target.Def / (attacker.Atk + target.Def);
                //攻击力百分比
                VInt atkMuti = skillConfig.damagePercentage / new VInt(100);
                VInt totalDamage = attacker.Atk * atkMuti;
                rawDamage = (totalDamage - (totalDamage * damageRate));
                break;
            case DamageType.HpPercentage:
                //计算伤害比率
                damageRate = target.Def / (attacker.Atk + target.Def);
                //百分比
                VInt hpMuti = skillConfig.damagePercentage / new VInt(100);
                VInt hpTotalDamage = attacker.Hp * hpMuti;
                rawDamage = (hpTotalDamage - (hpTotalDamage * damageRate));
                break;
        }
        return rawDamage;
    }
    
    public static VInt CalBuffDamage(BuffConfig buffConfig, HeroLogic attacker, HeroLogic target)
    {
        VInt rawDamage = new VInt(0);
        switch (buffConfig.damageType)
        {
            case BuffDamageType.NormalAttackDamage:
                //伤害减免比率 = 护甲 / (伤害 + 护甲) * 100%; atk = 1000 def = 300 300/(1000+300)==300/1300 = 0.23
                //攻击力 - 攻击力*伤害减免比率
                VInt damageRate = target.Def / (attacker.Atk + target.Def);
                rawDamage = attacker.Atk - (attacker.Atk * damageRate);
                break;
            case BuffDamageType.RealDamage:
                //无视护盾和减伤
                rawDamage = attacker.Atk;
                break;
            case BuffDamageType.AtkPercentage:
                //计算伤害比率
                damageRate = target.Def / (attacker.Atk + target.Def);
                //攻击力百分比
                VInt atkMuti = buffConfig.damagePercentage / new VInt(100);
                VInt totalDamage = attacker.Atk * atkMuti;
                rawDamage = (totalDamage - (totalDamage * damageRate));
                break;
            case BuffDamageType.HpPercentage:
                //计算伤害比率
                damageRate = target.Def / (attacker.Atk + target.Def);
                //百分比
                VInt hpMuti = buffConfig.damagePercentage / new VInt(100);
                VInt hpTotalDamage = attacker.Hp * hpMuti;
                rawDamage = (hpTotalDamage - (hpTotalDamage * damageRate));
                break;
        }
        return rawDamage;
    }
    
    /// <summary>
    /// 默认攻击前排目标
    /// </summary>
    /// <param name="heroList"></param>
    /// <param name="heroSeatId"></param>
    /// <returns></returns>
    public static HeroLogic GetNormalAttackTarget(List<HeroLogic> heroList, int heroSeatId)
    {
        if (heroList[0].objectState == LogicObjectState.Survival)
        {
            return heroList[0];
        }

        //选择中后排
        int[] attackOrderArr = GetAttackSeatArr(heroSeatId);
        for (int i = 0; i < attackOrderArr.Length; i++)
        {
            int index = attackOrderArr[i];
            if (heroList[index].objectState == LogicObjectState.Survival)
            {
                return heroList[index];
            }
        }

        return null;
    }

    public static int[] GetAttackSeatArr(int startSeatId)
    {
        if (startSeatId == 0)
        {
            return new int[] { 0, 1, 2, 3, 4 };
        }
        else if (startSeatId == 1 || startSeatId == 4)
        {
            return new int[] { 1, 2, 4, 3, 0 };
        }
        else if (startSeatId == 2 || startSeatId == 3)
        {
            return new int[] { 2, 1, 3, 4, 0 };
        }

        return null;
    }

    /// <summary>
    /// 通过攻击的类型获取攻击目标
    /// </summary>
    /// <param name="attackType"></param>
    /// <param name="heroList"></param>
    /// <param name="heroSeatId"></param>
    /// <returns></returns>
    public static List<HeroLogic> GetAttackListByAttackType(SkillAttackType attackType, List<HeroLogic> heroList,
        int heroSeatId)
    {
        //攻击列表
        List<HeroLogic> attackList = new List<HeroLogic>();

        switch (attackType)
        {
            case SkillAttackType.SingTarget:
                attackList.Add(GetNormalAttackTarget(heroList, heroSeatId));
                break;
            case SkillAttackType.AllHero:
                attackList = GetHeroSurvivalList(heroList);
                break;
            case SkillAttackType.BackRowHero:
                attackList = GetBackRowHeroList(heroList);
                attackList = GetHeroSurvivalList(attackList);
                if (attackList.Count == 0)
                {
                    //攻击前排
                    attackList = GetFrontRowHeroList(heroList);
                }

                attackList = GetHeroSurvivalList(attackList);
                break;
            case SkillAttackType.FrontRowHero:
                attackList = GetFrontRowHeroList(heroList);
                attackList = GetHeroSurvivalList(attackList);
                if (attackList.Count == 0)
                {
                    //攻击后排
                    attackList = GetBackRowHeroList(heroList);
                }

                attackList = GetHeroSurvivalList(attackList);
                break;
            case SkillAttackType.SameColumnHero:
                int[] targetArr = GetAttackSeatArr(heroSeatId);
                attackList.Add(heroList[targetArr[0]]);
                attackList.Add(heroList[targetArr[1]]);
                attackList = GetHeroSurvivalList(attackList);
                if (attackList.Count == 0)
                {
                    attackList.Add(heroList[targetArr[2]]);
                    attackList.Add(heroList[targetArr[3]]);
                    attackList = GetHeroSurvivalList(attackList);
                    if (attackList.Count == 0)
                    {
                        attackList.Add(heroList[targetArr[4]]);
                        attackList = GetHeroSurvivalList(attackList);
                    }
                }

                break;
        }

        if (attackList.Count == 0)
        {
            Debugger.LogError("没有查询到有效攻击对象");
        }

        return attackList;
    }

    private static List<HeroLogic> GetFrontRowHeroList(List<HeroLogic> heroList)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        attackList.Add(heroList[0]);
        attackList.Add(heroList[1]);
        attackList.Add(heroList[2]);
        return attackList;
    }

    /// <summary>
    /// 获取后排英雄
    /// </summary>
    /// <param name="heroList"></param>
    /// <returns></returns>
    private static List<HeroLogic> GetBackRowHeroList(List<HeroLogic> heroList)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        attackList.Add(heroList[^1]);
        attackList.Add(heroList[^2]);
        return attackList;
    }

    /// <summary>
    /// 获取存活的英雄
    /// </summary>
    /// <param name="heroList"></param>
    /// <returns></returns>
    private static List<HeroLogic> GetHeroSurvivalList(List<HeroLogic> heroList)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        foreach (var item in heroList)
        {
            if (item.objectState == LogicObjectState.Survival)
            {
                attackList.Add(item);
            }
        }

        return attackList;
    }
}