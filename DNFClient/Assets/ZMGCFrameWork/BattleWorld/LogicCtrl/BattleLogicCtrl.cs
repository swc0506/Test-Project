/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/3/16 13:08:23
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace ZMGC.Battle
{
    public class BattleLogicCtrl : ILogicBehaviour
    {
        private HeroLogicCtrl mHeroCtrl;
        private MonsterLogicCtrl mMonsterCtrl;
        public void OnCreate()
        {
            UnityEngine.Debug.Log("BattleLogicCtrl OnCreate");
            mHeroCtrl = BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>();
            mMonsterCtrl = BattleWorld.GetExitsLogicCtrl<MonsterLogicCtrl>();
        }

        /// <summary>
        /// 根据当前对象类型获取攻击的目标
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<LogicActor> GetEnemyList(LogicObjectType objectType)
        {
            List<LogicActor> enemyList = new List<LogicActor>();
            if (objectType==  LogicObjectType.Hero)
            {
                foreach (var item in mMonsterCtrl.monsterList)
                {
                    enemyList.Add(item);
                }
            }
            else if (objectType== LogicObjectType.Monster)
            {
                enemyList.Add(mHeroCtrl.HeroLogic);
            }
            return enemyList;
        }

        public void OnDestroy()
        {

        }

    }
}