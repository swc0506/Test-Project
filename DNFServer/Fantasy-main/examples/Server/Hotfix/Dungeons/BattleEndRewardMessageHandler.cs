using Fantasy;
using Hotfix.System.Dungeons;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class BattleEndRewardMessageHandler : Message<BattleEndRewardMessage_Share>
    {
        protected override async FTask Run(Session session, BattleEndRewardMessage_Share message)
        {
            var component = session.Scene.GetComponent<DungeonsManagerComponent>();
            //获取制定战斗中的所有的玩家
            var playerDic =  component.GetDungeonPlayerDic(message.battleid);
            if (playerDic != null)
            {
                //单纯做一个奖励领取转发，因为当前案例没有制作装备系统，物品库等一些功能，这个案例已核心功能为主。
                BattleEndRewardMessage_Share rewardMessage =new BattleEndRewardMessage_Share() {  cardRewardIndex=message.cardRewardIndex,battleid=message.battleid,playerid=message.playerid};
                foreach (var player in playerDic)
                {
                    player.Value.session.Send(rewardMessage);
                }
            }
            await FTask.CompletedTask;
        }
    }
}
