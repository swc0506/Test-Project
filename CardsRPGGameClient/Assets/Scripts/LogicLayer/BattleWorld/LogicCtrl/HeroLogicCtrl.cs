using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogicCtrl : ILogicBehaviour
{
    public void OnCreate()
    {
        
    }
    
    public void OnCreate(List<HeroData> heroList, List<HeroData> enemyList)
    {
        
    }
    
    /// <summary>
    /// 创建英雄
    /// </summary>
    /// <param name="heroList"></param>
    public void CreateHero(List<HeroData> heroList)
    {
        foreach (var heroData in heroList)
        {
            
        }
    }

    public void OnLogicFrameUpdate()
    {
    }

    public void OnDestroy()
    {
    }
}
