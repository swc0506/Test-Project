/*--------------------------------------------------------------------------------------
* Title: 数据脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/28 18:38:44
* Description:数据层,主要负责游戏数据的存储、更新和获取
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace ZMGC.Hall
{
    public class RecruitDataMgr : IDataBehaviour
    {
        public List<int> RecruitHeroList { get; private set; }

        public void OnCreate()
        {
        }

        public void OnDestroy()
        {
        }

        public void CacheRecruitData(List<int> heroList)
        {
            RecruitHeroList = heroList;
        }
    }
}