using System;
using System.IO;

namespace CardsRPGGameServer.Proto;

public class ProtoBuffSerialize
{
    /// <summary>
    ///  序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte[] Serialize<T>(T obj)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                byte[] result = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(result, 0, result.Length);
                return result;
            }
        }
        catch (Exception e)
        {
            Debugger.LogError("ProtoBuffSerialize Serialize error: " + e);
            throw;
        }
    }

    /// <summary>
    ///  反序列化
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Deserialize<T>(byte[] data)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                ms.Write(data, 0, data.Length);
                ms.Position = 0;
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }
        catch (Exception e)
        {
            Debugger.LogError("ProtoBuffSerialize Deserialize error: " + e);
            throw;
        }
    }

    public static byte[] Serialize<T>(Protocal protocal, T data)
    {
        try
        {
            // 包头 2字节 
            byte[] packetHead = BitConverter.GetBytes((short)protocal);
            // 包体
            byte[] packetData = Serialize(data);
            // 总包长度
            byte[] packet = new byte[packetHead.Length + packetData.Length];
            Array.Copy(packetHead, 0, packet, 0, packetHead.Length);
            Array.Copy(packetData, 0, packet, packetHead.Length, packetData.Length);
            return packet;
        }
        catch (Exception e)
        {
            Debugger.LogError("ProtoBuffSerialize Serialize error: " + e);
            throw;
        }
    }

    /// <summary>
    ///  反序列化包头
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Protocal DeSerializeProtocal(byte[] data)
    {
        short protocal = BitConverter.ToInt16(data, 0);
        return (Protocal)protocal;
    }

    /// <summary>
    ///  获取包体
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] DeSerializeData(byte[] data)
    {
        try
        {
            byte[] packetData = new byte[data.Length - 2];
            Array.Copy(data, 2, packetData, 0, packetData.Length);
            return packetData;
        }
        catch (Exception e)
        {
            Debugger.LogError("ProtoBuffSerialize DeSerializeData error: " + e);
            throw;
        }
    }
}