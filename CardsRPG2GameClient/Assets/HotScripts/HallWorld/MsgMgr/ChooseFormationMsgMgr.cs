/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/31 17:59:44
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace ZMGC.Hall
{
    public class ChooseFormationMsgMgr : IMsgBehaviour
    {
        public void OnCreate()
        {
            NetEventControl.AddEvent(Protocal.StartBattleResponse, StartFightRsp);
        }

        public void OnDestroy()
        {
            NetEventControl.RemoveEvent(Protocal.StartBattleResponse, StartFightRsp);
        }

        public void SendStartFightReq(List<HeroSeatDataPb> heroIdList, int levelId)
        {
            StartBattleRequest req = new StartBattleRequest();
            req.heroSeatDataList = heroIdList;
            req.levelId = levelId;
            NetWorkManager.Instance.SendPacket(Protocal.StartBattleRequest, req);
        }

        private void StartFightRsp(byte[] data)
        {
            StartBattleResponse response = ProtoBuffSerialize.Deserialize<StartBattleResponse>(data);
            if (response.result == ResultCode.Success)
            {
                HallWorld.GetExitsLogicCtrl<ChooseFormationLogicCtrl>().StartFightSuccess(response);
            }
            else
            {
                ToastManager.ShowToast(response.result.ToString());
            }
        }
    }
}