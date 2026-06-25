using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeroTeamEnum
{
    None,
    Self,
    Enemy
}

public class HeroLogicCtrl : ILogicBehaviour
{
    public List<HeroLogic> alList = new List<HeroLogic>();
    public List<HeroLogic> heroLogicList = new List<HeroLogic>();
    public List<HeroLogic> enemyLogicList = new List<HeroLogic>();

    public void OnCreate()
    {
    }

    public void OnCreate(List<HeroData> heroList, List<HeroData> enemyList)
    {
#if CLIENT_LOGIC
        CreateHero(heroList, BattleWorldNodes.Instance.heroRootArr, HeroTeamEnum.Self);
        CreateHero(enemyList, BattleWorldNodes.Instance.enemyRootArr, HeroTeamEnum.Enemy);
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
            GameObject heroObj = ResourcesManager.Instance.LoadObject("Prefabs/Hero/" + heroData.id,
                parents[heroData.seatid], true, false, true);
            HeroRender heroRender = heroObj.GetComponent<HeroRender>();
            heroLogic.SetRenderObject(heroRender);
            heroRender.SetLogicObject(heroLogic);
            heroRender.SetHeroData(heroData, team);
#endif

            heroLogic.OnCreate();
            alList.Add(heroLogic);
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

    public void OnDestroy()
    {
    }
}