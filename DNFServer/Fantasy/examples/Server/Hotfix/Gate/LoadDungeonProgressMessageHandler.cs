using Fantasy;
using Hotfix.Helper;
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

namespace Hotfix.Gate
{
    public class LoadDungeonProgressMessageHandler : Message<LoadDungeonProgresssMessage>
    {
        protected override async FTask Run(Session session, LoadDungeonProgresssMessage message)
        {
            var teamComponent = session.Scene.GetComponent<TeamManagerComponent>();
            //获取队伍中所有的玩家
            List<Role>? teamRoleList = teamComponent.GetTeamRoleListByTeamid(message.teamid);
            LoadDungeonProgresssMessage loadDungeonMessage= new LoadDungeonProgresssMessage();
            if (teamRoleList != null)
            {
                Log.Info($"玩家进入地下城进度更新progress:{message.progress} playerid:{message.playerid}");
                loadDungeonMessage.progress=message.progress;
                loadDungeonMessage.playerid=message.playerid;
                loadDungeonMessage.teamid=message.teamid;
                foreach (Role role in teamRoleList)
                {
                    role.session.Send(loadDungeonMessage);
                }
                //广播开始战斗消息，放这个for循环中广播，是为了给客户端一个进度更新的表现事件。
                bool isStartBattle = teamComponent.ComputeAllRoleLoadFinish(message.teamid,message.playerid,message.progress);
                if (isStartBattle)
                {
                    var dungeonsComponent = session.Scene.GetComponent<DungeonsManagerComponent>();

                    //延迟1秒推送战斗开始，和逻辑帧更新消息 原因：让客户端玩家加载资源完毕
                    session.Scene.TimerComponent.Core.OnceTimer(1000,() => {
                        StartDungeonBattleMessage battleMsg = new StartDungeonBattleMessage();
                        //装载战斗玩家数据
                        battleMsg.battleRoleList = new List<RoleData>();
                        foreach (Role role in teamRoleList)
                        {
                            battleMsg.battleRoleList.Add(role.ToRoleData());
                        }
                        //通知所有客户端可以开始战斗了
                        foreach (Role role in teamRoleList)
                        {
                            Log.Info("所有客户端加载副本完成，推送开始战斗消息");
                            role.session.Send(battleMsg);
                        }

                        //服务端开始副本战斗，和客户端进行帧同步
                        dungeonsComponent.StartDungeonBattle(teamRoleList);

                    });
                
                }
            }
            else
            {
                Log.Info("队伍不存在，玩家进入地下城失败:" + message.teamid);
                session.Send(loadDungeonMessage);
            }

            await FTask.CompletedTask;
        }
    }
}
