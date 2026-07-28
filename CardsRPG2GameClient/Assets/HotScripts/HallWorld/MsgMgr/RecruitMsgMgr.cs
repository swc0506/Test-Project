/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/28 18:38:13
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

namespace ZMGC.Hall
{
    public class RecruitMsgMgr : IMsgBehaviour
    {
        public void OnCreate()
        {
            NetEventControl.AddEvent(Protocal.RecruitHeroResponse, OnRecruitResponse);
        }

        public void OnDestroy()
        {
            NetEventControl.RemoveEvent(Protocal.RecruitHeroResponse, OnRecruitResponse);
        }

        /// <summary>
        ///  召唤请求
        /// </summary>
        public void SendRecruitReq(bool single)
        {
            RecruitHeroRequest request = new RecruitHeroRequest();
            request.single = single;
            NetWorkManager.Instance.SendPacket(Protocal.RecruitHeroRequest, request);
        }

        /// <summary>
        ///  召唤响应
        /// </summary>
        /// <param name="packet"></param>
        private void OnRecruitResponse(byte[] packet)
        {
            RecruitHeroResponse response = ProtoBuffSerialize.Deserialize<RecruitHeroResponse>(packet);
            if (response.resultCode == ResultCode.Success)
            {
                HallWorld.GetExitsLogicCtrl<RecruitLogicCtrl>().OnRecruitSuccess(response.rewardIdList);
            }
            else
            {
                ToastManager.ShowToast(response.resultCode.ToString());
            }
        }
    }
}