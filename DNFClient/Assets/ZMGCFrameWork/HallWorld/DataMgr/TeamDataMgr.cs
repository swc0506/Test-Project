/*--------------------------------------------------------------------------------------
* Title: 数据脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/29 7:58:05
* Description:数据层,主要负责游戏数据的存储、更新和获取
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using System.Collections.Generic;

namespace ZMGC.Hall
{
    public class TeamDataMgr : IDataBehaviour
    {
        /// <summary>
        /// 队伍id
        /// </summary>
        public int TeamID { get; private set; }
        /// <summary>
        /// 队伍角色列表
        /// </summary>
        public List<RoleData> TeamRoleDataList { get; private set; }

        public void OnCreate()
        {
            TeamRoleDataList = new List<RoleData>();
        }

        public void CacheTeamRole(RoleData roleData, int teamid)
        {
            if (teamid > 0)
            {
                TeamID = teamid;
            }

            TeamRoleDataList.Add(roleData);
        }
        public void RemoveTeamRole(long roleUid)
        {
            for (int i = 0; i < TeamRoleDataList.Count; i++)
            {
                RoleData roleData = TeamRoleDataList[i];
                if (roleData.uid == roleUid)
                {
                    TeamRoleDataList.Remove(roleData);
                    break;
                }
            }
        }

        public void ClearTeamData()
        {
            TeamID = 0;
            TeamRoleDataList.Clear();
        }

        public void OnDestroy()
        {
            ClearTeamData();
        }

    }
}
