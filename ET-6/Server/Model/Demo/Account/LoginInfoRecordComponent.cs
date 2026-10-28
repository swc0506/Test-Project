using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class LoginInfoRecordComponent : Entity,IAwake,IDestroy
    {
        public Dictionary<long, int> AccountLoginInfoDict = new Dictionary<long, int>();
    }
}
