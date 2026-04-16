#if PROTOBUF
using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace Core.Net
{
    public class ProtobufMessagePacker : IMessagePacker
    {
        private Func<Type, byte[], object> parseFunc;

        public void RegisterParseFunc(Func<Type, byte[], object> parseFunc)
        {
            this.parseFunc = parseFunc;
        }

        public byte[] Serialize(object data)
        {
            try
            {
                if (data is IMessage)
                {
                    return ((IMessage)data).ToByteArray();
                }
                else if (data is byte[])
                {
                    return (byte[])data;
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("ProtobufMessagePacker Serialize Exception:{0}", e.Message);
            }

            return null;
        }

        public object Deserialize(byte[] buffs, Type type)
        {
            object res = null;
            try
            {
                if (null != parseFunc)
                {
                    res = parseFunc.Invoke(type, buffs);
                }
                else
                {
                    IMessage message = (IMessage)Activator.CreateInstance(type);
                    message.MergeFrom(buffs);
                    res = message;
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("ProtobufMessagePacker Serialize Exception:{0}", e.Message);
            }

            return res;
        }

        public T Deserialize<T>(byte[] buffs)
        {
            return (T)Deserialize(buffs, typeof(T));
        }

        public string SerializeToString(object data)
        {
            if (data is IMessage)
            {
                return JsonFormatter.ToDiagnosticString((IMessage)data);
            }

            return string.Empty;
        }

        public T DeserializeFromString<T>(string text)
        {
            return default(T);
        }
    }
}

#endif