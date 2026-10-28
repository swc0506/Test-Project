using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ComponentOf(typeof(Session))]
    public class AccountCheckoutTimeComponent : Entity,IAwake<long>,IDestroy
    {
        public long Timer = 0;

        public long AccountId = 0;
    }
}
