using FixMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Skill
{
    //当前所有子弹累计时间列表
    private List<int> mCurCreateBulletAccTimeList = new List<int>();
    /// <summary>
    /// 逻辑随机数生成器
    /// </summary>
    private LogicRandom mLogicRandom;
    /// <summary>
    /// 初始化子弹相关数据
    /// </summary>
    public void OnBulletInit()
    {
        mLogicRandom = new LogicRandom(10);
        if (mSkillData.bulletCfgList != null && mSkillData.bulletCfgList.Count > 0)
        {
            for (int i = 0; i < mSkillData.bulletCfgList.Count; i++)
            {
                mCurCreateBulletAccTimeList.Add(0);
            }
        }
    }
    public void OnLogicFrameUpdateBullet()
    {
        if (mSkillData.bulletCfgList != null && mSkillData.bulletCfgList.Count > 0)
        {

            for (int i = 0; i < mSkillData.bulletCfgList.Count; i++)
            {
                mCurCreateBulletAccTimeList[i]+= LogicFrameConfig.LogicFrameIntervalms; 
                SkillBulletConfig item=  mSkillData.bulletCfgList[i];
                if (item.triggerFrame == mCurLogicFrame)
                {
                    //需要创建子弹
                    CreateBullet(item);
                }
                //判断子弹是否循环创建
                if (item.isLoopCreate)
                {
                    //加强校验错误处理
                    if (item.loopIntervalMs==0)
                    {
                        Debug.LogError("item.loopIntervalMs == 0,不进行子弹循环创建");
                        continue;
                    }
                    //当前所有子弹累计时间超过了创建子弹的间隔就创建子弹
                    while (mCurCreateBulletAccTimeList[i] >= item.loopIntervalMs)
                    {
                        CreateBullet(item);
                        mCurCreateBulletAccTimeList[i] -= item.loopIntervalMs;
                    }
                }
            }
        }
    }
  
    /// <summary>
    /// 创建子弹
    /// </summary>
    /// <param name="config">创建子弹的配置</param>
    public void CreateBullet(SkillBulletConfig config)
    {
        //简单处理，不对框架的功能做适配，因为不用深入学习，可以让子弹简单，移植到其他项目方便。
        //如果通过资源框架进行编写，如果通过资源框架就行管理。
        GameObject bulletObj = GameObject.Instantiate(config.bulletPrefab);

        //创建渲染器
        SkillBulletRender bulletRender= bulletObj.GetComponent<SkillBulletRender>();
        if (bulletRender==null)
        {
            bulletRender= bulletObj.AddComponent<SkillBulletRender>();
        }

        FixIntVector3 rangePos = FixIntVector3.zero;
        if (config.isLoopCreate)
        {
            //随机xyz轴偏移量
            FixInt x = mLogicRandom.Range(config.minrandomRangeVect3.x, config.maxRandomRangeVect3.x);
            FixInt y = mLogicRandom.Range(config.minrandomRangeVect3.y, config.maxRandomRangeVect3.y);
            FixInt z = mLogicRandom.Range(config.minrandomRangeVect3.z, config.maxRandomRangeVect3.z);
            rangePos = new FixIntVector3(x, y, z);
        }
        //创建逻辑类
        SKillBulletLogic bulletLogic = new SKillBulletLogic(this,mSkillCreater, bulletRender, config, rangePos);
        bulletRender.SetRenderData(bulletLogic,config);
        mSkillCreater.AddBullet(bulletLogic);
    }

    public void OnBulletRelease()
    {
        mCurCreateBulletAccTimeList.Clear();
        mLogicRandom=null;
    }
}
