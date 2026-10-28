using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public enum AccountType
    {
        General = 0,

        BlackList = 1,
    }

    public class Account : Entity,IAwake
    {
        public string AccountName;

        public string Password;

        public long CreatTime;

        public int AccountType;
    }
}
