/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/28 18:37:57
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace ZMGC.Hall
{
    public class RecruitLogicCtrl : ILogicBehaviour
    {
        private RecruitMsgMgr msgMgr;
        private RecruitDataMgr dataMgr;
        
        public void OnCreate()
        {
            msgMgr = HallWorld.GetExitsMsgMgr<RecruitMsgMgr>();
            dataMgr = HallWorld.GetExitsDataMgr<RecruitDataMgr>();
        }

        public void OnDestroy()
        {
        }
        
        /// <summary>
        ///  召唤
        /// </summary>
        /// <param name="single">是否十连抽</param>
        public int RecruitSingle(bool single)
        {

            if (HallWorld.UserData.Diamond < 100)
            {
                return 1;
            }
            
            msgMgr.SendRecruitReq(single);
            return 0;
        }
        
        /// <summary>
        /// 召唤成功
        /// </summary>
        /// <param name="heroDataList"></param>
        public void OnRecruitSuccess(List<int> heroDataList)
        {
            // 更新用户拥有的英雄列表
            HallWorld.UserData.UpdateHeroList(heroDataList);
            // 缓存召唤数据
            dataMgr.CacheRecruitData(heroDataList);
        }
    }
}