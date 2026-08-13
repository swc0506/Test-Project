
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
}