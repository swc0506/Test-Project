/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2026/9/10 19:09:45
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace ZMGC.Hall
{
    public class LevelLogicCtrl : ILogicBehaviour
    {
        private LevelDataMgr _levelDataMgr;
        
        public void OnCreate()
        {
            _levelDataMgr = HallWorld.GetExitsDataMgr<LevelDataMgr>();
        }

        public void OnDestroy()
        {
        }

        public void HandlerReplayDataList(List<ReplayData> replayDataList)
        {
            // 缓存 通知
            _levelDataMgr.CacheReplayData(replayDataList);
            UIEventControl.DispensEvent(UIEventEnum.AngerChange);
        }
    }
}