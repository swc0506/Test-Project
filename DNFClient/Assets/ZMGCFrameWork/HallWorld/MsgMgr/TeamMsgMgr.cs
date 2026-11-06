/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/29 7:57:54
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using UnityEngine.PlayerLoop;

namespace ZMGC.Hall
{
    public class TeamMsgMgr : IMsgBehaviour
    {

        public void OnCreate()
        {

        }
        /// <summary>
        /// 发送创建队伍请求
        /// </summary>
        /// <param name="mapType"></param>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public async FTask<CreateTeamResponse> SendCreateTeamRequest(MapType mapType,long playerid)
        {
            CreateTeamReqest request= new CreateTeamReqest();
            request.mapType = (int)mapType;
            request.playerid = playerid;
            return  await NetWorkManager.Instance.SendCallMessage<CreateTeamResponse>(request);
        }
        /// <summary>
        /// 发送加入队伍请求
        /// </summary>
        /// <param name="mapType"></param>
        /// <param name="playerid"></param>
        /// <param name="teamid"></param>
        /// <returns></returns>
        public async FTask<JoinTeamResponse> SendJoinTeamRequest(MapType mapType, long playerid,int teamid)
        {
            JoinTeamReqest request = new JoinTeamReqest();
            request.mapType = (int)mapType;
            request.playerid = playerid;
            request.teamid = teamid;
            return await NetWorkManager.Instance.SendCallMessage<JoinTeamResponse>(request);
        }

        public class TeamStateChangeMessageHandler : Message<TeamStateChangeMessage>
        {
            protected override async FTask Run(Session session, TeamStateChangeMessage message)
            {
                ProtoBuffConvert.ToJson(message);
                HallWorld.GetExitsLogicCtrl<TeamLogicCtrl>().OnTeamRoleStateChangeNotify((TeamState)message.teamState,message.roleData);
                await FTask.CompletedTask;
            }
        }

        public void OnDestroy()
        {

        }

    }
}
