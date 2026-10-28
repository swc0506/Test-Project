using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ChildType]
    [ComponentOf(typeof(Scene))]
    public class ServerInfosComponent : Entity,IAwake,IDestroy
    {
        public List<ServerInfo> ServerInfoList = new List<ServerInfo>();

        public int CurrentServerId = 0;
    }
}
