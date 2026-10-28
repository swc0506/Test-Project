using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Account:Entity,ISupportedDataBase
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string account;
        /// <summary>
        /// 密码
        /// </summary>
        public string passWorld;
        /// <summary>
        /// 金币
        /// </summary>
        public long gold;
        /// <summary>
        /// 钻石
        /// </summary>
        public long diamond;
        /// <summary>
        /// 账号登录时间
        /// </summary>
        public long loginTime;
        /// <summary>
        /// 账号创建时间
        /// </summary>
        public long createTime;
    }
}
