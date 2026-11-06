using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Authentication
{
    public  class AuthenticationAccountComponent:Entity
    {
        public readonly Dictionary<int,Account> accountCacheDic=new Dictionary<int, Account>();
    }
}
