using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ObjectSystem]
    public class ServerInfosComponentDestroySystem : DestroySystem<ServerInfosComponent>
    {
        public override void Destroy(ServerInfosComponent self)
        {
            foreach (var serverIfo in self.ServerInfoList)
            {
                serverIfo?.Dispose();
            }
            self.ServerInfoList.Clear();
        }
    }


    [FriendClass(typeof(ServerInfosComponent))]
    public static class ServerInfosComponentSystem
    {
        public static void Add(this ServerInfosComponent self, ServerInfo serverInfo)
        {
            self.ServerInfoList.Add(serverInfo);
        }
    }
}
