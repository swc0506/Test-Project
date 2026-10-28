using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ChildType]
    [ComponentOf(typeof(Scene))]
    public class ServerInfoManagerComponent : Entity, IAwake, IDestroy, ILoad
    {
        public List<ServerInfo> ServerInfos = new List<ServerInfo>();
    }
}
