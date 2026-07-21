
using CardsRPGGameServer.Proto;
using CardsRPGGameServer.Socket;

public class BattleResultRequestHandler : HandlerBase
{
    public override void HandlerMsg(ClientUser client, byte[] data)
    {
        base.HandlerMsg(client, data);
        BattleResultRequest request = ProtoBuffSerialize.Deserialize<BattleResultRequest>(data);

        int battleId = request.battleId;

        UserBattleData battleData = client.GetBattleData(battleId);
        if (battleData != null)
        {
            BattleResultResponse response = new BattleResultResponse();
            response.isWin = battleData.isWin;
            response.rewardList = battleData.rewardList;
            Debugger.Log("BattleResultRequestHandler HandlerMsg: isWin: " + response.isWin);
            client.SendPacket(Protocal.BattleResultResponse, response);
        }
        else
        {
            Debugger.LogError("BattleResultRequestHandler HandlerMsg error: battleData not found: " + battleId);
        }
    }
}