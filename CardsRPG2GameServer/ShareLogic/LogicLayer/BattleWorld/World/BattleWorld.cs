using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicLayer;

public class BattleWorld
{
    public static bool battleEnd = false;
    public HeroLogicCtrl heroLogicCtrl;
    public RoundLogicCtrl roundLogicCtrl;

    public int quickenMultiple = 1; // 加速倍数
    private int maxQuickenMultiple = 3; // 最大加速倍数
    public bool battlePause; //战斗暂停

    private float mAccLogicRunTime; // 累计逻辑运行时间
    private float mNextLogicFrameTime; // 下一个逻辑帧时间
    public static float deltaTime; // 动画缓动时间
    public int battleId;
    public bool isWin;
    public Action<BattleWorld> OnBattleEndCallBack;

    /// <summary>
    /// 战斗世界创建
    /// </summary>
    public void CreateWorld(List<HeroData> heroList, List<HeroData> enemyList, int randomSeed, int battleId,
        Action<BattleWorld> battleEndCallback = null)
    {
        OnBattleEndCallBack = battleEndCallback;
        LogicRandom.Instance.InitRandom(randomSeed);
        heroLogicCtrl = new HeroLogicCtrl();
        roundLogicCtrl = new RoundLogicCtrl();

        this.battleId = battleId;
        heroLogicCtrl.OnCreate(heroList, enemyList);
        roundLogicCtrl.OnCreate();
        battleEnd = false;
        quickenMultiple = 1;
        deltaTime = 0;
        LogicFrameSyncConfig.logicFrameId = 0;

#if CLIENT_LOGIC
        BattleDataModel dataModel = new BattleDataModel
            { heroList = heroList, enemyList = enemyList, battleSite = randomSeed, battleId = battleId };
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataModel);
        PlayerPrefs.SetString(BattleDataModel.key, json);
#endif
    }

    public void OnUpdate()
    {
        if (battleEnd || battlePause)
            return;

#if CLIENT_LOGIC
        mAccLogicRunTime += Time.deltaTime;
        // 控制帧数， 保证所有设备的逻辑帧帧数的一致性
        while (mAccLogicRunTime >= mNextLogicFrameTime)
        {
            OnLogicFrameUpdate();
            mNextLogicFrameTime += LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL;
            LogicFrameSyncConfig.logicFrameId++;
        }

        deltaTime = (mAccLogicRunTime + LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL - mNextLogicFrameTime) /
                    LogicFrameSyncConfig.LOGIC_FRAME_INTERVAL;
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
        ActionManager.Instance?.OnLogicFrameUpdate();
        LogicTimerManager.Instance?.OnLogicFrameUpdate();
        BulletManager.Instance?.OnLogicFrameUpdate();
        BuffManager.Instance?.OnLogicFrameUpdate();
    }

    public void BattlePause()
    {
#if CLIENT_LOGIC
        battlePause = !battlePause;
        Time.timeScale = battlePause ? 0 : quickenMultiple;
#endif
    }

    /// <summary>
    /// 战斗加速
    /// </summary>
    public void QuickenBattle()
    {
#if CLIENT_LOGIC
        quickenMultiple++;
        if (quickenMultiple > maxQuickenMultiple)
        {
            quickenMultiple = 1;
        }

        Time.timeScale = quickenMultiple;
#endif
    }

    /// <summary>
    /// 战斗结束
    /// </summary>
    /// <param name="isWin"></param>
    public void BattleEnd(bool isWin)
    {
        Debugger.Log("BattleEnd");
        string heroStr = "";
        for (int i = 0; i < heroLogicCtrl.allList.Count; i++)
        {
            HeroLogic hero = heroLogicCtrl.allList[i];
            heroStr += hero.Id + " hero Hp: " + hero.Hp + " 怒气值: " + hero.Rage + " IsBeControl: " +
                       hero.IsBeControl() + "\n";
        }

        Debugger.Log("战斗结束 战斗数据： \n所有英雄生命值：\n" + heroStr);
        battleEnd = true;
        this.isWin = isWin;
        //可以根据本地计算结果与服务端进行校验
        OnBattleEndCallBack?.Invoke(this);
#if CLIENT_LOGIC
        BattleWorldNodes.Instance.battleResultWindow.SetBattleResult(isWin);
#endif
    }

    public void DestroyWorld()
    {
        heroLogicCtrl.OnDestroy();
        roundLogicCtrl.OnDestroy();
        SkillManager.Instance.OnDestroy();
        LogicTimerManager.Instance.OnDestroy();
        ActionManager.Instance.OnDestroy();
        BulletManager.Instance.OnDestroy();
        BuffManager.Instance.OnDestroy();
    }
}