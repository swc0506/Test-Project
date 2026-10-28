using Fantasy;
using Hotfix.Helper;
using Hotfix.System.Gate;
using Model;
using Model.Gate;
using Model.Gate.Hall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Gate
{
    public class EnetMapRequestHandler : MessageRPC<EnterMapReqest, EnterMapResponse>
    {
        protected override async FTask Run(Session session, EnterMapReqest request, EnterMapResponse response, Action reply)
        {
            var roleComponent= session.Scene.GetComponent<RoleManagerComponent>();
            //验证玩家使用的角色是否存在,如果不存在则是非法的进入地图请求
            Role role= roleComponent.GetPlayerUseRole(request.playerid);
            if (role == null)
            {
                //非法数据，尚未选择角色
                response.ErrorCode = ErrorCode.EnterMapFailed_NotFindRole;
                return;
            }
            //验证玩家进入的地图是否存在
            var hallPlayerComponent = session.Scene.GetComponent<HallPlayerManagerComponent>();
            var errorCode= hallPlayerComponent.VerifyHallEnterMap(request.mapType,request.doorType);
            if (errorCode!=ErrorCode.Success)
            {
                response.ErrorCode = errorCode;
                return;
            }

            //移除该玩家所在的上一张地图
            hallPlayerComponent.RemoveHallPlayer(request.curMap,request.playerid);
            //把该玩家添加至新的地图中
            HallPlayer playerData= hallPlayerComponent.AddHallPlayer(session,request.mapType,request.doorType,request.playerid,role);
            //返回客户端响应
            response.roleInitPos = playerData.position.ToCSVector3();
            response.mapType = request.mapType;
            response.doorType = request.doorType;
            response.playerid = request.playerid;
            response.ErrorCode = 0;


            //广播玩家在上一张地图的状态给上一张地图的所有玩家 目的：让上一张地图上的所有的玩家去销毁/隐藏对应的玩家
            List<HallPlayer>? playerList= hallPlayerComponent.GetPlayerListInMap(request.curMap,request.playerid);
            if (playerList!=null)
            {
                OtherPlayerStateSyncMessage message = new OtherPlayerStateSyncMessage() {roleData=new StateSyncData() };
                message.roleData.playerMapStatus = 2;
                message.roleData.playerid = request.playerid;
                foreach (HallPlayer player in playerList)
                {
                    if (player!=null)
                    {
                        //向所有玩家推送当前玩家离开这个地图的消息
                        player.session.Send(message);
                    }
                }
            }

            //广播当前地图上的所有的玩家给当前客户端
            playerList = hallPlayerComponent.GetPlayerListInMap(request.mapType, request.playerid);
            if (playerList != null)
            {
                //记录其他玩家信息
                foreach (HallPlayer player in playerList)
                {
                    if (player != null)
                    {
                        OtherPlayerStateSyncMessage message = new OtherPlayerStateSyncMessage() { roleData = new StateSyncData() };
                        message.roleData.playerMapStatus = 1;
                        message.roleData.playerid = player.playerid;
                        message.roleData.position = player.position.ToCSVector3();
                        message.roleData.inputDir = new CSVector3();
                        message.roleData.roleid = player.roleInfo.roleid;
                        //向所有玩家推送当前玩家离开这个地图的消息
                        session.Send(message);
                    }
                }
            }
            //广播当前玩家给当前地图上的其他的玩家
            playerList=  hallPlayerComponent.GetPlayerListInMap(request.mapType,request.playerid);
            if (playerList != null)
            {
                OtherPlayerStateSyncMessage message = new OtherPlayerStateSyncMessage();
                message.roleData = new StateSyncData()
                {
                    playerid = request.playerid,
                    position = playerData.position.ToCSVector3(),
                    inputDir = new CSVector3(),
                    playerMapStatus = 1,
                    roleid = playerData.roleInfo.roleid,
                };
                foreach (HallPlayer item in playerList)
                {
                    item.session.Send(message);
                }
            }
            await FTask.CompletedTask;
        }
    }
}
