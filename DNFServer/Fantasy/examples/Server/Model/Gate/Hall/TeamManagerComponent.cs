using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Gate.Hall
{
    public class TeamManagerComponent:Entity
    {
        //队伍id默认为5为数 (商业项目中队伍id会有专门的算法去生成这个id，具体是几位数需要根据产品的需求去确定)
        private int teamId = 10000;
        public int TeamId { get {return teamId++; } }
        /// <summary>
        /// 所有已经创建的队伍信息字典
        /// </summary>
        public Dictionary<int, List<Role>> allTeamInfoDic = new Dictionary<int, List<Role>>();

        /// <summary>
        /// 已经创建的队伍字典：key玩家id，value为队伍id
        /// </summary>
        public Dictionary<long, int> alreadyCreateTeamDic = new Dictionary<long, int>();

        /// <summary>
        /// 所有队伍中所有玩家加载副本进度缓存字典
        /// </summary>
        public Dictionary<int, Dictionary<long,float>> teamPlayerLoadProgressDic = new Dictionary<int, Dictionary<long, float>>();
    }
}
