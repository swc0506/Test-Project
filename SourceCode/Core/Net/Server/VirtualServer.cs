using System;
using System.Collections.Generic;
using UnityEngine;

#if VIRTUAL

namespace Core.Net
{
    public class VirtualServer
    {
        public string Url { get; private set; }

        private Type serverHandlerType;

        private Dictionary<Connection, IServerHandler> handlerMap =
            new Dictionary<Connection, IServerHandler>();


        public VirtualServer(string url)
        {
            Url = url;
            MonoEventProxy.Instance.UpdateEvent += Update;
        }

        public void SetHandlerType(Type serverHandlerType)
        {
            this.serverHandlerType = serverHandlerType;
        }

        public void OnChannelActive(Connection connection)
        {
            var handler = (IServerHandler)Activator.CreateInstance(serverHandlerType);
            handler.Initial(this);
            handler.OnChannelActive(connection);
            handlerMap.Add(connection, handler);
        }

        public void OnChannelInactive(Connection connection)
        {
            if (handlerMap.TryGetValue(connection, out var handler))
            {
                handler.OnChannelInactive(connection);
                handlerMap.Remove(connection);
            }
        }

        public void OnReceive(Connection connection, byte[] buffs, int offset, int length)
        {
            if (handlerMap.TryGetValue(connection, out var handler))
            {
                handler.ProtocolCoder.Decode(buffs, offset, length);
            }
        }

        public void SendBuffs(Connection connection, byte[] buffs)
        {
            if (connection is VirtualConnection virtualConnection)
            {
                virtualConnection.OnReceiveCompleted(buffs, 0, buffs.Length);
            }
        }

        public void SendProtocol(Connection connection, IProtocol protocol)
        {
            if (handlerMap.TryGetValue(connection, out var handler))
            {
                byte[] buffs = handler.ProtocolCoder.Encode(protocol);
                SendBuffs(connection, buffs);
            }
        }

        public void SendMessageBuffs(Connection connection, int opcode, byte[] dataBuffs)
        {
            if (handlerMap.TryGetValue(connection, out var handler))
            {
                IProtocol protocol = handler.ProtocolCoder.CreateProtocol(opcode, dataBuffs);
                byte[] buffs = handler.ProtocolCoder.Encode(protocol);
                SendBuffs(connection, buffs);
            }
        }

        public void SendMessage(Connection connection, int opcode, object data)
        {
            if (handlerMap.TryGetValue(connection, out var handler))
            {
                byte[] dataBuffs = handler.MessagePacker.Serialize(data);
                IProtocol protocol = handler.ProtocolCoder.CreateProtocol(opcode, dataBuffs);
                byte[] buffs = handler.ProtocolCoder.Encode(protocol);
                SendBuffs(connection, buffs);
            }
        }

        public void Update(float delta)
        {
            foreach (var item in handlerMap)
            {
                item.Value.Update(delta);
            }
        }

        public void Dispose()
        {
            MonoEventProxy.Instance.UpdateEvent -= Update;
            handlerMap = null;
        }
    }
}
#endif