using System;
using System.Collections.Generic;
using Core.Event;

namespace Core.Net
{
    public class IntNetChannel : NetChannel
    {
        private EventDispatcher<int> eventDispatcher;
        private Dictionary<int, Type> typeMap;

        public IntNetChannel(Remote remote) : base(remote)
        {
            eventDispatcher = new EventDispatcher<int>(true);
            typeMap = new Dictionary<int, Type>();
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

        public void DispatchListener(int msgId, object data)
        {
            eventDispatcher.DispatchListener(msgId, data);
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
            if (null != msg)
            {
                PrintMsgLog("Send", msgId, msg);
                remote.SendMessage(msgId, msg);
            }
        }

        public override void OnReceiveMessage(MessagePacket packet)
        {
            int id = packet.Opcode;
            object data = null;
            if (typeMap.TryGetValue(id, out var type))
            {
                data = packet.Parse(type);
                PrintMsgLog("Receive", id, data);
                DispatchListener(id, data);
            }
            else
            {
                PrintMsgLog("Receive", id, data);
            }
        }

        private void PrintMsgLog(string tag, int msgId, object msg)
        {
            if (Console.IsEnable)
            {
                Logger.DebugFormat("[{0}] {1} MsgId:{2},Data:{3}", remote.Name, tag, msgId, remote.DataToString(msg));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            eventDispatcher = null;
            typeMap = null;
        }
    }
}