using Fantasy;
using Hotfix.System.Gate;
using Model;
using Model.Gate.Hall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class StateSyncRequestHandler : MessageRPC<StateSyncReqest, StateSyncResponse>
    {
        protected override async FTask Run(Session session, StateSyncReqest request, StateSyncResponse response, Action reply)
        {
            var hallPlayerComponent= session.Scene.GetComponent<HallPlayerManagerComponent>();

            //1.根据客户端的输入方向计算客户端角色要移动的位置
            var result= hallPlayerComponent.PlayerMove(request.roleSyncData);
            response.ErrorCode = result.errorCode;
            if (result.errorCode!=ErrorCode.Success)
            {
                return;
            }
            //2.移动位置成功
            response.statePacketId=request.statePacketId;
            response.roleSyncData= result.serverSyncData;

           
            //为节省性能只创建一个消息结构体
            OtherPlayerStateSyncMessage message = new OtherPlayerStateSyncMessage() {};
            message.roleData = response.roleSyncData;
            message.roleData.playerMapStatus = 1;//进入地图

            //把当前玩家的状态广播给当前地图上的其他玩家
            List<HallPlayer>? hallPalyerList= hallPlayerComponent.GetPlayerListInMap(request.roleSyncData.mapType,request.roleSyncData.playerid);
            if (hallPalyerList!=null)
            {
                foreach (var player in hallPalyerList)
                {
                    if (player == null) { continue; }
                    player.session.Send(message);
                }
            }
            await FTask.CompletedTask;
        }
    }
}
