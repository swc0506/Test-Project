
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
                    client.SendPacket(Protocal.BattleResultResponse, response);
                });
        }
        else
        {
            Debugger.LogError("BattleResultRequestHandler HandlerMsg error: snapShotData not found: " + battleId);
            response.resultCode = ResultCode.BattleNotFind;
            client.SendPacket(Protocal.BattleResultResponse, response);
        }
    }
}