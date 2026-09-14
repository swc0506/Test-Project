using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicLayer
{
    public class BattleWorldManager
    {
        /// <summary>
        /// 战斗是否已经开始
        /// </summary>
        public static bool BattleIsStarted = false;
        
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

        public static void CreateBattleWorld(List<HeroData> heroList, List<HeroData> enemyList, int randomSeed,
            long battleId, Action<BattleWorld> battleEndCallback = null, List<HeroSkillInputData> skillInputList = null,
            bool isReplay = false)
        {
            Debugger.Log("CreateBattleWorld.....");
            BattleWorld?.DestroyWorld();
            BattleWorld = new BattleWorld();
            BattleWorld.CreateWorld(heroList, enemyList, randomSeed, battleId, battleEndCallback, skillInputList,
                isReplay);
            BattleIsStarted = true;
        }

        public static void DestroyWorld()
        {
            BattleWorld.DestroyWorld();
            BattleIsStarted = false;
        }
    }
}