using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicLayer
{
    public class BattleWorldManager
    {
        public static BattleWorld BattleWorld { get; private set; }

        public static void Initialize()
        {
            ConfigCenter.Init();
            SkillConfigCenter.Initialized();
        }

        public static void OnUpdate()
        {
            if (BattleWorld != null)
            {
                BattleWorld.OnUpdate();
            }
        }

        public static void CreateBattleWorld(List<HeroData> heroList, List<HeroData> enemyList, int randomSeed,
            int battleId, Action<BattleWorld> battleEndCallback = null)
        {
            BattleWorld?.DestroyWorld();
            BattleWorld = new BattleWorld();
            BattleWorld.CreateWorld(heroList, enemyList, randomSeed, battleId, battleEndCallback);
        }

        public static void DestroyWorld()
        {
            BattleWorld.DestroyWorld();
        }
    }
}