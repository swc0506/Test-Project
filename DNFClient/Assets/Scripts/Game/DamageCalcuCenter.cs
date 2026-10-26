using FixMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 伤害计算中心
/// </summary>
public class DamageCalcuCenter
{
    /// <summary>
    /// 物理攻击力(武器基础物理攻击X（1+力量/250)
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static FixInt GetADAtck(LogicActor attacker)
    {
        return attacker.AD * (1 + attacker.STR / 250);
    }

    /// <summary>
    /// 魔法攻击力(武器基础物理攻击X（1+力量/250)
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static FixInt GetAPAtck(LogicActor attacker)
    {
        return attacker.AP * (1 + attacker.INT / 250);
    }

    /// <summary>
    /// 获取物理伤害减免 减伤百分比=自身防御/（攻击方等级X200+自身防御）减伤封顶75%
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static FixInt GetADReduction(LogicActor attacker, LogicActor attackTarget)
    {
        FixInt damageReductionRate = attackTarget.ADDef / (attacker.Level * 200 + attackTarget.ADDef);
        return damageReductionRate > 0.75 ? 0.75 : damageReductionRate;
    }

    /// <summary>
    /// 获取魔法伤害减免 减伤百分比=自身防御/（攻击方等级X200+自身防御）减伤封顶75%
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static FixInt GetAPReduction(LogicActor attacker, LogicActor attackTarget)
    {
        FixInt damageReductionRate = attackTarget.APdef / (attacker.Level * 200 + attackTarget.APdef);
        return damageReductionRate > 0.75 ? 0.75 : damageReductionRate;
    }

    /// <summary>
    /// 获取物理暴击伤害 暴击伤害=总伤害X（100%+50%）
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static FixInt GetADPCTDamage(FixInt totalDamage, LogicActor target)
    {
        return totalDamage * (1 + target.PCT);
    }
    /// <summary>
    /// 获取魔法暴击伤害 暴击伤害=总伤害X（100%+50%）
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static FixInt GetAPMCTDamage(FixInt totalDamage, LogicActor target)
    {
        return totalDamage * (1 + target.MCT);
    }
    /// <summary>
    /// 计算总伤害
    /// </summary>
    /// <returns></returns>
    public static FixInt CaclulateDamage(SkillDamageConfig damageCfg, LogicActor attacker, LogicActor attackTarget)
    {
        FixInt finalDamage = FixInt.Zero;
        switch (damageCfg.damageType)
        {
            case DamageType.None:
            case DamageType.ADDamage:
                finalDamage = GetADReduction(attacker, attackTarget) * GetADAtck(attacker);
                break;
            case DamageType.APDamage:
                finalDamage = GetAPReduction(attacker, attackTarget) * GetAPAtck(attacker);
                break;

        }
        return finalDamage * (damageCfg.damageRate / 100);
    }

    /// <summary>
    /// 计算总伤害
    /// </summary>
    /// <returns></returns>
    public static FixInt CaclulateDamage(BuffConfig buffCfg, LogicActor attacker, LogicActor attackTarget)
    {
        FixInt finalDamage = FixInt.Zero;
        DamageType damageType = buffCfg.targetConfig.isOpen ? buffCfg.targetConfig.damageCfg.damageType : buffCfg.damageType;
        switch (damageType)
        {
            case DamageType.None:
            case DamageType.ADDamage:
                finalDamage = GetADReduction(attacker, attackTarget) * GetADAtck(attacker);
                break;
            case DamageType.APDamage:
                finalDamage = GetAPReduction(attacker, attackTarget) * GetAPAtck(attacker);
                break;

        }
        return finalDamage * (buffCfg.targetConfig.isOpen ? (buffCfg.targetConfig.damageCfg.damageRate / 100) : finalDamage * (buffCfg.damageRate / 100));
    }

}
