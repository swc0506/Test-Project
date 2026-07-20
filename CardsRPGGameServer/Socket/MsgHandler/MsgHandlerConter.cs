using System.Collections.Generic;
using CardsRPGGameServer.Proto;
using CardsRPGGameServer.Socket;

public class MsgHandlerConter : Singleton<MsgHandlerConter>
{
    // key: 协议号，value: 处理器
    private Dictionary<Protocal, HandlerBase> mHandlerDict = new Dictionary<Protocal, HandlerBase>();

    public void Init()
    {
        mHandlerDict.Add(Protocal.LoginRequest, new LoginRequestHandler());
    }

    public void HandlerMsg(ClientUser client, Protocal protocal, byte[] data)
    {
        HandlerBase handler = null;
        mHandlerDict.TryGetValue(protocal, out handler);
        if (handler != null)
        {
            handler.HandlerMsg(client, data);
        }
        else
        {
            Debugger.LogError("MsgHandlerConter HandlerMsg error: protocal not found: " + protocal);
        }
    }
    
    public void Destroy()
    {
        mHandlerDict.Clear();
    }
}