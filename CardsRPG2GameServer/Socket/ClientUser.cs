using System.Collections.Generic;
using Fleck;

namespace CardsRPGGameServer.Socket;

public class UserBattleData
{
    public int battleId;
    public bool isWin;
    public List<RewardData> rewardList;
}

public class ClientUser : ClientSocket
{ 
    public string DeviceID { get; set; } 
    
    public int BattleId { get; set; }
    
    public long UserId { get; private set; }
    
    public string UserName { get; private set; }
    
    public Gender Gender { get; private set; }
    
    public List<UserBattleData> battleDataList = new List<UserBattleData>();
    
    public ClientUser(string url, IWebSocketConnection socket) : base(url, socket)
    {
    }

    public void CacheUserData(UserData userData)
    {
        UserId = userData.Id;
        UserName = userData.UserName;
        Gender = userData.Gender;
    }

    public void CacheBattleData(int battleId, bool isWin)
    {
        battleDataList.Add(new UserBattleData
        {
            battleId = battleId,
            isWin = isWin
        });
    }

    public UserBattleData GetBattleData(int battleId)
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
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}