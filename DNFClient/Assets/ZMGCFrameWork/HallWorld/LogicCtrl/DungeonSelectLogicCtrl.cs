/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/31 8:25:55
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using ZMGC.Battle;

namespace ZMGC.Hall
{
    public class DungeonSelectLogicCtrl : ILogicBehaviour
    {

        public void OnCreate()
        {

        }
        

        /// <summary>
        /// 进入地下城逻辑
        /// </summary>
        /// <param name="message"></param>
        public void OnEnterDungensLogic(EnterDungeonMessage message)
        {
            if (message.teamRoleInfoList == null)
            {
                ToastManager.ShowToast("进入地下城失败，队伍不存在");
                return;
            }
            //进入战斗世界
            HallWorld.EnterBattleWorld(message.teamRoleInfoList);
        }
        /// <summary>
        /// 加载地下城进度逻辑
        /// </summary>
        /// <param name="message"></param>
        public void OnLoadDungensProgressLogic(LoadDungeonProgresssMessage message)
        {
            UIEventControl.DispensEvent( UIEventEnum.DungeonProgressUpdate, message);
        }
        /// <summary>
        /// 开始地下城战斗城逻辑
        /// </summary>
        /// <param name="message"></param>
        public void OnStartBattleDungensLogic(StartDungeonBattleMessage message)
        {
            UIModule.Instance.DestroyAllWindow();

            WorldManager.CreateWorld<BattleWorld>(() => {
                BattleWorld.GetExitsDataMgr<HeroDataMgr>().CacheBattleRoleList(message.battleRoleList);
            });
        }
        public void OnDestroy()
        {

        }

    }
}
