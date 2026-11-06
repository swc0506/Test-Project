using Fantasy;
using Hotfix.Helper;
using Hotfix.System.Gate;
using Model.Gate.Hall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Gate
{
    public class JoinTeamRequestHandler : MessageRPC<JoinTeamReqest, JoinTeamResponse>
    {
        protected override async FTask Run(Session session, JoinTeamReqest request, JoinTeamResponse response, Action reply)
        {

            var teamComponent= session.Scene.GetComponent<TeamManagerComponent>();
            //加入队伍
            var result= teamComponent.JoinTeam(request.playerid, request.mapType,request.teamid);
            if (result.eroorCode==ErrorCode.Success)
            {
                response.tameid = request.teamid;
                response.roleList = new List<RoleData>();
                //装载队伍角色数据
                foreach (var item in result.teamRoleList)
                {
                    response.roleList.Add(item.ToRoleData());
                }
                Log.Info("JoinTeam Success");
                //广播玩家加入消息给其他的客户端
                TeamStateChangeMessage message = new TeamStateChangeMessage();
                message.teamState = 1;//有玩家加入队伍
                message.roleData = result.curRole.ToRoleData();
                foreach (var item in result.teamRoleList)
                {
                    if (item.accountid != request.playerid)
                    {
                        Log.Info("广播玩家加入消息给其他的客户端" + message.roleData.roleName);
                        item.session.Send(message);
                    }
                }
            }
          
            await FTask.CompletedTask;
        }
    }
}
