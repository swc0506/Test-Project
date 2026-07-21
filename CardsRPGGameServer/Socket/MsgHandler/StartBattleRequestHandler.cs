
using System;
using System.Collections.Generic;
using CardsRPGGameServer.Proto;
using CardsRPGGameServer.Socket;

public class StartBattleRequestHandler : HandlerBase
{
    public override void HandlerMsg(ClientUser client, byte[] data)
    {
        base.HandlerMsg(client, data);
        StartBattleRequest request = ProtoBuffSerialize.Deserialize<StartBattleRequest>(data);
        
        //处理战斗数据
        StartBattleResponse response = new StartBattleResponse();
        response.heroDataList = new List<BattleHeroDataPb>();
        response.enemyHeroDataList = new List<BattleHeroDataPb>();
        response.ResultCode = 0;
        response.battleId = client.BattleId++;
        Random random = new Random();
        response.randomSeed = random.Next(0, 100);

        List<HeroData> heroDataList = new List<HeroData>();
        for (int i = 0; i < request.heroSeatDataList.Count; i++)
        {
            HeroSeatDataPb heroSeatDataPb = request.heroSeatDataList[i];
            HeroData heroData = ConfigCenter.GetHeroData(heroSeatDataPb.id);

            BattleHeroDataPb battleHeroDataPb = heroData.ToBattleHeroDataPb();
            heroData.seatId = battleHeroDataPb.seatId = heroSeatDataPb.seatId;
            
            response.heroDataList.Add(battleHeroDataPb);
            heroDataList.Add(heroData);
        }

        for (int i = 0; i < ConfigCenter.EnemyDataList.Count; i++)
        {
            response.enemyHeroDataList.Add(ConfigCenter.EnemyDataList[i].ToBattleHeroDataPb());
        }
        
        client.SendPacket(Protocal.StartBattleResponse, response);
        
        //计算战斗结果
        WorldManager.CreateBattleWorld(heroDataList, ConfigCenter.EnemyDataList, response.randomSeed, response.battleId,
            (battleWorld) =>
            {
                //缓存战斗结果
                client.CacheBattleData(response.battleId, battleWorld.isWin);
            });
    }
}