using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using CardsRPGGameServer.Proto;
using Fleck;

namespace CardsRPGGameServer.Socket;

public class SocketServer
{
    private Dictionary<string, ClientUser> mClientUserDict = new Dictionary<string, ClientUser>();
    
    public void Init()
    {
        FleckLog.Level = LogLevel.Debug;
        WebSocketServer server = new WebSocketServer("ws://127.0.0.1:7777");
        server.RestartAfterListenError = true;
        server.Start(OnClientConnected);
    }
    
    public void OnClientConnected(IWebSocketConnection socket)
    {
        // 客户端连接成功后，注册事件监听
        socket.OnOpen = () =>
        {
            string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
            ClientUser clientSocket = new ClientUser(clientUrl, socket);
            mClientUserDict.Add(clientUrl, clientSocket);
            Console.WriteLine("Client connected " + clientUrl + ": " + DateTime.Now);
        };
        // 客户端断开连接时，取消事件监听
        socket.OnClose = () =>
        {
            string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
            if (mClientUserDict.ContainsKey(clientUrl))
            {
                mClientUserDict[clientUrl].OnDestroy();
                mClientUserDict.Remove(clientUrl);
            }
            Console.WriteLine("Client disconnected " + clientUrl + ": " + DateTime.Now);
        };
        // 客户端发生错误时，取消事件监听
        socket.OnError = (ex) =>
        {
            string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
            Console.WriteLine("Client error: " +clientUrl + ": " + ex.Message);
        };
        // 客户端发送消息时，处理消息
        socket.OnMessage = (message) =>
        {
            string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
            Console.WriteLine("Client message: "+clientUrl+ ":" + message + ": " + DateTime.Now);
        };
        // 客户端发送二进制消息时，处理消息
        socket.OnBinary = (data) =>
        {
            Console.WriteLine("Client binary message: " + data.Length + " bytes");
            OnRevClientPacket(data, socket);
        };
    }

    /// <summary>
    /// 接收客户端二进制数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="socket"></param>
    public void OnRevClientPacket(byte[] data, IWebSocketConnection socket)
    {
        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
        //通过Protobuf反序列化数据
        Protocal protocal = ProtoBuffSerialize.DeSerializeProtocal(data);
        byte[] packetData = ProtoBuffSerialize.DeSerializeData(data);
        
        //把消息体派发到对应功能的Hander里
        MsgHandlerConter.Instance.HandlerMsg(mClientUserDict[clientUrl], protocal, packetData);
    }
}