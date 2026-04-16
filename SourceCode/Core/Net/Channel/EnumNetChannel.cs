using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Event;

namespace Core.Net
{
    public class EnumNetChannel<M> : NetChannel where M : Enum
    {
        private EventDispatcher<M> eventDispatcher;
        private Dictionary<M, Type> typeMap;

        private Func<M, string, string> msgIdToTypeName;
        private Func<M, int> msgIdToInt;
        private Func<int, M> intToMsgId;

        public Action<M, object> onReceiveMsg;

        public EnumNetChannel(Remote remote, Func<M, string, string> msgIdToTypeName, Func<M, int> msgIdToInt,
            Func<int, M> intToMsgId) : base(remote)
        {
            this.msgIdToTypeName = msgIdToTypeName;
            this.msgIdToInt = msgIdToInt;
            this.intToMsgId = intToMsgId;
            eventDispatcher = new EventDispatcher<M>(new EnumEqualityComparer<M>(), true);
            typeMap = new Dictionary<M, Type>();
        }

        public EnumNetChannel(Remote remote, Func<M, int> msgIdToInt, Func<int, M> intToMsgId) : this(remote, null,
            msgIdToInt, intToMsgId)
        {
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

        private string MsgIdToTypeName(M m, string ns)
        {
            return string.IsNullOrEmpty(ns) ? m.ToString() : string.Format("{0}.{1}", ns, m);
        }

        public void AddListener<T>(M msgId, Action<T> callback)
        {
            eventDispatcher.AddListener(msgId, callback);
        }

        public void RemoveListener<T>(M msgId, Action<T> callback)
        {
            eventDispatcher.RemoveListener(msgId, callback);
        }

        public void DispatchListener(M msgId, object data)
        {
            eventDispatcher.DispatchListener(msgId, data);
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
            int opcode = msgIdToInt.Invoke(msgId);
            PrintMsgLog("Send", msgId, opcode, msg);
            remote.SendMessage(opcode, msg);
        }

        public override void OnReceiveMessage(MessagePacket packet)
        {
            object data = null;
            M msgId = intToMsgId.Invoke(packet.Opcode);
            if (typeMap.TryGetValue(msgId, out var type))
            {
                data = packet.Parse(type);
                PrintMsgLog("Receive", msgId, packet.Opcode, data);
                if (null != data)
                {
                    onReceiveMsg?.Invoke(msgId, data);
                    DispatchListener(msgId, data);
                }
            }
            else
            {
                PrintMsgLog("Receive", msgId, packet.Opcode, data);
            }
        }

        private void PrintMsgLog(string tag, M msgId, int opcode, object msg)
        {
            if (Console.IsEnable)
            {
                Logger.DebugFormat("[{0}] {1} MsgId:{2}={3},Data:{4}", remote.Name, tag, msgId, opcode,
                    remote.DataToString(msg));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            msgIdToInt = null;
            intToMsgId = null;
            eventDispatcher = null;
            typeMap = null;
            onReceiveMsg = null;
        }
    }
}