#if WEBSOCKET
using System;
using System.Net.Sockets;
using WebSocketSharp;

namespace Core.Net
{
    public class WebSocketConnection : Connection
    {
        private WebSocket webSocket;

        protected override void StartConnect(string host, int port)
        {
            string url = string.Format("{0}:{1}", host, port);
            StartConnect(url);
        }

        protected override void StartConnect(string url)
        {
            webSocket = new WebSocket(url);
            webSocket.OnOpen += OnWebSocketOpen;
            webSocket.OnError += OnWebSocketError;
            webSocket.OnMessage += OnWebSocketMessage;
            webSocket.OnClose += OnWebSocketClose;
            webSocket.ConnectAsync();
        }

        private void OnWebSocketOpen(object sender, EventArgs e)
        {
            //链接成功开始接收数据
            state = SocketState.Connected;
            OnConnectEvent?.Invoke(0);
        }

        private void OnWebSocketError(object sender, ErrorEventArgs e)
        {
            if (state == SocketState.Connecting)
            {
                OnConnectEvent?.Invoke((int)SocketError.SocketError);
            }
            else if (state == SocketState.Connected)
            {
                OnError(e.Message);
            }

            state = SocketState.None;
        }

        private void OnWebSocketMessage(object sender, MessageEventArgs e)
        {
            if (e.IsPing)
            {
                return;
            }
            else if (e.IsText)
            {
            }
            else if (e.IsBinary)
            {
                OnReceivedEvent?.Invoke(e.RawData, 0, e.RawData.Length);
            }
        }

        private void OnWebSocketClose(object sender, CloseEventArgs e)
        {
            if (state == SocketState.Connecting)
            {
                OnConnectEvent?.Invoke((int)SocketError.ConnectionRefused);
                state = SocketState.None;
            }
            else if (state == SocketState.Connected)
            {
                Disconnect();
                OnDisconnectEvent?.Invoke();
            }
        }

        protected override void OnSend(byte[] buffs, int offset, int length)
        {
            webSocket.SendAsync(buffs, null);
        }

        protected override void OnConnectTimeOut()
        {
            OnConnectEvent?.Invoke((int)SocketError.TimedOut);
        }

        public override void Disconnect()
        {
            if (null != webSocket)
            {
                state = SocketState.Disconnect;
                webSocket.OnOpen -= OnWebSocketOpen;
                webSocket.OnError -= OnWebSocketError;
                webSocket.OnMessage -= OnWebSocketMessage;
                webSocket.OnClose -= OnWebSocketClose;
                webSocket.Close();
                webSocket = null;
            }
        }
    }
}

#endif