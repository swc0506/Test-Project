using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgHandleCenter : Singleton<MsgHandleCenter>
{
    public void OnCreate()
    {
        NetEventControl.AddEvent(Protocal.LoginResponse, OnLoginResponse);
    }
    
    public void SendLoginRequest()
    {
        Debugger.Log("SendLoginRequest");
        LoginRequest request = new LoginRequest();
        request.DeviceID = SystemInfo.deviceUniqueIdentifier;
        NetWorkManager.Instance.SendPacket(Protocal.LoginRequest, request);
    }

    public void OnLoginResponse(byte[] msgBytes)
    {
        LoginResponse response = ProtoBuffSerialize.Deserialize<LoginResponse>(msgBytes);
        if (response.ResultCode == 0)
        {
            Debugger.Log($"OnLoginResponse....{response.Id} name:{response.Name}");
            BattleWorldNodes.Instance.selectHeroWindowTrans.gameObject.SetActive(true);
            BattleWorldNodes.Instance.startWindowTrans.gameObject.SetActive(false);
        }
    }
    
    public void OnDestroy()
    {
        NetEventControl.RemoveEvent(Protocal.LoginResponse, OnLoginResponse);
    }
}
