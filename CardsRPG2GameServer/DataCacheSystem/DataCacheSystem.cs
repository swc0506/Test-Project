using System.IO;
using System;
using Newtonsoft.Json;

/// <summary>
/// 用户数据库系统
/// </summary>
public class DataCacheSystem
{
    // 数据缓存路径
    private static string DataCachePath;
    
    // 文件后缀
    private static string fileSuffex = ".json";

    /// <summary>
    ///  初始化数据缓存路径
    /// </summary>
    public static void InitDataCache()
    {
        DataCachePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\DataCache\"));
        if (!Directory.Exists(DataCachePath))
        {
            Directory.CreateDirectory(DataCachePath);
        }
        Debugger.Log($"DataCachePath: {DataCachePath}");
    }
    
    /// <summary>
    /// 缓存数据
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    public static void CacheData(string fileName, object data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(DataCachePath + fileName + fileSuffex, json);
    }

    /// <summary>
    ///  获取数据
    /// </summary>
    /// <param name="fileName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetCacheData<T>(string fileName)
    {
        if (!CacheFileExist(fileName))
        {
            return default(T);
        }
        string jsonContent = File.ReadAllText(DataCachePath + fileName + fileSuffex);
        return JsonConvert.DeserializeObject<T>(jsonContent);
    }
    
    /// <summary>
    ///  检查文件是否存在
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool CacheFileExist(string fileName)
    {
        return File.Exists(DataCachePath + fileName + fileSuffex);
    }
}