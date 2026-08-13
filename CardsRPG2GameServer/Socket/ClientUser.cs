using System;
using System.Collections.Generic;
using Fleck;

namespace CardsRPGGameServer.Socket;

public class UserBattleData
{
    public long battleId;
    public bool isWin;
    public List<RewardData> rewardList;
}

public class UserBattleSnapShotData
{
    /// <summary>
    /// 战斗id
    /// </summary>
    public long battleId;
    /// <summary>
    /// 随机种子
    /// </summary>
    public int randomSeed;
    // 英雄数据列表
    public List<HeroData> heroDataList;
    // 敌人数据列表
    public List<HeroData> enemyDataList;
    // 战斗开始快照数据
    public StartBattleResponse startBattleResponse;
}

public class ClientUser : ClientSocket
{ 
    public string DeviceID { get; set; } 
    
    public int BattleId { get; set; }
    
    public long UserId { get; private set; }
    
    public string UserName { get; private set; }
    
    public Gender Gender { get; private set; }
    
    public List<UserBattleData> battleDataList = new List<UserBattleData>();
    public List<UserBattleSnapShotData> snapShotDataList = new List<UserBattleSnapShotData>();
    
    public ClientUser(string url, IWebSocketConnection socket) : base(url, socket)
    {
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void CacheUserData(UserData userData)
    {
        UserId = userData.Id;
        UserName = userData.UserName;
        Gender = userData.Gender;
    }

    public void CacheBattleData(long battleId, bool isWin)
    {
        battleDataList.Add(new UserBattleData
        {
            battleId = battleId,
            isWin = isWin
        });
    }

    /// <summary>
    /// 缓存快照数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="heroDataList"></param>
    /// <param name="enemyDataList"></param>
    /// <returns></returns>
    public UserBattleSnapShotData CacheBattleSnapShotData(StartBattleResponse data, List<HeroData> heroDataList, List<HeroData> enemyDataList)
    {
        var snapShotData = new UserBattleSnapShotData()
        {
            battleId = data.battleId,
            randomSeed = data.randomSeed,
            heroDataList = heroDataList,
            enemyDataList = enemyDataList,
            startBattleResponse = data
        };
        
        snapShotDataList.Add(snapShotData);
        return snapShotData;
    }

    public UserBattleData GetBattleData(long battleId)
    {
        foreach (var data in battleDataList)
        {
            if (data.battleId == battleId)
            {
                return data;
            }
        }
        
        return null;
    }
    
    public UserBattleSnapShotData GetUserBattleSnapShotData(long battleId)
    {
        foreach (var data in snapShotDataList)
        {
            if (data.battleId == battleId)
            {
                return data;
            }
        }
        
        return null;
    }

    /// <summary>
    ///  生成战斗id
    /// </summary>
    /// <returns></returns>
    public long GenerateBattleId()
    {
        return DateTime.Now.Ticks;
    }
}