
using System;
using System.Collections.Generic;
using CardsRPGGameServer.Socket;
using LogicLayer;

public class BattleResultRequestHandler : HandlerBase
{
    public override void HandlerMsg(ClientUser client, byte[] data)
    {
        base.HandlerMsg(client, data);
        BattleResultRequest request = ProtoBuffSerialize.Deserialize<BattleResultRequest>(data);
        BattleResultResponse response = new BattleResultResponse();
        long battleId = request.battleId;

        var snapShotData = client.GetUserBattleSnapShotData(battleId);
        if (snapShotData != null)
        {
            //计算战斗结果
            BattleWorldManager.CreateBattleWorld(snapShotData.heroDataList, snapShotData.enemyDataList, snapShotData.randomSeed, snapShotData.battleId,
                (battleWorld) =>
                {
                    //缓存战斗结果
                    //client.CacheBattleData(response.battleId, battleWorld.isWin);
                    response.resultCode = ResultCode.Success;
                    response.isWin = battleWorld.IsWin;
                    response.rewardList = new List<RewardData>();
                    Debugger.Log("BattleResultRequestHandler HandlerMsg: isWin: " + response.isWin);
                    UpdateReplayDataList(client, battleId, response.isWin);
                    client.SendPacket(Protocal.BattleResultResponse, response);
                }, request.HeroSkillInputDataList);
        }
        else
        {
            Debugger.LogError("BattleResultRequestHandler HandlerMsg error: snapShotData not found: " + battleId);
            response.resultCode = ResultCode.BattleNotFind;
            client.SendPacket(Protocal.BattleResultResponse, response);
        }
    }

    private void UpdateReplayDataList(ClientUser client,long battleId,bool isWin)
    {
        //构建战斗回放数据
        ReplayData replayData = new ReplayData()
        {
            battleId = battleId,
            isWin = isWin,
            battleTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        };
        //获取战斗回放数据列表名称
        string dataCacheName = DataCacheNameGeter.GetBattleReplayDataListKey(client.UserId);
        //本地数据中心不存该用户数据
        if (!DataCacheSystem.CacheFileExist(dataCacheName))
        {
            DataCacheSystem.CacheData(dataCacheName,new List<ReplayData>(){replayData});
            return;
        }
        //获取所有回放数据
        List<ReplayData> rePlayDataList = DataCacheSystem.GetCacheData<List<ReplayData>>(dataCacheName);
        //按照时间排序，最新的战斗记录放到列表首位
        rePlayDataList.Insert(0,replayData);
        //缓存数据
        DataCacheSystem.CacheData(dataCacheName,rePlayDataList);

    }
}