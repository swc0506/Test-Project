using System;
using System.Collections.Generic;
using Core.Event;

#if VIRTUAL

namespace Core.Net
{
    public class IntServerMessageHandler : IMessageHandler
    {
        private EventDispatcher<int> eventDispatcher;
        private Dictionary<int, Type> typeMap;

        protected IServerHandler serverHandler;

        public IntServerMessageHandler()
        {
            eventDispatcher = new EventDispatcher<int>(true);
            typeMap = new Dictionary<int, Type>();
        }

        public void SetServerHandler(IServerHandler serverHandler)
        {
            this.serverHandler = serverHandler;
        }

        public void RegisterProtoType(int msgId, Type type)
        {
            if (null != type)
            {
                if (!typeMap.ContainsKey(msgId))
                {
                    typeMap.Add(msgId, type);
                }
            }
        }

        public void RegisterProtoType<T>(int msgId)
        {
            RegisterProtoType(msgId, typeof(T));
        }

        public void RegisterProtoTypes(Dictionary<int, Type> typeMap)
        {
            foreach (var item in typeMap)
            {
                RegisterProtoType(item.Key, item.Value);
            }
        }

        public void AddListener<T>(int msgId, Action<T> callback)
        {
            eventDispatcher.AddListener(msgId, callback);
        }

        public void RemoveListener<T>(int msgId, Action<T> callback)
        {
            eventDispatcher.RemoveListener(msgId, callback);
        }

        public void ClearListener(int msgId)
        {
            eventDispatcher.ClearListener(msgId);
        }

        public void ClearListenerByCaller(object caller)
        {
            eventDispatcher.ClearListenerByCaller(caller);
        }

        public void SendMessage(int msgId, object msg)
        {
            serverHandler.Server.SendMessage(serverHandler.Connection, msgId, msg);
        }

        public void OnReceive(MessagePacket packet)
        {
            int id = packet.Opcode;
            object data = null;
            if (typeMap.TryGetValue(id, out var type))
            {
                data = packet.Parse(type);
            }

            eventDispatcher.DispatchListener(id, data);
        }

        public void OnReceiveHeartbeat(long timestamp)
        {
            var heartProtocol = serverHandler.ProtocolCoder.CreateProtocol(VirtualServerManager.Instance.Timestamp);
            serverHandler.Server.SendProtocol(serverHandler.Connection,heartProtocol);
        }
    }
}

#endif