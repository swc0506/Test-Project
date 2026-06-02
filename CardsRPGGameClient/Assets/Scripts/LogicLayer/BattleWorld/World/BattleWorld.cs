using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWorld
{
    public HeroLogicCtrl heroLogicCtrl;
    public RoundLogicCtrl roundLogicCtrl;
    
    private float mAccLogicRunTime; // 累计逻辑运行时间
    private float mNextLogicFrameTime; // 下一个逻辑帧时间
    public static float deltaTime; // 动画缓动时间
    
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debugger.Log("计时开始：" + Time.realtimeSinceStartup);
            heroLogicCtrl.heroLogicList[0].PlayAnim("Attack");
            MoveToAction move = new MoveToAction(heroLogicCtrl.heroLogicList[0],
                heroLogicCtrl.enemyLogicList[0].LogicPosition, new VInt(1000),
                () =>
                {
                    Debugger.Log("计时结束：" + Time.realtimeSinceStartup);
                    Debugger.Log("移动完成pos:" + heroLogicCtrl.heroLogicList[0].LogicPosition);
                    SkillEffect effect = ResourcesManager.Instance.LoadObject<SkillEffect>(AssetPathConfig.SKILL_EFFECT + "Effect_RenMa_hit");
                    effect.SetEffectPos(heroLogicCtrl.enemyLogicList[0].LogicPosition);
                });
            
            ActionManager.Instance.RunAction(move);
            LogicTimerManager.Instance.DelayCall(new VInt(700), () =>
            {
                heroLogicCtrl.enemyLogicList[0].DamageHp(30);
            });
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveToAction move = new MoveToAction(heroLogicCtrl.heroLogicList[0],
                new VInt3(BattleWorldNodes.Instance.heroRootArr[0].position), new VInt(1000),
                () =>
                {
                    Debugger.Log("移动完成pos:" + heroLogicCtrl.heroLogicList[0].LogicPosition);
                });
            ActionManager.Instance.RunAction(move);
        }
    }

    /// <summary>
    /// 逻辑帧更新
    /// </summary>
    public void OnLogicFrameUpdate()
    {
        heroLogicCtrl?.OnLogicFrameUpdate();
        roundLogicCtrl?.OnLogicFrameUpdate();
        ActionManager.Instance.OnLogicFrameUpdate();
        LogicTimerManager.Instance.OnLogicFrameUpdate();
    }
    
    public void DestroyWorld()
    {
        heroLogicCtrl.OnDestroy();
        roundLogicCtrl.OnDestroy();
    }
}
