using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ObjectSystem]
    public class RoleInfoComponentDestroySystem : DestroySystem<RoleInfoComponent>
    {
        public override void Destroy(RoleInfoComponent self)
        {
            foreach (var roleInfo in self.RoleInfos)
            {
                roleInfo?.Dispose();
            }
            self.RoleInfos.Clear();
            self.CurrentRoleId = 0;
        }
    }


    public static class RoleInfoComponentSystem
    {

    }
}
