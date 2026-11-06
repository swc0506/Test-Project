using Fantasy;
using Hotfix.Helper;
using Hotfix.System.Gate;
using Model;
using Model.Gate;
using Model.Gate.Hall;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Gate
{
    public class EnterDungeonMessageHandler : Message<EnterDungeonMessage>
    {
        protected override async FTask Run(Session session, EnterDungeonMessage message)
        {

            var teamComponent= session.Scene.GetComponent<TeamManagerComponent>();
            List<Role>? teamRoleList= teamComponent.GetTeamRoleListByTeamid(message.teamid);
            EnterDungeonMessage enterDungeon = new EnterDungeonMessage();
            if (teamRoleList != null)
            {
                Log.Info("通知所有玩家可以进入地下城" + message.teamid);
               
                enterDungeon.teamRoleInfoList = new List<RoleData>();
                //装载所有角色信息
                foreach (Role role in teamRoleList)
                {
                    enterDungeon.teamRoleInfoList.Add(role.ToRoleData());
                }
                //给队伍中的所有玩家发送通知
                foreach (var item in teamRoleList)
                {
                    item.session.Send(enterDungeon);
                }
                //清理当前玩家在大厅地图上的缓存
                var hallPlayerComponent= session.Scene.GetComponent<HallPlayerManagerComponent>();
                hallPlayerComponent.RemoveHallPlayer(teamRoleList);

            }
            else 
            {
                Log.Info("队伍不存在，玩家进入地下城失败:" + message.teamid);
                session.Send(enterDungeon);
            }
            await FTask.CompletedTask;
        }
    }
}
