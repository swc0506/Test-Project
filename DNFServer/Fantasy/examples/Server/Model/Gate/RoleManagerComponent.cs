using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Gate
{
    public class RoleManagerComponent:Entity
    {
        /// <summary>
        /// 角色数据缓存字典
        /// </summary>
        public Dictionary<int,Role> roleDataCacheDic=new Dictionary<int, Role>();

        /// <summary>
        /// 使用或选择的角色字典 key 玩家id value 玩家使用的角色
        /// </summary>
        public Dictionary<long, Role> useRoleCacheDic = new Dictionary<long, Role>();
    }
}
