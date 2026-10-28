using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public enum RoleInfoState
    {
        Normal = 0,
        Freeze = 1,
    }

    public class RoleInfo : Entity,IAwake
    {
        public string Name;

        public int ServerId;

        public int State;

        public long AccountId;

        public long LastLoginTine;

        public long CreatTime;
    }
}
