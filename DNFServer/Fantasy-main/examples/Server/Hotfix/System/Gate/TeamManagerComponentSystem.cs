using Amazon.Runtime.Internal.Transform;
using CommandLine;
using Fantasy;
using Hotfix.Helper;
using Model;
using Model.Gate;
using Model.Gate.Hall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.System.Gate
{
    public static class TeamManagerComponentSystem
    {
        /// <summary>
        /// 获取角色列表通过队伍id
        /// </summary>
        /// <param name="self"></param>
        /// <param name="teamid"></param>
        /// <returns></returns>
        public static List<Role> GetRoleListByTeamid(this TeamManagerComponent self, int teamid)
        {
            self.allTeamInfoDic.TryGetValue(teamid, out var result);
            return result;
        }
        /// <summary>
        /// 解散队伍
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public static int DissolvedTeam(this TeamManagerComponent self, long playerid)
        { 
            //首先查询缓存中是否存在该队伍，若不存在则解散失败
            if (self.alreadyCreateTeamDic.TryGetValue(playerid,out var teamid)) 
            { 
                self.alreadyCreateTeamDic.Remove(playerid);
                if (self.allTeamInfoDic.TryGetValue(teamid,out var playerList))
                {
                    //把当前玩家从队伍列表中移除掉
                    for (int i = 0; i < playerList.Count; i++)
                    {
                        if (playerList[i].accountid==playerid)
                        {
                            playerList[i].Dispose();
                            playerList.Remove(playerList[i]);
                            break;
                        }
                    }

                    //移除该队伍
                    if (playerList.Count==0)
                    {
                        self.allTeamInfoDic.Remove(teamid);
                    }
                    return teamid ;
                }
            }
            return 0;
        }
        /// <summary>
        /// 根据队伍id获取该队中的所有角色信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="teamid"></param>
        /// <returns></returns>
        public static bool ComputeAllRoleLoadFinish(this TeamManagerComponent self, int teamid,long playerid,float progress)
        {
            //缓存的加载队伍字典中是否存在这个队伍
            if (!self.teamPlayerLoadProgressDic.TryGetValue(teamid,out var teamRoleProgressDic)) 
            { 
                self.teamPlayerLoadProgressDic.Add(teamid,new Dictionary<long, float> { { playerid, progress } });
                return false;
            }
             
            //如果队伍列表中没有这个玩家的加载进度
            if (!teamRoleProgressDic.ContainsKey(playerid))
            {
                teamRoleProgressDic.Add(playerid,progress);
            }
            //队伍中存在这个玩家的进度，直接进行更新
            teamRoleProgressDic[playerid] = progress;
            //获取这个队伍真实的玩家数量
            self.allTeamInfoDic.TryGetValue(teamid, out var teamInfoList);
            if (teamInfoList == null || teamInfoList.Count == 0)
            {
                Log.Error("队伍玩家异常，开始战斗失败...");
                return false; 
            }
            //获取队伍中真实的玩家数量
            int realPlayerCount= teamInfoList.Count;
            //验证所有玩家是否加载副本已经完成
            if (realPlayerCount == teamRoleProgressDic.Count)
            {
                foreach (var item in teamRoleProgressDic)
                {
                    if (item.Value!=1)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据队伍id获取该队中的所有角色信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="teamid"></param>
        /// <returns></returns>
        public static List<Role>? GetTeamRoleListByTeamid(this TeamManagerComponent self, int teamid)
        {
            //根据队伍id获取到该队伍中的所有角色
            self.allTeamInfoDic.TryGetValue(teamid, out var teamInfo);
            return teamInfo;
        }


        /// <summary>
        /// 创建队伍
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerid"></param>
        /// <param name="mapType"></param>
        /// <returns></returns>
        public static (uint eroorCode, int teamid, Role? role) CreateTeam(this TeamManagerComponent self, long playerid, int mapType)
        {
            //首先查询该队伍是否存在，如果已经存在表明创建失败 以及该玩家是否已经在队伍中
            if (self.alreadyCreateTeamDic.ContainsKey(playerid))
            {
                return (ErrorCode.CreateTeamFailed_AlreadyExitsTeaming, 0, null);
            }


            //获取大厅中的该玩家
            var hallPlayerComponent = self.Scene.GetComponent<HallPlayerManagerComponent>();
            HallPlayer? player = hallPlayerComponent.GetPlayer(mapType, playerid);
            if (player == null)
            {
                return (ErrorCode.CreateTeamFailed_NotFindPlayer, 0, null);
            }
            //队伍没有创建，可以创建队伍了
            int teamid = self.TeamId;
            //创建队伍
            self.alreadyCreateTeamDic.Add(playerid, teamid);
            self.allTeamInfoDic.Add(teamid, new List<Role> { player.roleInfo });
            //创建队伍成功
            Log.Info("创建队伍成功 Teamid:" + teamid);
            return (ErrorCode.Success, teamid, player.roleInfo);
        }

        /// <summary>
        /// 加入队伍
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerid"></param>
        /// <param name="mapType"></param>
        /// <returns></returns>
        public static (uint eroorCode, List<Role>? teamRoleList, Role? curRole) JoinTeam(this TeamManagerComponent self, long playerid, int mapType, int teamid)
        {
            //如果这个队伍不存在
            if (!self.allTeamInfoDic.TryGetValue(teamid, out var teamRoleList))
            {
                return (ErrorCode.JoinTeamFailed_NotFindTeam, null, null);
            }

            //判断队伍中玩家是否超过上限
            if (teamRoleList.Count > CommonConfig.TeamMaxPlayerCount)
            {
                return (ErrorCode.JoinTeamFailed_OutTeamPlayerCount, null, null);
            }


            //获取大厅中的该玩家
            var hallPlayerComponent = self.Scene.GetComponent<HallPlayerManagerComponent>();
            HallPlayer? player = hallPlayerComponent.GetPlayer(mapType, playerid);
            if (player == null)
            {
                return (ErrorCode.JoinTeamFailed_NotFindPlayer, null, null);
            }
            //把该玩家添加至队伍中
            teamRoleList.Add(player.roleInfo);
            self.alreadyCreateTeamDic.Add(playerid, teamid);
            //加入队伍成功
            Log.Info("加入队伍成功 Teamid:" + teamid);
            return (ErrorCode.Success, teamRoleList, player.roleInfo);
        }

        /// <summary>
        /// 玩家断开连接
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerid"></param>
        /// <param name="mapType"></param>
        /// <returns></returns>
        public static void PlayerDisConnect(this TeamManagerComponent self, long playerid)
        {
            //该玩家是否在队伍中
            if (!self.alreadyCreateTeamDic.TryGetValue(playerid, out var teamid))
            {
                return;
            }

            //查询该玩家所在的队伍
            if (!self.allTeamInfoDic.TryGetValue(teamid, out var teamRoleList))
            {
                return;
            }
            TeamStateChangeMessage message = new TeamStateChangeMessage();
            //该玩家是队伍队长  队伍解散
            if (teamRoleList.Count > 0 && teamRoleList[0].accountid == playerid)
            {
                message.teamState = 3;
                //广播消息到其他的客户端，告诉他们队长断线，队伍就直接解散
                self.BroadCastMessageToOtherPlayer(teamRoleList, message, playerid);
                //清理玩家队伍信息
                self.alreadyCreateTeamDic.Remove(playerid);
                self.allTeamInfoDic.Remove(teamid);
                Log.Info("队长断线，队伍解散...");
                return;
            }
            //玩家离开
            if (teamRoleList.Count > 0)
            {
                //该玩家是一个队员
                message.teamState = 2;
                //广播消息到其他的客户端，告诉他们队长断线，队伍就直接解散
                self.BroadCastMessageToOtherPlayer(teamRoleList, message, playerid);
                //清理玩家队伍信息
                self.alreadyCreateTeamDic.Remove(playerid);
                //从该玩家所在的队伍中移除该玩家
                for (int i = 0; i < teamRoleList.Count; i++)
                {
                    if (teamRoleList[i].accountid == playerid)
                    {
                        teamRoleList.Remove(teamRoleList[i]);
                        break;
                    }
                }
                Log.Info("队员断线，队员被提出...");
                return;
            }



        }
        /// <summary>
        /// 广播消息到所有的客户端
        /// </summary>
        /// <param name="self"></param>
        /// <param name="teamRoleList"></param>
        /// <param name="message"></param>
        /// <param name="playerid"></param>
        public static void BroadCastMessageToOtherPlayer(this TeamManagerComponent self, List<Role> teamRoleList, TeamStateChangeMessage message, long playerid)
        {
            RoleData roleData = self.GetTeamRoleDataByPlayerid(teamRoleList, playerid);
            foreach (Role role in teamRoleList)
            {
                if (role.accountid != playerid)
                {
                    message.roleData = roleData;
                    role.session.Send(message);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="teamRoleList"></param>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public static RoleData GetTeamRoleDataByPlayerid(this TeamManagerComponent self, List<Role> teamRoleList, long playerid)
        {
            foreach (Role role in teamRoleList)
            {
                if (role.accountid == playerid)
                {
                    return role.ToRoleData();
                }
            }
            return null;
        }
    }
}
