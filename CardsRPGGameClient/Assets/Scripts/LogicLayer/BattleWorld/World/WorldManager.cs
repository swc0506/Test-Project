using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager
{
    public static BattleWorld BattleWorld { get; private set; }
    
    public static void Initialize()
    {
        
    }

    public static void OnUpdate()
    {
        if (BattleWorld != null)
        {
            BattleWorld.OnUpdate();
        }
    }
    
    public static void CreateBattleWorld(List<HeroData> heroList, List<HeroData> enemyList)
    {
        BattleWorld = new BattleWorld();
        BattleWorld.CreateWorld(heroList, enemyList);
    }
    
    public static void DestroyWorld()
    {
        BattleWorld.DestroyWorld();
    }
}