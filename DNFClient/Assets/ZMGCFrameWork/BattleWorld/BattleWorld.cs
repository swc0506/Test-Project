using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

namespace ZMGC.Battle
{
    public class BattleWorld : World
    {
        public HeroLogicCtrl HeroLogicCtrl { get; set; }
        public MonsterLogicCtrl MonsterLogicCtrl { get; set; }
        
        public override void OnCreate()
        {
            base.OnCreate();
            HeroLogicCtrl = GetExitsLogicCtrl<HeroLogicCtrl>();
            MonsterLogicCtrl = GetExitsLogicCtrl<MonsterLogicCtrl>();
            Debug.Log("BattleWorld  OnCreate>>>");
            HeroLogicCtrl.InitHero();
            MonsterLogicCtrl.InitMonster();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
        }
    }
}