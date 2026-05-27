using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public void Awake()
    {
        WorldManager.Initialize();
        
        //Test
        List<HeroData> heroList = new List<HeroData>();
        List<HeroData> enemyList = new List<HeroData>();
        // 10个测试英雄
        List<int> heroIdList = new List<int>{101, 102, 103, 104, 105, 501, 502, 503, 504, 505};
        for (int i = 0; i < heroIdList.Count; i++)
        {
            HeroData heroData = new HeroData();
            heroData.id = heroIdList[i];
            if (i < 5)
            { 
                heroData.seatid = i;
                heroList.Add(heroData);
            }
            else
            {
                heroData.seatid = i - 5;
                enemyList.Add(heroData);
            }
        }
        
        WorldManager.CreateBattleWorld(heroList, enemyList);
    }
    
    public void Update()
    {
        WorldManager.OnUpdate();
    }
    
    
    public void OnDestroy()
    {
        WorldManager.DestroyWorld();
    }
}
