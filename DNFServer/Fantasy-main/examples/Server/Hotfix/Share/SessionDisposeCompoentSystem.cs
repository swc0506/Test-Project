using Fantasy;
using Hotfix.System.Gate;
using Model;
using Model.Gate.Hall;
using Model.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Share
{
    public class SessionDisposeCompoentSystemDestroy : DestroySystem<SessionDisposeCompoent>
    {
        protected override void Destroy(SessionDisposeCompoent self)
        {
            if (self == null)
            {
                return;
            }
            Log.Info("SessionDisposeCompoentSystemDestroy" + self.accountId);

            //队伍玩家断线处理
            var teamComponent = self.Scene.GetComponent<TeamManagerComponent>();
            teamComponent.PlayerDisConnect(self.accountId);

            //处理玩家断线
            var hallPlayerComponent = self.Scene.GetComponent<HallPlayerManagerComponent>();
            hallPlayerComponent.PlayerDisConnect(self.accountId);
         
        }
    }

    public class SessionDisposeCompoentSystem
    {
    }
}
