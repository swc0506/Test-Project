using Fantasy;
using Model;
using Model.Authentication;
using Model.Gate;
using Model.Gate.Hall;
using Model.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    /// <summary>
    /// Scene创建事件监听， 有任意一个Scene进行创建，这个回调都会触发
    /// </summary>
    public class OnSceneCreateCfg : AsyncEventSystem<OnCreateScene>
    {
        public override async FTask Handler(OnCreateScene self)
        {
            switch (self.Scene.SceneType)
            {
                case SceneType.Authentication:
                    self.Scene.AddComponent<AuthenticationAccountComponent>();
                    self.Scene.AddComponent<RSAEncryptComponent>();
                    break;
                case SceneType.Gate:
                    self.Scene.AddComponent<RSAEncryptComponent>();
                    self.Scene.AddComponent<RoleManagerComponent>();
                    self.Scene.AddComponent<HallPlayerManagerComponent>();
                    self.Scene.AddComponent<TeamManagerComponent>();
                    self.Scene.AddComponent<DungeonsManagerComponent>();
                    break;

            }
            await FTask.CompletedTask;
        }
    }
}
