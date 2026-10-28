using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [FriendClassAttribute(typeof(ET.ServerInfo))]
    public static class ServerInfoSystem
    {
        public static void FroMessage(this ServerInfo self, ServerInfoProto serverInfoProto)
        {
            self.Id = serverInfoProto.Id;
            self.Status = serverInfoProto.Staus;
            self.ServerName = serverInfoProto.ServerName;
        }

        public static ServerInfoProto ToMessage(this ServerInfo self)
        {
            return new ServerInfoProto() { Id = (int)self.Id, ServerName = self.ServerName, Staus = self.Status };
        }
    }
}
