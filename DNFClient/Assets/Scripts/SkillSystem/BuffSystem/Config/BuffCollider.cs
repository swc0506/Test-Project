using FixIntPhysics;
using FixMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZMGC.Battle;

/// <summary>
/// 只处理buff碰撞体的生成、对目标的检测，不处理buff的伤害计算逻辑
/// 只对外提供碰撞检测的API
/// </summary>
public class BuffCollider  
{
    /// <summary>
    /// 当前碰撞体实例
    /// </summary>
    private ColliderBehaviour mBuffCollider;
    /// <summary>
    /// Buff配置
    /// </summary>
    private BuffConfig mBuffCfg;
    /// <summary>
    /// Buff伤害配置
    /// </summary>
    private SkillDamageConfig mDamageCfg;

    /// <summary>
    /// Buff释放者
    /// </summary>
    private LogicActor mReleaser;

    /// <summary>
    /// Buff附加目标
    /// </summary>
    private LogicActor mAttachTarget;
    /// <summary>
    /// 当前Buff所属技能
    /// </summary>
    private Skill mSKill;

    //初始化碰撞体相关数据
    public BuffCollider(Buff buff)
    {
        mBuffCfg = buff.BuffCfg;
        mDamageCfg = buff.BuffCfg.targetConfig.damageCfg;
        mReleaser = buff.releaser;
        mAttachTarget = buff.attachTarget;
        mSKill = buff.skill;
    }

    //1.生成对应的碰撞体

    /// <summary>
    /// 创建碰撞体
    /// </summary>
    public ColliderBehaviour CreateOrUpdateCollider(LogicObject followObj = null)
    {
        //创建对应的多边形碰撞体
        if (mDamageCfg.detectionMode == DamageDetectionMode.BOX3D)
        {
            FixIntVector3 boxSize = new FixIntVector3(mDamageCfg.boxSize);
            FixIntVector3 offset = new FixIntVector3(mDamageCfg.boxOffset) ;

            //负数y轴偏移只做向上界的偏移
            offset.y = FixIntMath.Abs(offset.y);
            if (mBuffCollider == null)
                mBuffCollider = new FixIntBoxCollider(boxSize, offset);

            mBuffCollider.SetBoxData(offset, boxSize);
            mBuffCollider.UpdateColliderInfo(GetBuffPos(), boxSize);
        }
        else if (mDamageCfg.detectionMode == DamageDetectionMode.Sphere3D)
        {
            FixIntVector3 offset = new FixIntVector3(mDamageCfg.sphereOffset);
            //负数y轴偏移只做向上界的偏移
            offset.y = FixIntMath.Abs(offset.y);

            if (mBuffCollider == null)
                mBuffCollider = new FixIntSphereCollider(mDamageCfg.raduis, offset);

            mBuffCollider.SetBoxData(mDamageCfg.raduis, offset);
            mBuffCollider.UpdateColliderInfo(GetBuffPos(), FixIntVector3.zero, mDamageCfg.raduis);
        }
        return mBuffCollider;
    }



    //2.检测碰撞体，检测判断向目标发出碰撞
    public List<LogicActor> CacleColliderTargetObjects()
    {
        // //1.获取所有目标列表 例如 英灵
        // List<LogicActor> enemyList = BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>().GetEnemyList(mReleaser.ObjectType);
        //
        // //2.通过碰撞检测逻辑去检测碰撞体的敌人
        // List<LogicActor> damageTargetList = new List<LogicActor>();
        // foreach (var target in enemyList)
        // {
        //     if (mBuffCollider.ColliderType == ColliderType.Box)
        //     {
        //         //如果这个值为True，说明两个碰撞体发生了碰撞
        //         if (PhysicsManager.IsCollision(mBuffCollider as FixIntBoxCollider, target.Collider))
        //         {
        //             damageTargetList.Add(target);
        //         }
        //     }
        //     else if (mBuffCollider.ColliderType == ColliderType.Shpere)
        //     {
        //         //如果这个值为True，说明两个碰撞体发生了碰撞
        //         if (PhysicsManager.IsCollision(target.Collider, mBuffCollider as FixIntSphereCollider))
        //         {
        //             damageTargetList.Add(target);
        //         }
        //     }
        // }
        return null;
    }
    /// <summary>
    /// 获取Buff所在位置
    /// </summary>
    /// <returns></returns>
    public FixIntVector3 GetBuffPos()
    {
        if (mBuffCfg.attachType== BuffAttachType.Guide_Pos)
        {
            return mSKill.sKillGuidePos;
        }
        else if (mBuffCfg.attachType == BuffAttachType.Creator)
        {
            return mReleaser.LogicPos;
        }
        else if (mBuffCfg.attachType == BuffAttachType.Target)
        {
            return mAttachTarget.LogicPos;
        }
        else
        {
            return mReleaser.LogicPos;
        }
    }

    //3.释放当前碰撞体
    public void OnRelease()
    {
        mBuffCollider?.OnRelease();
        mBuffCollider = null;
    }
}
