
using System.Collections.Generic;
using CardsRPGGameServer.Socket;

public class BattleReplayRequestHandler : HandlerBase
{
    public override void HandlerMsg(ClientUser client, byte[] data)
    {
        BattleReplayDataRequest request = ProtoBuffSerialize.Deserialize<BattleReplayDataRequest>(data);
        BattleReplayDataResponse response = new BattleReplayDataResponse();
        
        string dataCacheName = DataCacheNameGeter.GetBattleReplayDataListKey(client.UserId);
        List<ReplayData> replayDataList = DataCacheSystem.GetCacheData<List<ReplayData>>(dataCacheName);
        response.replayDataList = replayDataList;
        response.resultCode = ResultCode.Success;
        client.SendPacket<BattleReplayDataResponse>(Protocal.BattleReplayDataListResponse, response);
    }
}