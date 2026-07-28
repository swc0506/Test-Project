/*--------------------------------------------------------------------------------------
* Title: 数据脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/24 11:30:45
* Description:数据层,主要负责游戏数据的存储、更新和获取
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace ZMGC.Hall
{
    public class UserDataMgr : IDataBehaviour
    {
        private UserData userData;
        
        public long UserId => userData.Id;
        public string UserName => userData.UserName;
        public Gender Gender => userData.Gender;
        public List<int> HeroIdList => userData.HeroIdList;
        public long Diamond => 999999;
        
        public void OnCreate()
        {
            Debugger.Log("UserDataMgr OnCreate");
        }

        public void OnDestroy()
        {
        }
        
        public void CacheUserData(UserData userData)
        {
            this.userData = userData;
        }
        
        public void UpdateHeroList(List<int> heroList)
        {
            userData.HeroIdList ??= new List<int>();

            foreach (int heroId in heroList)
            {
                if (!userData.HeroIdList.Contains(heroId))
                    userData.HeroIdList.Add(heroId);
            }
        }
    }
}