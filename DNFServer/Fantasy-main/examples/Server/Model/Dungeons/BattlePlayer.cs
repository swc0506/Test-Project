using Fantasy;
using Model.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BattlePlayer :Entity
    {
        /// <summary>
        /// 玩家id
        /// </summary>
        public long playerid;
        /// <summary>
        /// 玩家角色信息
        /// </summary>
        public Role roleInfo;
        /// <summary>
        /// 玩家会话
        /// </summary>
        public Session session;

        public bool battleisEnd = false;

        public override void Dispose()
        {
            base.Dispose();
            playerid = 0;
            roleInfo = null;
            session = null;
            battleisEnd = false;
        }
    }
}
