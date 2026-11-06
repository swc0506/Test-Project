/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/31 8:26:07
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;

namespace ZMGC.Hall
{
    public class DungeonSelectMsgMgr : IMsgBehaviour
    {

        public void OnCreate()
        {

        }
        /// <summary>
        /// 发送进入地下城通知到服务端
        /// </summary>
        /// <param name="teamid"></param>
        public void SendEnterDungeonsMessage(int teamid)
        { 
            EnterDungeonMessage message = new EnterDungeonMessage();
            message.teamid = teamid;
            message.dungeonCfgid = 0;
            NetWorkManager.Instance.Send(message);
        }

        /// <summary>
        /// 发送加载地下城进度通知消息
        /// </summary>
        /// <param name="teamid"></param>
        public void SendLoadDungeonsProgressMessage(int teamid,long playerid,float progress)
        {
            LoadDungeonProgresssMessage message = new LoadDungeonProgresssMessage();
            message.teamid = teamid;
            message.playerid = playerid;
            message.progress = progress;
            NetWorkManager.Instance.Send(message);
        }

        /// <summary>
        /// 进入地下城通知
        /// </summary>
        public class EnterDungeonMessageHandler : Message<EnterDungeonMessage>
        {
            protected override async FTask Run(Session session, EnterDungeonMessage message)
            {
                ProtoBuffConvert.ToJson(message);
                HallWorld.GetExitsLogicCtrl<DungeonSelectLogicCtrl>().OnEnterDungensLogic(message);
                await FTask.CompletedTask;
            }
        }
        /// <summary>
        /// 加载地下城进度通知
        /// </summary>
        public class LoadDungeonProgresssHandler : Message<LoadDungeonProgresssMessage>
        {
            protected override async FTask Run(Session session, LoadDungeonProgresssMessage message)
            {
                ProtoBuffConvert.ToJson(message);
                HallWorld.GetExitsLogicCtrl<DungeonSelectLogicCtrl>().OnLoadDungensProgressLogic(message);
                await FTask.CompletedTask;
            }
        }
        /// <summary>
        /// 可以开始地下城战斗通知
        /// </summary>
        public class StartDungeonBattleHandler : Message<StartDungeonBattleMessage>
        {
            protected override async FTask Run(Session session, StartDungeonBattleMessage message)
            {
                ProtoBuffConvert.ToJson(message);
                HallWorld.GetExitsLogicCtrl<DungeonSelectLogicCtrl>().OnStartBattleDungensLogic(message);
                await FTask.CompletedTask;
            }
        }
        public void OnDestroy()
        {

        }

    }
}
