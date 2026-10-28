using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ChildType]
    [ComponentOf(typeof(Scene))]
    public class RoleInfoComponent : Entity, IAwake, IDestroy 
    {
        public List<RoleInfo> RoleInfos = new List<RoleInfo>();
        public long CurrentRoleId = 0;
    }
}
