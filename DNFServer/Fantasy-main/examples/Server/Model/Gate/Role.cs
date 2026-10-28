using Fantasy;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Gate
{
    public class Role:Entity,ISupportedDataBase
    {
        /// <summary>
        /// 角色所属账户
        /// </summary>
        public long accountid;
        /// <summary>
        /// 角色Cfgid
        /// </summary>
        public int roleid;
        /// <summary>
        /// 角色等级
        /// </summary>
        public int level;
        /// <summary>
        /// 角色名称
        /// </summary>
        public string roleName;
        /// <summary>
        /// 移动速度
        /// </summary>
        public float moveSpeed=10;
        /// <summary>
        /// 当前玩家的会话
        /// </summary>
        [BsonIgnore] //忽略此字段，不让其写入到数据中
        public Session session;
    }
}
