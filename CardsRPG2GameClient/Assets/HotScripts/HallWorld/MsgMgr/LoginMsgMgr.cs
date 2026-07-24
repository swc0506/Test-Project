/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/24 11:28:20
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using UnityEngine;

namespace ZMGC.Hall
{
    public class LoginMsgMgr : IMsgBehaviour
    {
        public void OnCreate()
        {
            Debugger.Log("LoginMsgMgr OnCreate");
            NetEventControl.AddEvent(Protocal.LoginResponse, OnLoginResponse);
        }

        public void OnDestroy()
        {
            NetEventControl.RemoveEvent(Protocal.LoginResponse, OnLoginResponse);
        }

        #region 网络消息处理函数
        
        public void SendLoginReq()
        {
            LoginRequest req = new LoginRequest();
            req.DeviceID = SystemInfo.deviceUniqueIdentifier;
            NetWorkManager.Instance.SendPacket(Protocal.LoginRequest, req);
        }


        private void OnLoginResponse(byte[] packet)
        {
            LoginResponse resp = ProtoBuffSerialize.Deserialize<LoginResponse>(packet);
            if (resp.ResultCode == ResultCode.Success)
            {
                HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>().LoginSuccess(resp.UserData);
            }
            else
            {
                HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>().LoginFailed(resp.ResultCode);
            }
        }
        
        #endregion
    }
}