using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model 
{
    public enum BattleStateEnum
    { 
        None,
        Start,
        End,
    }
    public class Dungeon:Entity
    {
        /// <summary>
        /// 战斗id
        /// </summary>
        public long battleid { get { return Id; } }
        /// <summary>
        /// 地下城中的玩家字典
        /// </summary>
        public Dictionary<long,BattlePlayer> playerDic=new Dictionary<long, BattlePlayer>();
        /// <summary>
        /// 地下城战斗状态
        /// </summary>
        public BattleStateEnum battleState;
        /// <summary>
        /// 逻辑帧id
        /// </summary>
        public long logicFrameid;

        /// <summary>
        /// 逻辑帧计时器id
        /// </summary>
        public long timerid;
    }
}
