using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ExcelDataBinary 
{
    /// <summary>
    /// 反序列化Excel配置数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytesPath"></param>
    /// <returns></returns>
    public static List<T> DeSerilizeBinaryData<T>(string bytesPath) where T : class
    {
        List<object> heroDatas = BinaryDeSerilize<List<object>>(bytesPath);
        List<T> herolist = new List<T>();
        foreach (var herodata in heroDatas)
        {
            herolist.Add(herodata as T);
        }
        return herolist;
    }
    /// <summary>
    /// 反序列化二进制
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T BinaryDeSerilize<T>(string path)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            object obj = null;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter binary = new BinaryFormatter();
                obj = binary.Deserialize(ms);
            }
            return (T)obj;
        }
        catch (Exception e)
        {
            Debug.LogError("BinaryDeserilize Fiale! Error:" + e.ToString());
            return default(T);
        }

    }
}
