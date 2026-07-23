using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicLayer;

public class MsgHandleCenter : Singleton<MsgHandleCenter>
{
    public void OnCreate()
    {
        NetEventControl.AddEvent(Protocal.LoginResponse, OnLoginResponse);
        NetEventControl.AddEvent(Protocal.StartBattleResponse, OnStartBattleResponse);
        NetEventControl.AddEvent(Protocal.BattleResultResponse, OnBattleResultResponse);
    }
    
    public void OnDestroy()
    {
        NetEventControl.RemoveEvent(Protocal.LoginResponse, OnLoginResponse);
        NetEventControl.RemoveEvent(Protocal.StartBattleResponse, OnStartBattleResponse);
        NetEventControl.RemoveEvent(Protocal.BattleResultResponse, OnBattleResultResponse);
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
    
    /// <summary>
    /// 发送开始战斗请求
    /// </summary>
    /// <param name="heroList"></param>
    public void SendStartBattleRequest(List<HeroSeatDataPb> heroList)
    {
        StartBattleRequest request = new StartBattleRequest();
        request.heroSeatDataList = heroList;
        NetWorkManager.Instance.SendPacket(Protocal.StartBattleRequest, request);
    }
    
    public void OnStartBattleResponse(byte[] msgBytes)
    {
        StartBattleResponse response = ProtoBuffSerialize.Deserialize<StartBattleResponse>(msgBytes);
        if (response.ResultCode == 0)
        {
            Debugger.Log($"OnStartBattleResponse....{response.randomSeed} battleId:{response.battleId}");
            BattleWorldNodes.Instance.selectHeroWindowTrans.gameObject.SetActive(false);
            List<HeroData> enemyList = new List<HeroData>();
            List<HeroData> heroList = new List<HeroData>();
            foreach (var item in response.heroDataList)
            {
                heroList.Add(item.ToHeroData());
            }
            foreach (var item in response.enemyHeroDataList)
            {
                enemyList.Add(item.ToHeroData());
            }
            LogicLayer.WorldManager.CreateBattleWorld(heroList, enemyList, response.randomSeed, response.battleId);
        }
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="battleId"></param>
    public void SendBattleResultRequest(int battleId)
    {
        BattleResultRequest request = new BattleResultRequest();
        request.battleId = battleId;
        NetWorkManager.Instance.SendPacket(Protocal.BattleResultRequest, request);
    }
    
    public void OnBattleResultResponse(byte[] msgBytes)
    {
        BattleResultResponse response = ProtoBuffSerialize.Deserialize<BattleResultResponse>(msgBytes);
        if (response.ResultCode == 0)
        {
            Debugger.Log($"OnBattleResultResponse....{response.isWin}");
            LogicLayer.WorldManager.BattleWorld.BattleEnd(response.isWin);
        }
    }
}
