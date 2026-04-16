using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Event;

#if VIRTUAL

namespace Core.Net
{
    public class EnumServerMessageHandler<M> : IMessageHandler where M : Enum
    {
        private EventDispatcher<M> eventDispatcher;
        private Dictionary<M, Type> typeMap;

        private Func<M, string, string> msgIdToTypeName;
        private Func<M, int> msgTypeToInt;
        private Func<int, M> intToMsgType;

        public Action<M, object> onReceiveMsg;

        protected IServerHandler serverHandler;

        public EnumServerMessageHandler(Func<M, string, string> msgIdToTypeName, Func<M, int> msgTypeToInt,
            Func<int, M> intToMsgType)
        {
            this.msgIdToTypeName = msgIdToTypeName;
            this.msgTypeToInt = msgTypeToInt;
            this.intToMsgType = intToMsgType;
            eventDispatcher = new EventDispatcher<M>(new EnumEqualityComparer<M>(), true);
            typeMap = new Dictionary<M, Type>();
        }

        public EnumServerMessageHandler(Func<M, int> msgTypeToInt, Func<int, M> intToMsgType) : this(null, msgTypeToInt,
            intToMsgType)
        {
        }

        public void SetServerHandler(IServerHandler serverHandler)
        {
            this.serverHandler = serverHandler;
        }

        public void RegisterProtoType(M msgId, Type type)
        {
            if (null != type)
            {
                if (!typeMap.ContainsKey(msgId))
                {
                    typeMap.Add(msgId, type);
                }
            }
        }

        public void RegisterProtoType<T>(M msgId)
        {
            RegisterProtoType(msgId, typeof(T));
        }

        public void RegisterProtoTypes(Dictionary<M, Type> typeMap)
        {
            foreach (var item in typeMap)
            {
                RegisterProtoType(item.Key, item.Value);
            }
        }

        public void RegisterProtoTypes()
        {
            Type mType = typeof(M);
            if (!mType.IsEnum)
            {
                return;
            }

            string[] names = Enum.GetNames(mType);
            if (null == names || names.Length == 0)
            {
                return;
            }

            Assembly assembly = mType.Assembly;
            string ns = mType.Namespace;
            foreach (var item in names)
            {
                M m = (M)Enum.Parse(mType, item);
                string msgTypeName = null;
                if (null != msgIdToTypeName)
                {
                    msgTypeName = msgIdToTypeName.Invoke(m, ns);
                }
                else
                {
                    msgTypeName = string.IsNullOrEmpty(ns) ? item : string.Format("{0}.{1}", ns, item);
                }

                Type type = assembly.GetType(msgTypeName, false, true);
                RegisterProtoType(m, type);
            }
        }

        public void AddListener(M msgId, Action callback)
        {
            eventDispatcher.AddListener(msgId, callback);
        }

        public void AddListener<T>(M msgId, Action<T> callback)
        {
            eventDispatcher.AddListener(msgId, callback);
        }

        public void RemoveListener<T>(M msgId, Action<T> callback)
        {
            eventDispatcher.RemoveListener(msgId, callback);
        }

        public void RemoveListener(M msgId, Action callback)
        {
            eventDispatcher.RemoveListener(msgId, callback);
        }

        public void ClearListener(M msgId)
        {
            eventDispatcher.ClearListener(msgId);
        }

        public void ClearListenerByCaller(object caller)
        {
            eventDispatcher.ClearListenerByCaller(caller);
        }

        public void SendMessage(M msgId, object msg)
        {
            if (null != msg)
            {
                int opcode = msgTypeToInt.Invoke(msgId);
                serverHandler.Server.SendMessage(serverHandler.Connection, opcode, msg);
            }
        }

        public void OnReceive(MessagePacket packet)
        {
            object data = null;
            M msgId = intToMsgType.Invoke(packet.Opcode);
            if (typeMap.TryGetValue(msgId, out var type))
            {
                data = packet.Parse(type);
                eventDispatcher.DispatchListener(msgId, data);
            }
            else
            {
                eventDispatcher.DispatchListener(msgId);
            }

            onReceiveMsg?.Invoke(msgId, data);
        }

        public void OnReceiveHeartbeat(long timestamp)
        {
            var heartProtocol = serverHandler.ProtocolCoder.CreateProtocol(VirtualServerManager.Instance.Timestamp);
            serverHandler.Server.SendProtocol(serverHandler.Connection,heartProtocol);
        }
    }
}
#endif