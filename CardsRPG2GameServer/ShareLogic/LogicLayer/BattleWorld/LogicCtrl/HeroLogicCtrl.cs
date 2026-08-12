using System;
using System.Collections;
using System.Collections.Generic;
using LogicLayer;
using UnityEngine;
#if CLIENT_LOGIC
using ZM.ZMAsset;
#endif

public enum HeroTeamEnum
{
    None,
    Self,
    Enemy
}

public class HeroLogicCtrl : LogicLayer.ILogicBehaviour
{
    public List<HeroLogic> allList = new List<HeroLogic>();
    public List<HeroLogic> heroLogicList = new List<HeroLogic>();
    public List<HeroLogic> enemyLogicList = new List<HeroLogic>();

    public void OnCreate()
    {
    }

    public void OnCreate(List<HeroData> heroList, List<HeroData> enemyList)
    {
#if CLIENT_LOGIC
        CreateHero(heroList, BattleWorldManager.BattleWorld.Root3D.leftSeatTransArr, HeroTeamEnum.Self);
        CreateHero(enemyList, BattleWorldManager.BattleWorld.Root3D.rightSeatTransArr, HeroTeamEnum.Enemy);
#else
        CreateHero(heroList, null, HeroTeamEnum.Self);
        CreateHero(enemyList, null, HeroTeamEnum.Enemy);
#endif
    }

    /// <summary>
    /// 创建英雄
    /// </summary>
    /// <param name="heroList"></param>
    /// <param name="parents"></param>>
    /// <param name="team"></param>
    public void CreateHero(List<HeroData> heroList, Transform[] parents, HeroTeamEnum team)
    {
        foreach (HeroData heroData in heroList)
        {
            HeroLogic heroLogic = new HeroLogic(heroData, team);

#if CLIENT_LOGIC
            //生成
            // GameObject heroObj = ResourcesManager.Instance.LoadObject("Prefabs/Hero/" + heroData.id,
            //     parents[heroData.seatId], true, false, true);
            GameObject heroObj = ZMAsset.InstantiateObject(
                $"{AssetsPathConfig.HALL_PREFABS_PATH}BattleRoles/role_{heroData.name}", parents[heroData.seatId]);
            HeroRender heroRender = heroObj.GetComponent<HeroRender>();
            if (heroRender == null)
            {
                heroRender = heroObj.AddComponent<HeroRender>();
            }
            heroLogic.SetRenderObject(heroRender);
            heroRender.SetLogicObject(heroLogic);
            heroRender.SetHeroData(heroData, team);
#endif

            heroLogic.OnCreate();
            allList.Add(heroLogic);
            switch (team)
            {
                case HeroTeamEnum.Self:
                    heroLogicList.Add(heroLogic);
                    break;
                case HeroTeamEnum.Enemy:
                    enemyLogicList.Add(heroLogic);
                    break;
            }
        }
    }

    public void OnLogicFrameUpdate()
    {
    }

    public List<HeroLogic> GetHeroListByTeam(HeroLogic attacker, HeroTeamEnum attackTeam)
    {
        switch (attacker.TeamEnum)
        {
            case HeroTeamEnum.Self:
                return attackTeam == HeroTeamEnum.Self ? heroLogicList : enemyLogicList;
            case HeroTeamEnum.Enemy:
                return attackTeam == HeroTeamEnum.Enemy ? heroLogicList : enemyLogicList;
        }

        return null;
    }

    /// <summary>
    /// 计算出手队列
    /// </summary>
    /// <returns></returns>
    public Queue<HeroLogic> CalcAttackSort()
    {
        Queue<HeroLogic> heroLogicQueue = new Queue<HeroLogic>();
        allList.Sort((x, y) => { return y.Agl.CompareTo(x.Agl); });
        foreach (HeroLogic heroLogic in allList)
        {
            heroLogicQueue.Enqueue(heroLogic);
        }

        return heroLogicQueue;
    }

    public bool HeroIsAllDeath(HeroTeamEnum team)
    {
        Debugger.Log("HeroIsDeath:" + "mHeroList.Count" + heroLogicList.Count + "  enemyCount:" + enemyLogicList.Count);
        List<HeroLogic> list = team == HeroTeamEnum.Self ? heroLogicList : enemyLogicList;
        foreach (var logic in list)
        {
            if (logic.objectState == LogicObjectState.Survival)
            {
                return false;
            }
        }

        return true;
    }

#if CLIENT_LOGIC
    /// <summary>
    /// 设置自己队伍的遮罩
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="isShow"></param>
    public void SetSelfTeamMask(HeroLogic attacker, bool isShow)
    {
        List<HeroLogic> list = attacker.TeamEnum == HeroTeamEnum.Self ? heroLogicList : enemyLogicList;
        foreach (var heroLogic in list)
        {
            if (heroLogic.Id != attacker.Id)
            {
                heroLogic.HeroRender.SetHeroState(isShow);
            }
        }
    }
    
    /// <summary>
    /// 设置己方所有英雄遮罩
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="isShow"></param>
    public void SetSelfAllMask(HeroLogic attacker, bool isShow)
    {
        List<HeroLogic> list = attacker.TeamEnum == HeroTeamEnum.Self ? heroLogicList : enemyLogicList;
        foreach (var heroLogic in list)
        {
            heroLogic.HeroRender.SetHeroState(isShow);
        }
    }
    
    /// <summary>
    /// 设置除目标英雄外遮罩
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="targetList"></param>
    /// <param name="isShow"></param>
    public void SetOutsideOfTargetMask(HeroLogic attacker, List<HeroLogic> targetList, bool isShow)
    {
        List<int> targetIdList = new List<int>();
        foreach (var target in targetList)
        {
            targetIdList.Add(target.Id);
        }
        
        foreach (var heroLogic in heroLogicList)
        {
            if (!targetIdList.Contains(heroLogic.Id))
            {
                heroLogic.HeroRender.SetHeroState(isShow);
            }
        }

        foreach (var logic in enemyLogicList)
        {
            if (!targetIdList.Contains(logic.Id))
            {
                logic.HeroRender.SetHeroState(isShow);
            }
        }
    }
#endif

    public void OnDestroy()
    {
        for (int i = 0; i < allList.Count; i++)
        {
            allList[i].OnDestroy();
        }

        allList.Clear();
        heroLogicList.Clear();
        enemyLogicList.Clear();
    }
}