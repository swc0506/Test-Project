using CardsRPGGameServer.Proto;
using CardsRPGGameServer.Socket;

public class LoginRequestHandler : HandlerBase
{
    public override void HandlerMsg(ClientUser client, byte[] data)
    {
        LoginRequest request = ProtoBuffSerialize.Deserialize<LoginRequest>(data);
        if (request != null)
        {
            client.DeviceID = request.DeviceID;
            Debugger.Log("LoginRequestHandler HandlerMsg: " + request.DeviceID);
            LoginResponse response = new LoginResponse();
            response.ResultCode = 0;
            response.Id = 1;
            response.Name = "test";
            client.SendPacket(Protocal.LoginResponse, response);
        }
        else
        {
            Debugger.LogError("LoginRequestHandler HandlerMsg error: request is null");
        }
    }
}