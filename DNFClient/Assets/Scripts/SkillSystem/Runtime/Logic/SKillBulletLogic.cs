using FixIntPhysics;
using FixMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZMGC.Battle;
public class SKillBulletLogic : LogicObject
{
    /// <summary>
    /// 子弹所属技能
    /// </summary>
    private Skill mSKill;
    /// <summary>
    /// 子弹发射者
    /// </summary>
    private LogicActor mFireLogicActor;
    /// <summary>
    /// 子弹配置
    /// </summary>
    private SkillBulletConfig mBulletCfg;

    /// <summary>
    /// 当前子弹碰撞体
    /// </summary>
    private ColliderBehaviour mBulletCollider;
    //当前逻辑帧
    private int mCurLogicFrame = 0;
    //当前逻辑帧累计时间
    private int mCurLogicFrameAccTime;
    //子弹是否命中目标
    private bool mBulletIsHit=false;
    //是否失效
    public bool isFailure = false;   
    //子弹当前帧命中目标列表
    private List<LogicActor> mHitTargetList = new List<LogicActor>();
    public SKillBulletLogic(Skill skill, LogicActor fireLogicActor, RenderObject renderObj, SkillBulletConfig bulletCfg, FixIntVector3 rangePos)
    {
        mSKill = skill;
        mFireLogicActor = fireLogicActor;
        RenderObj = renderObj;
        mBulletCfg = bulletCfg;
        //帧同步中处理，这里不用处理  1.如果使用单例的话 对不起，使用弧线？子弹轨迹？
        //什么鬼 看策划 PPT
        //初始化自身位置
      
        //逻辑朝向
        LogicXAxis = fireLogicActor.LogicXAxis;
        //初始化逻辑朝向数据
        LogicDir = new FixIntVector3(LogicXAxis,0,0);
        //初始化当前子弹偏移位置
        FixIntVector3 pos = LogicXAxis * (new FixIntVector3(mBulletCfg.offset) + rangePos);
        pos.y= FixIntMath.Abs(pos.y);
        LogicPos = fireLogicActor.LogicPos + pos;
      
        //设置旋转角度
        LogicAngle = new FixIntVector3(bulletCfg.angle);
        
        //当前子弹是否附加伤害
        if (bulletCfg.isAttachDamage)
        {
            SkillDamageConfig damageCfg = bulletCfg.damageCfg;
            if (damageCfg.detectionMode== DamageDetectionMode.BOX3D)
            {
                mBulletCollider = new FixIntBoxCollider(FixIntVector3.zero,FixIntVector3.zero);
            }
            else if (damageCfg.detectionMode == DamageDetectionMode.Sphere3D)
            {
                mBulletCollider = new FixIntSphereCollider(damageCfg.raduis, FixIntVector3.zero);
            }
        }
    }
    /// <summary>
    /// 处理子弹的碰撞和子弹的移动逻辑
    /// </summary>
    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
        //计算逻辑帧累计时间
        mCurLogicFrameAccTime = mCurLogicFrame * LogicFrameConfig.LogicFrameIntervalms;
        //逻辑帧递增
        mCurLogicFrame++;
       
            //子弹命中逻辑
        foreach (LogicActor target in mHitTargetList)
        {
            //造成子弹伤害
            target.BulletDamage(9999,mBulletCfg.damageCfg);
            //播放击中特效
            target.OnHit(mBulletCfg.hitEffect,mBulletCfg.hitEffectSurvivalTimems,this);
            //播放命中音效
            if (mBulletCfg.hitAudio!=null)
            {
                AudioController.GetInstance().PlaySoundByAudioClip(mBulletCfg.hitAudio,false,1);
            }
            //设置子弹附加的Buff
            AttachBuff(target);

            if (mBulletCfg.isHitDestory)
            {
                Release();
                break;
            }
        }
        //等目标处理完毕，清空命中目标列表
        if (mHitTargetList.Count>0)
        {
            mHitTargetList.Clear();
        }
        //子弹碰撞体位置更新
        if (mBulletCollider != null)
        {
            if (mBulletCfg.damageCfg.colliderPosType== ColliderPosType.FollowPos)
            {
                //设置子弹碰撞体位置
                if (mBulletCfg.damageCfg.detectionMode== DamageDetectionMode.BOX3D)
                {
                    FixIntVector3 offset = LogicXAxis * new FixIntVector3(mBulletCfg.damageCfg.boxOffset);
                    mBulletCollider.SetBoxData(offset,new FixIntVector3(mBulletCfg.damageCfg.boxSize));
                    mBulletCollider.UpdateColliderInfo(LogicPos, new FixIntVector3(mBulletCfg.damageCfg.boxSize));
                }
                else if (mBulletCfg.damageCfg.detectionMode == DamageDetectionMode.Sphere3D)
                {
                    FixIntVector3 offset = LogicXAxis * new FixIntVector3(mBulletCfg.damageCfg.sphereOffset);
                    mBulletCollider.SetBoxData(mBulletCfg.damageCfg.raduis, offset);
                    mBulletCollider.UpdateColliderInfo(LogicPos,FixIntVector3.zero, mBulletCfg.damageCfg.raduis);
                }
            }

            // //获取所有场景中的敌人
            // List<LogicActor> enemyList= BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>().GetEnemyList(mFireLogicActor.ObjectType);
            // //检测子弹的碰撞器是否命中了敌人
            // foreach (var target in enemyList)
            // {
            //     if (mBulletCfg.damageCfg.detectionMode == DamageDetectionMode.BOX3D)
            //     {
            //        mBulletIsHit= PhysicsManager.IsCollision(mBulletCollider as FixIntBoxCollider, target.Collider);
            //     }
            //     else if (mBulletCfg.damageCfg.detectionMode == DamageDetectionMode.Sphere3D)
            //     {
            //         mBulletIsHit = PhysicsManager.IsCollision(target.Collider, mBulletCollider as FixIntSphereCollider);
            //     }
            //     //把命中的目标加入命中列表
            //     if (mBulletIsHit)
            //     {
            //         mHitTargetList.Add(target);
            //     }
            //}
        }

        //子弹位置更新
        LogicPos += LogicDir * (FixInt)mBulletCfg.moveSpeed * (FixInt)LogicFrameConfig.LogicFrameInterval;
        //当前存活时间，达到子弹存活时间就销毁子弹
        if (mCurLogicFrameAccTime>=mBulletCfg.survivalTimeMsg)
        {
            Release();
        }
    }
    /// <summary>
    /// 给子弹附加buff
    /// </summary>
    public void AttachBuff(LogicActor target)
    {

        if (mBulletCfg.damageCfg.addBuffs!=null&& mBulletCfg.damageCfg.addBuffs.Length>0)
        {
            foreach (var buffid in mBulletCfg.damageCfg.addBuffs)
            {
                BuffSystem.Instance.AttachBuff(buffid,mFireLogicActor, target,mSKill);
            }
        }
    }
    public void Release()
    {
        RenderObj.OnRelease();
        mBulletCollider?.OnRelease();
        mBulletCollider = null;
        isFailure = true;
    }
}
