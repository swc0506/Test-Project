using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Gate.Hall
{
    public class HallPlayer : Entity
    {
        public long playerid;//玩家id
        public Role roleInfo;//角色信息
        public Session session;//当前玩家的会话
        public int mapType;//当前所在的地图

        public SVector3 position;//当前玩家所处的坐标

        public override void Dispose()
        {
            base.Dispose();
            playerid = 0;
            roleInfo = null;
            session = null;
            mapType = 0;
            position = null;
        }
    
    }
}
