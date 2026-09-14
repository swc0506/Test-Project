using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicLayer;
using ZMGC.Hall;
#if CLIENT_LOGIC
using ZM.UI;
using ZM.ZMAsset;
#endif

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
    public long battleId;
    public bool IsWin { get; set; }
    public Action<BattleWorld> OnBattleEndCallBack;

    /// <summary>
    /// 是否战斗回放
    /// </summary>
    public bool IsPlayBack { get; set; }

    /// <summary>
    /// 是否自动战斗
    /// </summary>
    public bool IsAutoBattle { get; private set; }

    private GameObject cloneObj;

#if CLIENT_LOGIC
    public BattleRoot3D Root3D { get; private set; }

    /// <summary>
    /// 所有战斗相关的窗口列表
    /// </summary>
    private List<WindowBase> mAllBattleWindowList = new List<WindowBase>();
#endif

    /// <summary>
    /// 战斗世界创建
    /// </summary>
    public void CreateWorld(List<HeroData> heroList, List<HeroData> enemyList, int randomSeed, long battleId,
        Action<BattleWorld> battleEndCallback = null, List<HeroSkillInputData> skillInputList = null,
        bool isPlayBack = false)
    {
        OnBattleEndCallBack = battleEndCallback;
        LogicRandom.Instance.InitRandom(randomSeed);
        heroLogicCtrl = new HeroLogicCtrl();
        roundLogicCtrl = new RoundLogicCtrl();
        this.battleId = battleId;
        IsPlayBack = isPlayBack;
        battleEnd = false;
        quickenMultiple = 1;
        deltaTime = 0;
        LogicFrameSyncConfig.logicFrameId = 0;
#if CLIENT_LOGIC
        MsgHandleCenter.Instance.OnCreate();
        UIEventControl.DispensEvent(UIEventEnum.SwitchInBattle);
        UIEventControl.AddEvent(UIEventEnum.SwitchOutBattle,SwitchOutBattle);
        UIEventControl.AddEvent(UIEventEnum.SwitchInBattle,SwitchInBattle);
        BattleDataModel dataModel = new BattleDataModel
            { heroList = heroList, enemyList = enemyList, battleSite = randomSeed, battleId = battleId };
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataModel);
        PlayerPrefs.SetString(BattleDataModel.key, json);
        CreateRenderEnv(heroList);
#endif
        heroLogicCtrl.OnCreate(heroList, enemyList);
        heroLogicCtrl.CacheClientInputSkillData(skillInputList);
        roundLogicCtrl.OnCreate();
    }

    private void CreateRenderEnv(List<HeroData> heroList)
    {
#if CLIENT_LOGIC
        var battleRoot = ZMAsset.InstantiateObject($"{AssetsPathConfig.HALL_PREFABS_PATH}Battle/3DBattleRoot", null);
        Root3D = battleRoot.GetComponent<BattleRoot3D>();
        Root3D.LoadMap("Map3");

        mAllBattleWindowList.Add(UIModule.Instance.PopUpWindow<ZM.UI.HUDWindow>());
        mAllBattleWindowList.Add(UIModule.Instance.PopUpWindow<ZM.UI.RoundWindow>().InitViewState(heroList));
        mAllBattleWindowList.Add(UIModule.Instance.PopUpWindow<ZM.UI.SkillWindow>());
        UIModule.Instance.PopUpWindow<ZM.UI.HallButtonsWidow>().InitView(MainTabEnum.Battle);
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

    /// <summary>
    /// 切出战斗
    /// </summary>
    private void SwitchOutBattle(object data)
    {
        if (battleEnd) return;
        Debugger.Log("切出战斗");
        //关闭战斗摄像机的渲染
        Root3D.battleCamera.enabled = false;
        //伪隐藏窗口
        foreach (var item in mAllBattleWindowList)
        {
            item.PseudoHidden(0);
        }
    }

    /// <summary>
    /// 切入战斗
    /// </summary>
    private void SwitchInBattle(object data)
    {
        if (battleEnd) return;
        Debugger.Log("切入战斗");
        //取消伪隐藏窗口
        foreach (var item in mAllBattleWindowList)
        {
            item.PseudoHidden(1);
        }

        //关闭战斗摄像机的渲染
        Root3D.battleCamera.enabled = true;
    }

    public void SetAutoBattle(bool isAuto)
    {
        IsAutoBattle = isAuto;
        Debugger.Log("SetAutoBattle " + isAuto);
    }

    public bool BattlePause()
    {
#if CLIENT_LOGIC
        battlePause = !battlePause;
        Time.timeScale = battlePause ? 0 : quickenMultiple;
        return battlePause;
#endif
        return false;
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

    public void JumpBattle()
    {
        if (IsPlayBack)
        {
            ReplayBattleEnd();
            return;
        }

        MsgHandleCenter.Instance.SendBattleResultRequest(BattleWorldManager.BattleWorld.battleId,
            BattleWorldManager.BattleWorld.heroLogicCtrl.skillInputDataList);
    }

    /// <summary>
    /// 回放战斗结束
    /// </summary>
    public void ReplayBattleEnd()
    {
        ReplayData data = HallWorld.GetExitsDataMgr<LevelDataMgr>().GetReplayData(battleId);
        if (data == null)
        {
            Debugger.LogError("战斗回放数据获取错误，战斗id:" + battleId);
            return;
        }

        BattleEnd(new BattleResultResponse() { isWin = data.isWin });
    }

    /// <summary>
    /// 战斗结束
    /// </summary>
    /// <param name="response"></param>
    public void BattleEnd(BattleResultResponse response)
    {
        IsWin = response.isWin;
        Debugger.Log("BattleEnd IsWin" + IsWin);
        string heroStr = "";
        for (int i = 0; i < heroLogicCtrl.allList.Count; i++)
        {
            HeroLogic hero = heroLogicCtrl.allList[i];
            heroStr += hero.Id + " hero Hp: " + hero.Hp + " 怒气值: " + hero.Rage + " IsBeControl: " +
                       hero.IsBeControl() + "\n";
        }

        Debugger.Log("战斗结束 战斗数据： \n所有英雄生命值：\n" + heroStr);
        battleEnd = true;

        //可以根据本地计算结果与服务端进行校验
        OnBattleEndCallBack?.Invoke(this);
#if CLIENT_LOGIC
        //BattleWorldNodes.Instance.battleResultWindow.SetBattleResult(isWin);
        if (IsWin)
        {
            UIModule.Instance.PopUpWindow<BattleWinWindow>().InitView(response.rewardList);
        }
        else
        {
            UIModule.Instance.PopUpWindow<BattleLoosWindow>();
        }
#endif
    }

    public void DestroyWorld()
    {
        UIEventControl.RemoveEvent(UIEventEnum.SwitchOutBattle,SwitchOutBattle);
        UIEventControl.RemoveEvent(UIEventEnum.SwitchInBattle,SwitchInBattle);
        heroLogicCtrl.OnDestroy();
        roundLogicCtrl.OnDestroy();
        mAllBattleWindowList.Clear();
        SkillManager.Instance.OnDestroy();
        LogicTimerManager.Instance.OnDestroy();
        ActionManager.Instance.OnDestroy();
        BulletManager.Instance.OnDestroy();
        BuffManager.Instance.OnDestroy();
#if CLIENT_LOGIC
        MsgHandleCenter.Instance.OnDestroy();
#endif
    }
}