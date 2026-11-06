/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2024/3/16 13:09:55
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using System.Collections.Generic;

namespace ZMGC.Battle
{
    /// <summary>
    /// 战斗消息层 所有的消息收发都需要写到这里
    /// </summary>
    public class BattleMsgMgr : IMsgBehaviour
    {
      

        public void OnCreate()
        {

        }
        /// <summary>
        /// 发送退出地下城的请求
        /// </summary>
        /// <param name="battleid"></param>
        /// <param name="playerid"></param>
        public async FTask<ExitDungeonResponse> SendExitDungeonRequest(long battleid,long playerid)
        {
            ExitDungeonReqest request=new ExitDungeonReqest();
            request.battleid=battleid;
            request.playerid=playerid;
            return await NetWorkManager.Instance.SendCallMessage<ExitDungeonResponse>(request);
        }
        /// <summary>
        /// 发送战斗结束通知
        /// </summary>
        /// <param name="battleid"></param>
        /// <param name="playerid"></param>
        public void SendBattleEndMessage(long battleid,long playerid)
        {
            BattleEndMessage_Share message=new BattleEndMessage_Share();
            message.battleid=battleid;
            message.playerid=playerid;
            ProtoBuffConvert.ToJson(message);
            NetWorkManager.Instance.Send(message);
        }
        /// <summary>
        /// 发送领取战斗结束奖励消息
        /// </summary>
        /// <param name="index"></param>
        public void SendReceiveEndRewardMessage(int index,long playerid,long battleid)
        {
            BattleEndRewardMessage_Share messasge=new BattleEndRewardMessage_Share();
            messasge.cardRewardIndex= index;
            messasge.playerid=playerid;
            messasge.battleid=battleid;
            ProtoBuffConvert.ToJson(messasge);
            NetWorkManager.Instance.Send(messasge);
        }

        /// <summary>
        /// 发送逻辑帧操作事件到服务端
        /// </summary>
        /// <param name="battleid"></param>
        /// <param name="frameDataList"></param>
        public void SendFrameOperateEventMessage(long battleid,List<FrameOperateData> frameDataList)
        {
            FrameOperateEventMessage_C2G message = new FrameOperateEventMessage_C2G();
            message.battleid = battleid;
            message.frameDataList = frameDataList;
            NetWorkManager.Instance.Send(message);
        }
        /// <summary>
        /// 监听逻辑帧更新推送
        /// </summary>
        public class FrameOperateEventMessage : Message<FrameOperateEventMessage_G2C>
        {
            //private BattleLogicCtrl mBattleLogic;
            protected override async FTask Run(Session session, FrameOperateEventMessage_G2C message)
            {
                //if (mBattleLogic == null)
                //{
                //    mBattleLogic = BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>();
                //}
                ProtoBuffConvert.ToJson(message);
                //这里需要优化调用方式
                BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>().OnLogicFrameUpdateByServer(message);
                await FTask.CompletedTask;
            }
        }

        public class BattleEndRewardMessage_ShareHandler : Message<BattleEndRewardMessage_Share>
        {
            protected override async FTask Run(Session session, BattleEndRewardMessage_Share message)
            {
                BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>().OnReceiveCradReward(message.cardRewardIndex);
                await FTask.CompletedTask;
            }
        }
        /// <summary>
        /// 战斗结束通知
        /// </summary>
        public class BattleEndMessage_ShareHandler : Message<BattleEndMessage_Share>
        {
            protected async override FTask Run(Session session, BattleEndMessage_Share message)
            {
                BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>().OnBattleEndByServer();
                await FTask.CompletedTask;
            }
        }
        public void OnDestroy()
        {

        }

    }
 
}
