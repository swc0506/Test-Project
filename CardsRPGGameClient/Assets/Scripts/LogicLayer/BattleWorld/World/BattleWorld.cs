using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// 战斗世界创建
    /// </summary>
    public void CreateWorld(List<HeroData> heroList, List<HeroData> enemyList)
    {
        heroLogicCtrl = new HeroLogicCtrl();
        roundLogicCtrl = new RoundLogicCtrl();

        heroLogicCtrl.OnCreate(heroList, enemyList);
        roundLogicCtrl.OnCreate();
        battleEnd = false;

#if CLIENT_LOGIC
        BattleDataModel dataModel = new BattleDataModel { heroList = heroList, enemyList = enemyList, };
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
                    SkillEffect effect =
                        ResourcesManager.Instance.LoadObject<SkillEffect>(AssetPathConfig.SKILL_EFFECT +
                                                                          "Effect_RenMa_hit");
                    effect.SetEffectPos(heroLogicCtrl.enemyLogicList[0].LogicPosition);
                });

            ActionManager.Instance.RunAction(move);
            LogicTimerManager.Instance.DelayCall(new VInt(700),
                () => { heroLogicCtrl.enemyLogicList[0].DamageHp(30); });
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveToAction move = new MoveToAction(heroLogicCtrl.heroLogicList[0],
                new VInt3(BattleWorldNodes.Instance.heroRootArr[0].position), new VInt(1000),
                () => { Debugger.Log("移动完成pos:" + heroLogicCtrl.heroLogicList[0].LogicPosition); });
            ActionManager.Instance.RunAction(move);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SkillManager.Instance.ReleaseSkill(1010, heroLogicCtrl.heroLogicList[0], true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            heroLogicCtrl.heroLogicList[0].TryClearRage();
            SkillManager.Instance.ReleaseSkill(1011, heroLogicCtrl.heroLogicList[0], false);
            heroLogicCtrl.heroLogicList[0].UpdateAnger(0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            heroLogicCtrl.heroLogicList[3].TryClearRage();
            SkillManager.Instance.ReleaseSkill(1041, heroLogicCtrl.heroLogicList[3], false);
            heroLogicCtrl.heroLogicList[3].UpdateAnger(0);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SkillManager.Instance.ReleaseSkill(1040, heroLogicCtrl.heroLogicList[3], true);
        }
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
        battleEnd = true;
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
    }
}