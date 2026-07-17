using Fleck;

namespace CardsRPGGameServer.Socket;

public class ClientUser : ClientSocket
{
    public ClientUser(string url, IWebSocketConnection socket) : base(url, socket)
    {
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}