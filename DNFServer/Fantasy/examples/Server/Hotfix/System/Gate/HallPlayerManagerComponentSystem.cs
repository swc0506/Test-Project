using Fantasy;
using Hotfix.Helper;
using Model;
using Model.Gate;
using Model.Gate.Hall;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.System.Gate
{
    public static class HallPlayerManagerComponentSystem
    {
        /// <summary>
        /// 玩家断开连接
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public static void PlayerDisConnect(this HallPlayerManagerComponent self, long playerid)
        {
            
            foreach (var mapType in self.HallPlayerDic.Keys)
            {
                //当前断线玩家是否存在某个地图中
                if (!self.HallPlayerDic.TryGetValue(mapType,out var playerDic))
                {
                    //该地图中没有玩家
                    continue;
                }
                //移除离线玩家
                self.RemoveHallPlayer(mapType, playerid);

                //查找该玩家字典中其他的玩家
                List<HallPlayer> otherPlayerList= self.GetPlayerListInMap(mapType,playerid);
                if (otherPlayerList != null)
                {
                    OtherPlayerStateSyncMessage message = new OtherPlayerStateSyncMessage() { roleData = new StateSyncData { playerMapStatus = 2, playerid = playerid } };
                    foreach (var mapOtherPlayer in otherPlayerList)
                    {
                        //广播玩家离线消息给当前地图中的其他玩家
                        mapOtherPlayer.session.Send(message);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 添加玩家至对应的地图中
        /// </summary>
        /// <param name="self"></param>
        /// <param name="session"></param>
        /// <param name="mapType"></param>
        /// <param name="doorType"></param>
        /// <param name="playerid"></param>
        /// <param name="roleInfo"></param>
        /// <returns></returns>
        public static HallPlayer AddHallPlayer(this HallPlayerManagerComponent self, Session session, int mapType, int doorType, long playerid, Role roleInfo)
        {
            //字典中不存在这个玩家所要去的地图数据
            if (!self.HallPlayerDic.TryGetValue(mapType, out var playerDic))
            {
                //创建玩家
                HallPlayer player = Entity.Create<HallPlayer>(self.Scene, true);
                player.playerid = playerid;
                player.roleInfo = roleInfo;
                player.session = session;
                player.mapType = mapType;
                //玩家角色在新地图中初始化位置
                player.position = MapConfigConter.Instance.GetMapConfig((MapType)mapType).GetRoleInitPos((DoorType)doorType).ToSVector3();
                //缓存至字典
                self.HallPlayerDic.Add(mapType, new Dictionary<long, HallPlayer>() { { playerid, player } });
                return player;
            }
            //添加玩家至对应的地图中
            if (!playerDic.TryGetValue(playerid, out var hallPlayer))
            {
                //创建玩家
                HallPlayer player = Entity.Create<HallPlayer>(self.Scene, true);
                player.playerid = playerid;
                player.roleInfo = roleInfo;
                player.session = session;
                player.mapType = mapType;
                //玩家角色在新地图中初始化位置
                player.position = MapConfigConter.Instance.GetMapConfig((MapType)mapType).GetRoleInitPos((DoorType)doorType).ToSVector3();
                playerDic.Add(playerid, player);
                return player;
            }
            //该玩家已经在当前地图中
            return hallPlayer;
        }


        /// <summary>
        /// 移除当前角色所在的上一张地图
        /// </summary>
        /// <param name="self"></param>
        /// <param name="curMapType"></param>
        /// <param name="playerid"></param>
        public static void RemoveHallPlayer(this HallPlayerManagerComponent self, int curMapType, long playerid)
        {
            if (self.HallPlayerDic.TryGetValue(curMapType, out var playerDic))
            {
                if (playerDic.ContainsKey(playerid))
                {
                    HallPlayer player = playerDic[playerid];
                    player.Dispose();
                    playerDic.Remove(playerid);
                }
            }
        }
        /// <summary>
        /// 移除大厅地图上指定的玩家列表
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roleList"></param>
        public static void RemoveHallPlayer(this HallPlayerManagerComponent self, List<Role> roleList)
        {
            foreach (var item in self.HallPlayerDic)
            {
                if (item.Value==null)
                {
                    //这个地图上没有玩家
                    continue;
                }
                //地图上有玩家，把该玩家移除掉
                foreach (var role in roleList)
                {
                    item.Value.Remove(role.accountid);
                }
            }
        }
        /// <summary>
        /// 获取指定地图上的指定玩家
        /// </summary>
        /// <param name="self"></param>
        /// <param name="mapType"></param>
        /// <param name="playerid"></param>
        public static HallPlayer? GetPlayer(this HallPlayerManagerComponent self, int mapType, long playerid)
        {
            //验证该地图中是否存在该玩家
            if (!self.HallPlayerDic.TryGetValue(mapType, out var playerDic))
            {
                return null;
            }
            //该地图中存在玩家
            foreach (var item in playerDic)
            {
                if (item.Key == playerid)
                {
                    return item.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定地图上的所有的玩家
        /// </summary>
        /// <param name="self"></param>
        /// <param name="mapType"></param>
        /// <param name="filterPlayerid">过滤的玩家id</param>
        /// <returns></returns>
        public static List<HallPlayer>? GetPlayerListInMap(this HallPlayerManagerComponent self, int mapType, long filterPlayerid)
        {
            //当前玩家处于家里的时候是不需要广播
            if (mapType == (int)MapType.Home)
            {
                return null;
            }
            //地图字典中是否存在这个玩家或者这个地图
            if (!self.HallPlayerDic.TryGetValue(mapType, out var playerDic))
            {
                return null;
            }
            List<HallPlayer> playeList = new List<HallPlayer>();

            //该地图中存在玩家
            foreach (var item in playerDic)
            {
                //找到该地图上的其他玩家
                if (item.Key != filterPlayerid)
                {
                    //添加其他玩家
                    playeList.Add(item.Value);
                }
            }
            return playeList;
        }
        /// <summary>
        /// 验证玩家进入的地图是否存在
        /// </summary>
        /// <param name="self"></param>
        /// <param name="mapType"></param>
        /// <param name="doorType"></param>
        /// <returns></returns>
        public static uint VerifyHallEnterMap(this HallPlayerManagerComponent self, int mapType, int doorType)
        {
            MapConfig mapCfg = MapConfigConter.Instance.GetMapConfig((MapType)mapType);
            if (mapCfg == null)
            {
                return ErrorCode.EnterMapFailed_NotFindMap;
            }
            if (mapCfg.GetRoleInitPos((DoorType)doorType) == null)
            {
                return ErrorCode.EnterMapFailed_NotFindDoor;
            }
            return ErrorCode.Success;
        }

        /// <summary>
        /// 角色移动逻辑
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerid"></param>
        /// <param name="syncData"></param>
        /// <returns></returns>
        public static (uint errorCode, StateSyncData? serverSyncData) PlayerMove(this HallPlayerManagerComponent self, StateSyncData syncData)
        {
            HallPlayer player = self.GetPlayer(syncData.mapType, syncData.playerid);
            if (player == null)
            {
                //非法玩家，玩家不在当前地图中
                return (ErrorCode.PlayerMoveFailed_NotFindPlayerInMap, null);
            }
            //计算玩家需要移动的位置
            player.position.x += syncData.inputDir.x * self.fixDeltaTime * player.roleInfo.moveSpeed;
            player.position.y += syncData.inputDir.y * self.fixDeltaTime * player.roleInfo.moveSpeed;
            player.position.z += syncData.inputDir.z * self.fixDeltaTime * player.roleInfo.moveSpeed;
            //同步玩家最新位置
            syncData.position = player.position.ToCSVector3();
            Log.Info($"playerid:{syncData.playerid} position:{syncData.position}");
            return (ErrorCode.Success, syncData);
        }
    }
}
