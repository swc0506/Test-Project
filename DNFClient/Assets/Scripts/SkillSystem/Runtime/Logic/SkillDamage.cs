using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FixIntPhysics;
using FixMath;
using ZMGC.Battle;

/// <summary>
    /// 伤害来源
/// </summary>
public enum DamageSource
{
    None,
    SKill,//技能伤害
    Buff,//Buff伤害
    Bullet,//子弹伤害
}
public partial class Skill
{
    /// <summary>
    /// 碰撞器字典，key为碰撞配置的HashCode，Value为生成的对应碰撞器
    /// </summary>

    private Dictionary<int, ColliderBehaviour> mColliderDic = new Dictionary<int, ColliderBehaviour>();
    /// <summary>
    /// 当前伤害累计时间
    /// </summary>
    private int mCurDamageAccTime;
    /// <summary>
    /// 逻辑帧更新伤害
    /// </summary>
    public void OnLogicFrameUpdateDamage()
    {
        //判断当前伤害配置列表是否为空，以及数量是否大于0
        if (mSkillData.damageCfgList!=null&&mSkillData.damageCfgList.Count>0)
        {
            foreach (var item in mSkillData.damageCfgList)
            {
                int hashCode= item.GetHashCode();
                if (item.colliderPosType== ColliderPosType.FollowPos)
                {
                    ColliderBehaviour damageCollider = null;
                    //更新碰撞器位置
                    if (mColliderDic.TryGetValue(item.GetHashCode(), out damageCollider) && damageCollider != null)
                    {
                        CreateOrUpdateCollider(item, damageCollider);
                    }
                }
              
                //创建碰撞器
                if (mCurLogicFrame == item.triggerFrame)
                {
                    DestroyCollider(item);
                    ColliderBehaviour collider = CreateOrUpdateCollider(item,null);
                    //加入字典缓存当前碰撞器
                    mColliderDic.Add(hashCode, collider);
                    if (item.triggerIntervalMs == 0)
                    {
                        //触发一次伤害//TODO
                        if (mColliderDic.ContainsKey(hashCode))
                        {
                            TriggerColliderDamage(mColliderDic[hashCode], item);
                        }
                    }
                }

                //配置碰撞器伤害间隔
                if (item.triggerIntervalMs != 0)
                {
                    mCurDamageAccTime += LogicFrameConfig.LogicFrameIntervalms;
                    //如果当前累计时间大于触发伤害间隔就触发伤害间隔
                    if (mCurDamageAccTime>=item.triggerIntervalMs)
                    {
                        //触发一次伤害//TODO
                        mCurDamageAccTime = 0;
                        if (mColliderDic.ContainsKey(hashCode))
                        {
                            TriggerColliderDamage(mColliderDic[hashCode],item);
                        }
                    }
                }
                
                //销毁碰撞器
                if (item.endFrame==mCurLogicFrame)
                {
                    DestroyCollider(item);
                }
            }
        }
    }
    /// <summary>
    /// 触发碰撞器伤害
    /// </summary>
    public void TriggerColliderDamage(ColliderBehaviour collider,SkillDamageConfig config)
    {
        //1.获取敌对目标列表 过滤 英雄
        // List<LogicActor> enemyList= BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>().GetEnemyList(mSkillCreater.ObjectType);
        //
        // //2.通过碰撞器逻辑去判断碰撞到的敌人
        // List<LogicActor> damageTargetList = new List<LogicActor>();
        // foreach (var target in enemyList)
        // {
        //     if (collider.ColliderType== ColliderType.Box)
        //     {
        //         //如果返回值为True，说明与碰撞器发生了碰撞
        //         if (PhysicsManager.IsCollision(collider as FixIntBoxCollider, target.Collider))
        //         {
        //             damageTargetList.Add(target);
        //         }
        //     }
        //     else if (collider.ColliderType== ColliderType.Shpere)
        //     {
        //         //如果返回值为True，说明与碰撞器发生了碰撞
        //         if (PhysicsManager.IsCollision(target.Collider,collider as FixIntSphereCollider))
        //         {
        //             damageTargetList.Add(target);
        //         }
        //     }
        // }
        //释放列表
        // enemyList.Clear();
        // //3.获取受伤目标后，对这些目标造成伤害
        // foreach (var target in damageTargetList)
        // {
        //     //造成伤害
        //     target.SkillDamage(9999, config);
        //
        //     //添加Buff
        //     if (config.addBuffs!=null&&config.addBuffs.Length>0)
        //     {
        //         foreach (var buffid in config.addBuffs)
        //         {
        //             BuffSystem.Instance.AttachBuff(buffid,mSkillCreater,target,this,null);
        //         }
        //     }
        //     
        //     //添加受击特效
        //     AddHitEffect(target);
        //
        //     //播放受击音效
        //     PlayHitAudio();
        // }
    }
    /// <summary>
    /// 添加受击特效
    /// </summary>
    public void AddHitEffect(LogicActor targetObj)
    {
        if (mSkillData.skillCfg.skillHitEffect!=null)
        {
            targetObj.OnHit(mSkillData.skillCfg.skillHitEffect, mSkillData.skillCfg.hitEffectSurvivalTimeMs,mSkillCreater);
        }
    }
    /// <summary>
    /// 创建碰撞器
    /// </summary>
    public ColliderBehaviour CreateOrUpdateCollider(SkillDamageConfig item, ColliderBehaviour damageCollider,LogicObject followObj=null)
    {
        ColliderBehaviour collider= damageCollider;
        LogicObject followTragetObj = followObj == null ? mSkillCreater : followObj;
         //生成对应的矩形或球形碰撞器
        if (item.detectionMode== DamageDetectionMode.BOX3D)
        {
            FixIntVector3 boxSize= new FixIntVector3(item.boxSize);
            FixIntVector3 offset = new FixIntVector3(item .boxOffset)* followTragetObj.LogicXAxis;

            //这里y轴偏移只允许向上发生偏移
            offset.y = FixIntMath.Abs(offset.y);
            if (damageCollider==null)
                collider = new FixIntBoxCollider(boxSize, offset);

            collider.SetBoxData(offset, boxSize);
            collider.UpdateColliderInfo(followTragetObj.LogicPos,boxSize);
        }
        else if (item.detectionMode== DamageDetectionMode.Sphere3D)
        {
            FixIntVector3 offset = new FixIntVector3(item.sphereOffset) * followTragetObj.LogicXAxis;
            //这里y轴偏移只允许向上发生偏移
            offset.y = FixIntMath.Abs(offset.y);

            if (damageCollider == null)
                collider = new FixIntSphereCollider(item.raduis, offset);

            collider.SetBoxData(item.raduis, offset);
            collider.UpdateColliderInfo(followTragetObj.LogicPos, FixIntVector3.zero,item.raduis);
        }
        return collider;
    }



    /// <summary>
    /// 销毁对应配置生成的碰撞器
    /// </summary>
    /// <param name="item"></param>
    public void DestroyCollider(SkillDamageConfig  item)
    {
        ColliderBehaviour collider = null;
        int hashCode = item.GetHashCode();
        mColliderDic.TryGetValue(hashCode, out collider);
        if (collider != null)
        {
            mColliderDic.Remove(hashCode);
            collider.OnRelease();
        }
    }
}
