using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWorld
{
    public HeroLogicCtrl heroLogicCtrl;
    public RoundLogicCtrl roundLogicCtrl;
    
    private float mAccLogicRunTime; // 累计逻辑运行时间
    private float mNextLogicFrameTime; // 下一个逻辑帧时间
    private float deltaTime; // 动画缓动时间
    
    /// <summary>
    /// 战斗世界创建
    /// </summary>
    public void CreateWorld(List<HeroData> heroList, List<HeroData> enemyList)
    {
        heroLogicCtrl = new HeroLogicCtrl();
        roundLogicCtrl = new RoundLogicCtrl();
        
        heroLogicCtrl.OnCreate(heroList, enemyList);
        roundLogicCtrl.OnCreate();
    }
    
    public void OnUpdate()
    {
#if CLIENT_LOGIC
        mAccLogicRunTime += Time.deltaTime;
        // 控制帧数， 保证所有设备的逻辑帧帧数的一致性
        while (mAccLogicRunTime >= mNextLogicFrameTime)
        {
            OnLogicFrameUpdate();
            mNextLogicFrameTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL;
            LogicFrameSyncConfig.logicFrameId++;
        }
        
        deltaTime = (mAccLogicRunTime + LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL - mNextLogicFrameTime) / LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL;
#else
        OnLogicFrameUpdate();
#endif
    }

    /// <summary>
    /// 逻辑帧更新
    /// </summary>
    public void OnLogicFrameUpdate()
    {
        heroLogicCtrl?.OnLogicFrameUpdate();
        roundLogicCtrl?.OnLogicFrameUpdate();
        ActionManager.Instance.OnLogicFrameUpdate();
    }
    
    public void DestroyWorld()
    {
        heroLogicCtrl.OnDestroy();
        roundLogicCtrl.OnDestroy();
    }
}
