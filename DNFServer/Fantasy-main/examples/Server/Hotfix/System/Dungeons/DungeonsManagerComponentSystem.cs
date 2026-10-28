using Fantasy;
using Model;
using Model.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.System.Dungeons
{
    public static class DungeonsManagerComponentSystem
    {
        /// <summary>
        /// 移除地下城中的玩家
        /// </summary>
        /// <param name="self"></param>
        /// <param name="battleid"></param>
        /// <param name="playerid"></param>
        public static void RemoveDungeonPlayer(this DungeonsManagerComponent self, long battleid ,long playerid)
        {
            if (self.AllDungeonsDic.TryGetValue(battleid, out var dungeon))
            {
                 dungeon.playerDic.Remove(playerid);
            }
        }
        /// <summary>
        /// 获取地下城中的所有的玩家通过战斗di
        /// </summary>
        /// <param name="self"></param>
        /// <param name="battleid"></param>
        /// <returns></returns>
        public static  Dictionary<long, BattlePlayer>  GetDungeonPlayerDic(this DungeonsManagerComponent self, long battleid)
        {
            if (self.AllDungeonsDic.TryGetValue(battleid, out var dungeon))
            {
               return dungeon.playerDic;
            }
            return null;
        }
        /// <summary>
        /// 校验战斗是否结束
        /// </summary>
        /// <param name="self"></param>
        /// <param name="battleid"></param>
        /// <param name="playerid"></param>
        public static (bool battleisEnd,Dictionary<long,BattlePlayer> playerDic) VerifyBattleIsEnd(this DungeonsManagerComponent self, long battleid, long playerid)
        {
            self.AllDungeonsDic.TryGetValue(battleid, out var dungeon);
            if (dungeon == null)
            {
                Log.Info("该战斗不存在，战斗结束验证失败");
                return (false, null);
            }

            //这场战斗中不存在这个玩家
            if (!dungeon.playerDic.TryGetValue(playerid,out var battlePlayer))
            {
                Log.Info("该战斗存在，但战斗中的玩家不存在");
                return (false, null);
            }

            //所有玩家是否战斗结束
            battlePlayer.battleisEnd = true;
            foreach (var item in dungeon.playerDic)
            {
                if (!item.Value.battleisEnd)
                {
                    return (false,null);
                }
            }
            //战斗已经结束
            return (true, dungeon.playerDic);
        }

        /// <summary>
        /// 开始地下城战斗
        /// </summary>
        /// <param name="self"></param>
        /// <param name="rolelist"></param>
        public static void StartDungeonBattle(this DungeonsManagerComponent self, List<Role> rolelist)
        {
            //创建地下城
            Dungeon dungeon = Entity.Create<Dungeon>(self.Scene, false);
            self.AllDungeonsDic.Add(dungeon.battleid, dungeon);

            //缓存地下城中所有的战斗玩家
            dungeon.playerDic = new Dictionary<long, BattlePlayer>();
            foreach (Role role in rolelist)
            {
                BattlePlayer player = Entity.Create<BattlePlayer>(self.Scene, false);
                player.playerid = role.accountid;
                player.session = role.session;
                player.roleInfo = role;
                dungeon.playerDic.Add(player.playerid, player);
            }
            //开始战斗，推送逻辑帧更新逻辑
            dungeon.BattleStart();
        }

        public static void DungeonBattleEnd(this DungeonsManagerComponent self, long battleid)
        {
            //释放当前地下城，停止逻辑帧推送
            if (self.AllDungeonsDic.TryGetValue(battleid,out var dungeon))
            {
                dungeon.Dispose();
                self.AllDungeonsDic.Remove(battleid);
            }
        }
        public static void OnPlayerOperateFrameInput(this DungeonsManagerComponent self, long battleid, List<FrameOperateData> frameDataList)
        {
            //当前战斗是否存在
            self.AllDungeonsDic.TryGetValue(battleid ,out var dungeon);
            if (dungeon == null)
            {
                Log.Info("操作帧同步无效，地下城战斗不存在"+battleid);
                return;
            }
            //缓存玩家操作
            dungeon.SyncPlayerFrameData(battleid,frameDataList);
        }
    }
}
