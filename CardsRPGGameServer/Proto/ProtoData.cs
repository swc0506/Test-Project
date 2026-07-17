using ProtoBuf;

namespace CardsRPGGameServer.Proto;

public class ProtoData
{
    public class Packet
    {
        [ProtoMember(1)] public int ResultCode;
    }
    
    [ProtoContract]
    public class LoginRequest : Packet
    {
        [ProtoMember(1)] public string DeviceID;
    }
    
    [ProtoContract]
    public class LoginResponse : Packet
    {
        [ProtoMember(1)] public int Id;
        [ProtoMember(2)] public string Name;
    }
}