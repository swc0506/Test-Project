using System.Collections.Generic;
using ProtoBuf;

public class Packet
{
    [ProtoMember(1)] public int ResultCode;
}
    
[ProtoContract]
public class LoginRequest : Packet
{
    [ProtoMember(1)] public string DeviceID;
}
    
[ProtoContract]
public class LoginResponse : Packet
{
    [ProtoMember(1)] public int Id;
    [ProtoMember(2)] public string Name;
}

[ProtoContract]
public class HeroSeatDataPb
{
    [ProtoMember(1)] public int id;
    [ProtoMember(2)] public int seatId;
}

[ProtoContract]
public class BattleHeroDataPb
{
    [ProtoMember(1)] public int id;
    [ProtoMember(2)] public int seatId;
    [ProtoMember(3)] public int[] skillIdArr;
    [ProtoMember(4)] public int hp;
    [ProtoMember(5)] public int atk;
    [ProtoMember(6)] public int def;
    [ProtoMember(7)] public int agl;
    [ProtoMember(8)] public int atkRage;
    [ProtoMember(9)] public int takeDamageRage;
    [ProtoMember(10)] public int maxRage;

    public HeroData ToHeroData()
    {
        HeroData heroData = new HeroData();
        heroData.id = id;
        heroData.seatId = seatId;
        heroData.skillIdArr = skillIdArr;
        heroData.hp = hp;
        heroData.atk = atk;
        heroData.def = def;
        heroData.agl = agl;
        heroData.atkRage = atkRage;
        heroData.takeDamageRage = takeDamageRage;
        heroData.maxRage = maxRage;
        return heroData;
    }
}

/// <summary>
/// 开始战斗请求
/// </summary>
[ProtoContract]
public class StartBattleRequest : Packet
{
    [ProtoMember(1)] public List<HeroSeatDataPb> heroSeatDataList;
}

/// <summary>
/// 开始战斗响应
/// </summary>
[ProtoContract]
public class StartBattleResponse : Packet
{
    [ProtoMember(1)] public int battleId;// 战斗id
    [ProtoMember(2)] public int randomSeed; // 随机种子
    [ProtoMember(3)] public List<BattleHeroDataPb> heroDataList;// 英雄数据列表
    [ProtoMember(4)] public List<BattleHeroDataPb> enemyHeroDataList;// 敌方英雄数据列表
}

[ProtoContract]
public class BattleResultRequest : Packet
{
    [ProtoMember(1)] public int battleId;
}

[ProtoContract]
public class BattleResultResponse : Packet
{
    [ProtoMember(1)] public bool isWin;
    [ProtoMember(2)] public List<RewardData> rewardList;
}

[ProtoContract]
public class RewardData
{
    [ProtoMember(1)] public int itemId;
    [ProtoMember(2)] public int count;
}