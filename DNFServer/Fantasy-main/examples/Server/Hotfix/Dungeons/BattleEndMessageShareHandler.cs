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
    public class BattleEndMessageShareHandler : Message<BattleEndMessage_Share>
    {
        protected override async FTask Run(Session session, BattleEndMessage_Share message)
        {
            //处理所有客户端上报过来的战斗结束的通知，收到所有客户端发送的战斗结束的通知时，表示战斗已结束
            var component = session.Scene.GetComponent<DungeonsManagerComponent>();
            var result= component.VerifyBattleIsEnd(message.battleid,message.playerid);
            if (result.battleisEnd)
            {
                //延迟2秒后发送战斗结束通知，给客户端表现的时间。目的：服务都在线上可以及调整
                session.Scene.TimerComponent.Core.OnceTimer(2000, () =>
                {
                    Log.Info("推送战斗结束的通知到客户端");
                    BattleEndMessage_Share battleEndMessage = new BattleEndMessage_Share() { battleid = message.battleid, playerid = message.playerid };
                    foreach (var item in result.playerDic)
                    {
                        item.Value.session.Send(battleEndMessage);
                    }
                } );
          
            }
            await FTask.CompletedTask;
        }
    }
}
