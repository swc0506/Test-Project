using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager
{
    private static BattleWorld mBattleWorld;
    
    public static void Initialize()
    {
        
    }

    public static void OnUpdate()
    {
        if (mBattleWorld != null)
        {
            mBattleWorld.OnUpdate();
        }
    }
    
    public static void CreateBattleWorld(List<HeroData> heroList, List<HeroData> enemyList)
    {
        mBattleWorld = new BattleWorld();
        mBattleWorld.CreateWorld(heroList, enemyList);
    }
    
    public static void DestroyWorld()
    {
        mBattleWorld.DestroyWorld();
    }
}