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
            NetEventControl.AddEvent(Protocal.CreateUserResponse, OnCreateUserResponse);
        }

        public void OnDestroy()
        {
            NetEventControl.RemoveEvent(Protocal.LoginResponse, OnLoginResponse);
            NetEventControl.RemoveEvent(Protocal.CreateUserResponse, OnCreateUserResponse);
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
        
        public void SendCreateUserReq(string userName, Gender gender)
        {
            CreateUserRequest req = new CreateUserRequest();
            req.userName = userName;
            req.gender = gender;
            req.deviceId = SystemInfo.deviceUniqueIdentifier;
            NetWorkManager.Instance.SendPacket(Protocal.CreateUserRequest, req);
        }
        
        private void OnCreateUserResponse(byte[] packet)
        {
            CreateUserResponse resp = ProtoBuffSerialize.Deserialize<CreateUserResponse>(packet);
            if (resp.resultCode == ResultCode.Success)
            {
                HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>().CreateUserSuccess(resp.userData);
            }
            else
            {
                ToastManager.ShowToast(resp.resultCode.ToString());
            }
        }
        
        #endregion
    }
}