using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.WebSocket;
using UnityEngine;

public class SocketBase
{
    protected WebSocket socket;
    public Uri webSocketUrl = new Uri("ws://127.0.0.1:7777");

    public virtual void ConnectSocket()
    {
        socket = new WebSocket(webSocketUrl);
        socket.OnOpen += OnOpen;
        socket.OnClosed += OnClose;
        socket.OnError += OnError;
        socket.OnMessage += OnMessage;
        socket.OnBinary += OnBinary;
        
        socket.Open();
    }

    public virtual void OnOpen(WebSocket ws)
    {
        Debugger.Log("Socket Open");
        socket.Send("客户端连接成功");
    }
    
    public virtual void OnClose(WebSocket ws, UInt16 code, string message)
    {
        Debugger.Log("Socket Close");
    }
    
    public virtual void OnError(WebSocket ws, string message)
    {
        Debugger.LogError("Socket Error: " + message);
    }
    
    public virtual void OnMessage(WebSocket ws, string message)
    {
        Debugger.Log("Socket Message: " + message);
    }
    
    public virtual void OnBinary(WebSocket ws, byte[] data)
    {
        Debugger.Log("Socket Binary: " + data.Length);
        //去除协议号
        Protocal protocal = ProtoBuffSerialize.DeSerializeProtocal(data);
        byte[] msgBytes = ProtoBuffSerialize.DeSerializeData(data);
        OnReceivePacket(protocal, msgBytes);
    }
    
    public virtual void OnReceivePacket(Protocal protocal, byte[] data)
    {
        
    }
    
    public virtual void SendPacket<T>(Protocal protocal, T data)
    {
        socket.Send(ProtoBuffSerialize.Serialize(protocal, data));
    }
}
