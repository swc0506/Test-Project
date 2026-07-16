using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundLogicCtrl : ILogicBehaviour
{
    /// <summary>
    /// 回合id
    /// </summary>
    public int RoundId { get; private set; }

    /// <summary>
    /// 最大回合id
    /// </summary>
    public int MaxRoundId { get; private set; }

    /// <summary>
    /// 出手队列
    /// </summary>
    private Queue<HeroLogic> mHeroAttackQueue = new Queue<HeroLogic>();

    private HeroLogicCtrl mHeroLogicCtrl;

    public void OnCreate()
    {
        mHeroLogicCtrl = WorldManager.BattleWorld.heroLogicCtrl;
#if RENDER_LOGIC
        BattleWorldNodes.Instance.roundWindow.RoundStart(RoundId);
#endif
        LogicTimerManager.Instance.DelayCall(2000, NextRoundStart);
    }

    public void NextRoundStart()
    {
        RoundId++;
#if RENDER_LOGIC
        BattleWorldNodes.Instance.roundWindow.NextRound(RoundId);
#endif
        //计算英雄出手顺序
        foreach (var logic in mHeroLogicCtrl.allList)
        {
            logic.RoundStarEvent(RoundId);
        }

        mHeroAttackQueue = mHeroLogicCtrl.CalcAttackSort();
        StartNextHeroAttack();
    }

    public void StartNextHeroAttack()
    {
        //检测战斗是否结束
        if (CheckBattleIsOver() || BattleWorld.battleEnd)
        {
            return;
        }

        //所有攻击结束
        if (mHeroAttackQueue.Count == 0)
        {
            RoundEnd();
            NextRoundStart();
            return;
        }

        //下一个英雄攻击
        HeroLogic heroLogic = mHeroAttackQueue.Dequeue();
        heroLogic.OnActionEndListener = HeroActionEnd;
        heroLogic.BeginAction();
    }

    public void HeroActionEnd()
    {
        Debugger.Log("HeroActionEnd");
        StartNextHeroAttack();
    }

    /// <summary>
    /// 检测战斗是否结束
    /// </summary>
    public bool CheckBattleIsOver()
    {
        if (mHeroLogicCtrl.HeroIsAllDeath(HeroTeamEnum.Self))
        {
            Debugger.Log("You Lose!");
            WorldManager.BattleWorld.BattleEnd(false);
            return true;
        }

        if (mHeroLogicCtrl.HeroIsAllDeath(HeroTeamEnum.Enemy))
        {
            Debugger.Log("You Win!");
            WorldManager.BattleWorld.BattleEnd(true);
            return true;
        }

        return false;
    }

    public void RoundEnd()
    {
        foreach (var logic in mHeroLogicCtrl.allList)
        {
            logic.RoundEndEvent();
        }
    }

    public void OnLogicFrameUpdate()
    {
    }

    public void OnDestroy()
    {
    }
}