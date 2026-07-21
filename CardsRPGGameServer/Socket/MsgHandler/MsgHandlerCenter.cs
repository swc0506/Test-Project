using System.Collections.Generic;
using CardsRPGGameServer.Proto;
using CardsRPGGameServer.Socket;

public class MsgHandlerCenter : Singleton<MsgHandlerCenter>
{
    // key: 协议号，value: 处理器
    private Dictionary<Protocal, HandlerBase> mHandlerDict = new Dictionary<Protocal, HandlerBase>();

    public void Init()
    {
        mHandlerDict.Add(Protocal.LoginRequest, new LoginRequestHandler());
        mHandlerDict.Add(Protocal.StartBattleRequest, new StartBattleRequestHandler());
        mHandlerDict.Add(Protocal.BattleResultRequest, new BattleResultRequestHandler());
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
            Debugger.LogError("MsgHandlerCenter HandlerMsg error: protocal not found: " + protocal);
        }
    }
    
    public void Destroy()
    {
        mHandlerDict.Clear();
    }
}