using Fleck;

namespace CardsRPGGameServer.Socket;

public class ClientUser : ClientSocket
{ 
    public string DeviceID { get; set; } 
    
    public ClientUser(string url, IWebSocketConnection socket) : base(url, socket)
    {
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}