using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkManager : SocketBase
{
    private static NetWorkManager _instance;
    public static NetWorkManager Instance
    {
        get
        {
            _instance ??= new NetWorkManager();
            return _instance;
        }
    }

    public override void OnReceivePacket(Protocal protocal, byte[] data)
    {
        base.OnReceivePacket(protocal, data);
        Debugger.Log("OnReceivePacket: " + protocal);
        NetEventControl.DispatchEvent(protocal, data);
    }
}
