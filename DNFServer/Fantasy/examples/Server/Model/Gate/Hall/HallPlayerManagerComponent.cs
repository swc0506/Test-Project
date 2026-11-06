using Fantasy;
using Model.Gate.Hall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class HallPlayerManagerComponent:Entity
    {
        /// <summary>
        /// FixUpdate更新频率
        /// </summary>
        public float fixDeltaTime = 0.02f;
        /// <summary>
        /// 大厅所有玩家字典 key为地图类型， value为玩家（value.key 玩家账户id value.vale玩家对象类 ）
        /// </summary>
        public Dictionary<int,Dictionary<long,HallPlayer>> HallPlayerDic=new Dictionary<int, Dictionary<long, HallPlayer>>();
    }
}
