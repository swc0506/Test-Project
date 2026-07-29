using System;
using System.Collections.Generic;
using CardsRPGGameServer.Socket;

public class RecruitHeroRequestHandler : HandlerBase
{
    public override void HandlerMsg(ClientUser client, byte[] data)
    {
        base.HandlerMsg(client, data);
        RecruitHeroRequest request = ProtoBuffSerialize.Deserialize<RecruitHeroRequest>(data);
        if (request != null)
        {
            RecruitHeroResponse response = new RecruitHeroResponse();
            int minId = ConfigCenter.HeroDataList[0].id;
            int maxId = ConfigCenter.HeroDataList[^1].id;

            List<int> rewardHeroList = new List<int>();
            Random random = new Random();

            var userData = DataCacheSystem.GetCacheData<UserData>(client.DeviceID);
            userData.HeroIdList ??= new List<int>();

            for (int i = 0; i < 10; i++)
            {
                int heroId = random.Next(minId, maxId);
                rewardHeroList.Add(heroId);
                
                if (!userData.HeroIdList.Contains(heroId))
                    userData.HeroIdList.Add(heroId);
            }
            
            DataCacheSystem.CacheData(client.DeviceID, userData);
            
            response.rewardIdList = rewardHeroList;
            client.SendPacket(Protocal.RecruitHeroResponse, response);
        }
    }
}