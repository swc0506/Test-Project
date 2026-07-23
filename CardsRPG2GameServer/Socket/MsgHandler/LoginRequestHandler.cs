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

            if (!DataCacheSystem.CacheFileExist(client.DeviceID))
            {
                Debugger.Log("该账户不存在...");
                response.ResultCode = ResultCode.AccountNotFind;
                client.SendPacket(Protocal.LoginResponse, response);
                return;
            }

            // 获取用户数据
            response.UserData = DataCacheSystem.GetCacheData<UserData>(client.DeviceID);
            // 缓存用户数据
            client.CacheUserData(response.UserData);
            // 发送响应
            client.SendPacket(Protocal.LoginResponse, response);
        }
        else
        {
            Debugger.LogError("LoginRequestHandler HandlerMsg error: request is null");
        }
    }
}