/*--------------------------------------------------------------------------------------
* Title: 数据脚本自动生成工具
* Author: 铸梦xy
* Date:2024/2/11 22:53:35
* Description:数据层,主要负责游戏数据的存储、更新和获取
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Fantasy;

namespace ZMGC.Hall
{
    public class UserDataMgr : IDataBehaviour
    {
        public List<int> createRuleIdList { get; private set; } = new List<int> { 1000, 1001 };

        /// <summary>
        /// 已经创建的所有角色数据列表
        /// </summary>
        public List<RoleData> AllRoleDataList { get; private set; }

        public string userName;

        public string Account { get; private set; }

        public long AccountId { get; private set; }

        public long gold { get; private set; }

        public long diamonds { get; private set; }
        public long Level { get; private set; }

        public int RoleId=> Role.roleid;

        public RoleData Role { get; private set; }
        public void OnCreate()
        {
            Debug.Log("UserDataMgr OnCreate :");
        }
     
        public void CacheAccountData(LoginGateResponse accountData)
        {
            this.AccountId = accountData.accountid;
            this.gold= accountData.gold;
            this.diamonds = accountData.diamond;
            this.Level = accountData.level;
            this.AllRoleDataList = accountData.roleList;
        }
        public void CacheSelectRole(RoleData roleData)
        {
            this.Role = roleData;
        }
        public void OnDestroy()
        {

        }

    }
}
