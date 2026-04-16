using System;

namespace Core.Net
{
    public interface IMessagePacker
    {
        byte[] Serialize(object data);

        object Deserialize(byte[] buffs,Type type);
        
        T Deserialize<T>(byte[] buffs);

        
        string SerializeToString(object data);

        T DeserializeFromString<T>(string text);

    }
}