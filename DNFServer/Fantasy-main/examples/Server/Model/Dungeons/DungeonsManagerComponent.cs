using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DungeonsManagerComponent:Entity
    {
        /// <summary>
        /// 所有地下城字典 key：战斗id，value ：地下城，每个地下城根据玩家或队伍进入的数量开启多个
        /// </summary>
        public Dictionary<long,Dungeon> AllDungeonsDic = new Dictionary<long,Dungeon>();
    }
}
