using Fantasy;
using Hotfix.System.Dungeons;
using Hotfix.System.Gate;
using Model;
using Model.Gate;
using Model.Gate.Hall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class ExitDungeonRequestHandler : MessageRPC<ExitDungeonReqest, ExitDungeonResponse>
    {
        protected override async FTask Run(Session session, ExitDungeonReqest request, ExitDungeonResponse response, Action reply)
        {
            //1.玩家退出地下城，处理1：解散队伍，处理：保留队伍 方便玩家回到大厅之后可以再次选择副本进行战斗
            var teamComponent = session.Scene.GetComponent<TeamManagerComponent>();
            int teamid = teamComponent.DissolvedTeam(request.playerid);
            //2.移除战斗中的玩家
            var dungeonComponent = session.Scene.GetComponent<DungeonsManagerComponent>();
            dungeonComponent.RemoveDungeonPlayer(request.battleid, request.playerid);
            //3.判断当前地下城中是否还有玩家，如果没有玩家，就停止帧同步逻辑帧推送。
            List<Role> teamRoleList = teamComponent.GetTeamRoleListByTeamid(teamid);
            if (teamRoleList == null || teamRoleList.Count == 0)
            {
                //释放当前玩家所在的地下城，并停止逻辑帧的推送
                dungeonComponent.DungeonBattleEnd(request.battleid);
            }
            await FTask.CompletedTask;
        }
    }
}
