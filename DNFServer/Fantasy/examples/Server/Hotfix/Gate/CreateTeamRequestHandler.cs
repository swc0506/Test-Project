using Fantasy;
using Hotfix.Helper;
using Hotfix.System.Gate;
using Model.Gate.Hall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class CreateTeamRequestHandler : MessageRPC<CreateTeamReqest, CreateTeamResponse>
    {
        protected override async FTask Run(Session session, CreateTeamReqest request, CreateTeamResponse response, Action reply)
        {
            var teamComponent = session.Scene.GetComponent<TeamManagerComponent>();
            var result = teamComponent.CreateTeam(request.playerid, request.mapType);
            response.ErrorCode = result.eroorCode;
            if (result.eroorCode==ErrorCode.Success)
            {
                response.roleData = result.role.ToRoleData();
                response.tameid = result.teamid;
            }
            await FTask.CompletedTask;
        }
    }
}
