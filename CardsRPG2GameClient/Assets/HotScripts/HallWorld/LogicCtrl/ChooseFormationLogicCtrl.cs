/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/31 17:59:34
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using LogicLayer;

namespace ZMGC.Hall
{
    public class ChooseFormationLogicCtrl : ILogicBehaviour
    {
        private ChooseFormationDataMgr dataMgr;
        private ChooseFormationMsgMgr msgMgr;

        public void OnCreate()
        {
            dataMgr = HallWorld.GetExitsDataMgr<ChooseFormationDataMgr>();
            msgMgr = HallWorld.GetExitsMsgMgr<ChooseFormationMsgMgr>();
        }

        public void OnDestroy()
        {
        }

        /// <summary>
        ///  英雄上阵
        /// </summary>
        /// <param name="heroId"></param>
        public int HeroSeatDown(int heroId)
        {
            if (dataMgr.GetNullSeatCount() == 0)
            {
                ToastManager.ShowToast("上阵已满");
                return 1;
            }

            int seatId = 0;
            foreach (var seat in dataMgr.HeroSeatDict)
            {
                if (seat.Value == 0)
                {
                    dataMgr.AddHeroToSeat(seat.Key, heroId);
                    seatId = seat.Key;
                    break;
                }
            }

            UIEventControl.DispensEvent(UIEventEnum.UpdateHeroSeat, new object[] { heroId, seatId });
            return 0;
        }

        public void HeroSeatLeave(int heroId)
        {
            int seatId = 0;
            foreach (var dic in dataMgr.HeroSeatDict)
            {
                if (dic.Value == heroId)
                {
                    seatId = dic.Key;
                    dataMgr.RemoveHeroFromSeat(seatId);
                    break;
                }
            }

            UIEventControl.DispensEvent(UIEventEnum.HeroLeaveSeat, heroId);
        }

        public bool IsHeroSeatDown(int heroId)
        {
            foreach (var item in dataMgr.HeroSeatDict)
            {
                if (item.Value == heroId)
                {
                    return true;
                }
            }

            return false;
        }


        public int StartFight(int levelId)
        {
            List<HeroSeatDataPb> meHeroList = new List<HeroSeatDataPb>();

            foreach (var item in dataMgr.HeroSeatDict)
            {
                if (item.Value == 0)
                {
                    continue;
                }

                meHeroList.Add(new HeroSeatDataPb()
                {
                    id = item.Value,
                    seatId = item.Key
                });
            }

            if (meHeroList.Count < 5)
            {
                ToastManager.ShowToast("请选择英雄");
                return 1;
            }

            msgMgr.SendStartFightReq(meHeroList, levelId);
            return 0;
        }

        /// <summary>
        ///  开始战斗成功
        /// </summary>
        /// <param name="response"></param>
        public void StartFightSuccess(StartBattleResponse response)
        {
            List<HeroData> heroList = new List<HeroData>();
            List<HeroData> enemyList = new List<HeroData>();
            foreach (var item in response.heroDataList)
            {
                heroList.Add(item.ToHeroData());
            }

            foreach (var item in response.enemyHeroDataList)
            {
                enemyList.Add(item.ToHeroData());
            }
            
            UIModule.Instance.DestroyAllWindow();
            
            Debugger.Log("开始战斗......");
            BattleWorldManager.CreateBattleWorld(heroList, enemyList, response.randomSeed, response.battleId);
        }
    }
}