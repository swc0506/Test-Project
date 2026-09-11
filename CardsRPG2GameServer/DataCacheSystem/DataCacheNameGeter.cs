
public class DataCacheNameGeter
{
    /// <summary>
    /// 获取快照数据的key
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="battleId"></param>
    /// <returns></returns>
    public static string GetSnapShotDataKey(long userId, long battleId)
    {
        return $"{userId}_{battleId}_SnapShotData";
    }
    
    /// <summary>
    /// 获取战斗回放数据文件名称
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    public static string GetBattleReplayDataListKey(long userid)
    {
        return $"{userid}_BattleReplayData";
    }
}