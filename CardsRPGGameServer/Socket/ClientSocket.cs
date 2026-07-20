using CardsRPGGameServer.Proto;
using Fleck;

namespace CardsRPGGameServer.Socket;

public class ClientSocket
{
    public string ClientURL { get; private set; }
    public IWebSocketConnection Socket { get; private set; }
    
    public ClientSocket(string url, IWebSocketConnection socket)
    {
        this.ClientURL = url;
        this.Socket = socket;
    }
    
    public void SendMessage(string message)
    {
        Socket.Send(message);
    }
    
    public void SendPacket<T>(Protocal protocal, T packet)
    {
        Socket.Send(ProtoBuffSerialize.Serialize<T>(protocal, packet));
    }
    
    public void SendBinaryMessage(byte[] data)
    {
        Socket.Send(data);
    }
    
    public virtual void OnDestroy()
    {
        Socket.Close();
        Socket = null;
    }
}