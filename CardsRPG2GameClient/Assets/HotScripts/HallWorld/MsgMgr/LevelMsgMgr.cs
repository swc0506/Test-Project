/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2026/9/10 18:55:11
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

namespace ZMGC.Hall
{
    public class LevelMsgMgr : IMsgBehaviour
    {
        public void OnCreate()
        {
            NetEventControl.AddEvent(Protocal.BattleReplayDataListResponse, OnGetReplayDataListResponse);
        }

        public void OnDestroy()
        {
            NetEventControl.RemoveEvent(Protocal.BattleReplayDataListResponse, OnGetReplayDataListResponse);
        }

        public void SendGetReplayDataListRequest()
        {
            BattleReplayDataRequest req = new BattleReplayDataRequest();
            NetWorkManager.Instance.SendPacket(Protocal.BattleReplayDataListRequest, req);
        }
        
        private void OnGetReplayDataListResponse(byte[] data)
        {
            BattleReplayDataResponse response = ProtoBuffSerialize.Deserialize<BattleReplayDataResponse>(data);

            if (response.resultCode == 0 && response.replayDataList != null)
            {
                HallWorld.GetExitsLogicCtrl<LevelLogicCtrl>().HandlerReplayDataList(response.replayDataList);
            }
            else
            {
                
            }
        }
    }
}