using System;
using System.Text;

namespace Core.Net
{
    public class JsonMessagePacker : IMessagePacker
    {
        public byte[] Serialize(object obj)
        {
            byte[] buffs = null;
            try
            {
                if (obj is string)
                {
                    buffs = Encoding.UTF8.GetBytes(obj.ToString());
                }
                else
                {
                    string text = JsonUtils.ToJson(obj);
                    buffs = Encoding.UTF8.GetBytes(text);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("JsonMessagePacker Serialize Exception:{0}", e.Message);
            }

            return buffs;
        }

        public object Deserialize(byte[] buffs, Type type)
        {
            try
            {
                string text = Encoding.UTF8.GetString(buffs);
                if (type == typeof(string))
                {
                    return (text);
                }
                else
                {
                    return JsonUtils.ToObject(text, type);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("JsonMessagePacker Deserialize Exception:{0}", e.Message);
            }

            return null;
        }

        public T Deserialize<T>(byte[] buffs)
        {
            return (T)Deserialize(buffs, typeof(T));
        }

        public string SerializeToString(object data)
        {
            if (null != data)
            {
                return JsonUtils.ToJson(data);
            }

            return string.Empty;
        }

        public T DeserializeFromString<T>(string text)
        {
            return JsonUtils.ToObject<T>(text);
        }
    }
}