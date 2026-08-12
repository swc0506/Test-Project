
using System;
using System.Collections.Generic;
using CardsRPGGameServer.Socket;
using LogicLayer;

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

        List<HeroData> heroDataList = new List<HeroData>();
        for (int i = 0; i < request.heroSeatDataList.Count; i++)
        {
            HeroSeatDataPb heroSeatDataPb = request.heroSeatDataList[i];
            HeroData heroData = ConfigCenter.GetHeroData(heroSeatDataPb.id);
            
            heroData.seatId = heroSeatDataPb.seatId;
            
            response.heroDataList.Add(heroData.ToBattleHeroDataPb());
            heroDataList.Add(heroData);
        }
        
        // 获取敌人数据
        LevelData levelData = ConfigCenter.GetLevelData(request.levelId);

        if (levelData == null)
        {
            Debugger.Log($"关卡：{request.levelId}不存在， 战斗开始失败");
            response.result = ResultCode.LevelNotFind;
            client.SendPacket(Protocal.StartBattleResponse, response);
            return;
        }
        
        // 计算敌人的数据列表
        List<HeroData> enemyHeroDataList = new List<HeroData>();
        for (int i = 0; i < levelData.enemys.Count; i++)
        {
            HeroData enemyHeroData = ConfigCenter.GetHeroData(levelData.enemys[i]);
            enemyHeroData.seatId = i;
            response.enemyHeroDataList.Add(enemyHeroData.ToBattleHeroDataPb());
            enemyHeroDataList.Add(enemyHeroData);
        }
        
        // 生成随机种子
        response.result = ResultCode.Success;
        response.battleId = client.GenerateBattleId();
        Random random = new Random();
        response.randomSeed = random.Next(0, 100);
        client.SendPacket(Protocal.StartBattleResponse, response);
        Debugger.Log("随机种子： " + response.randomSeed);
        
        //计算战斗结果
        BattleWorldManager.CreateBattleWorld(heroDataList, enemyHeroDataList, response.randomSeed, response.battleId,
            (battleWorld) =>
            {
                //缓存战斗结果
                //client.CacheBattleData(response.battleId, battleWorld.isWin);
            });
    }
}