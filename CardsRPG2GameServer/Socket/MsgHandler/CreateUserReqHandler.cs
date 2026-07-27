using System;
using CardsRPGGameServer.Proto;
using CardsRPGGameServer.Socket;

public class CreateUserReqHandler : HandlerBase
{
    public override void HandlerMsg(ClientUser client, byte[] data)
    {
        CreateUserRequest request = ProtoBuffSerialize.Deserialize<CreateUserRequest>(data);
        if (request != null)
        {
            client.DeviceID = request.deviceId;
            CreateUserResponse response = new CreateUserResponse();
            if (DataCacheSystem.CacheFileExist(client.DeviceID))
            {
                Debugger.Log("该账户已存在...");
                var user = DataCacheSystem.GetCacheData<UserData>(request.deviceId);
                response.resultCode = ResultCode.AccountExist;
                response.userData = user;
                client.SendPacket(Protocal.CreateUserResponse, response);
                return;
            }

            UserData userData = new UserData();
            userData.Id = Math.Abs(request.deviceId.GetHashCode());
            userData.UserName = request.userName;
            userData.Gender = request.gender;
            DataCacheSystem.CacheData(client.DeviceID, userData);
            response.resultCode = ResultCode.Success;
            response.userData = userData;
            client.SendPacket(Protocal.CreateUserResponse, response);
        }
    }
}