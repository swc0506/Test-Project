using System;
using System.Threading.Tasks;


namespace ET
{
    [FriendClass(typeof(AccountInfoComponent))]
    [FriendClass(typeof(ServerInfosComponent))]
    [FriendClass(typeof(RoleInfoComponent))]
    public static class LoginHelper
    {
        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password)
        {
            A2C_LoginAccount a2C_LoginAccount = null;
            Session accountSession = null;

            try
            {
                accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                password = MD5Helper.StringMD5(password);
                a2C_LoginAccount = (A2C_LoginAccount)await accountSession.Call(new C2A_LoginAccount()
                {
                    AccountName = account,
                    Password = password
                });

            }
            catch (Exception e)
            {
                accountSession?.Dispose();
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2C_LoginAccount.Error != ErrorCode.ERR_Success)
            {
                return a2C_LoginAccount.Error;
            }

            zoneScene.AddComponent<SessionComponent>().Session = accountSession;
            zoneScene.GetComponent<SessionComponent>().Session.AddComponent<PingComponent>();

            zoneScene.GetComponent<AccountInfoComponent>().Token = a2C_LoginAccount.Token;
            zoneScene.GetComponent<AccountInfoComponent>().AccountId = a2C_LoginAccount.AccountId;

            return ErrorCode.ERR_Success;
        }


        public static async ETTask<int> GetServerInfos(Scene zoneScene)
        {
            A2C_GetServerIfos a2C_GetServerIfos = null;

            try
            {
                a2C_GetServerIfos = (A2C_GetServerIfos)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetServerIfos()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token
                });

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2C_GetServerIfos.Error != ErrorCode.ERR_Success)
            {
                return a2C_GetServerIfos.Error;
            }

            foreach (var serverInfoProto in a2C_GetServerIfos.ServerIfosList)
            {
                ServerInfo serverInfo = zoneScene.GetComponent<ServerInfosComponent>().AddChild<ServerInfo>();
                serverInfo.FroMessage(serverInfoProto);

                zoneScene.GetComponent<ServerInfosComponent>().Add(serverInfo);
            }


            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> CreateRole(Scene zoneScene, String name)
        {
            A2C_CreateRole a2C_CreateRole = null;

            try
            {
                a2C_CreateRole = (A2C_CreateRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_CreaetRole()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    Name = name,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2C_CreateRole.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2C_CreateRole.Error.ToString());
                return a2C_CreateRole.Error;
            }

            RoleInfo newRoleInfo = zoneScene.GetComponent<RoleInfoComponent>().AddChild<RoleInfo>();
            newRoleInfo.FromMessage(a2C_CreateRole.RoleInfo);
            zoneScene.GetComponent<RoleInfoComponent>().RoleInfos.Add(newRoleInfo);

            return ErrorCode.ERR_Success;
        }


        public static async ETTask<int> GetRoles(Scene zoneScene)
        {
            A2C_GetRoles a2C_GetRoles = null;

            try
            {
                a2C_GetRoles = (A2C_GetRoles)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRoles()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId
                });

            }catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2C_GetRoles.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2C_GetRoles.Error.ToString());
                return a2C_GetRoles.Error;
            }

            zoneScene.GetComponent<RoleInfoComponent>().RoleInfos.Clear();
            foreach (var roleInfoProto in a2C_GetRoles.RoleInfoList)
            {
                RoleInfo roleInfo = zoneScene.GetComponent<RoleInfoComponent>().AddChild<RoleInfo>();
                roleInfo.FromMessage(roleInfoProto);
                zoneScene.GetComponent<RoleInfoComponent>().RoleInfos.Add(roleInfo);
            }

            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> DeleteRole(Scene zoneScene)
        {
            A2C_DeleteRole a2C_DeleteRole = null;

            try
            {
                a2C_DeleteRole = (A2C_DeleteRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_DeleteRole()
                {
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    DeleteRoleInfoId = zoneScene.GetComponent<RoleInfoComponent>().CurrentRoleId,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId
                });

            }catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (a2C_DeleteRole.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2C_DeleteRole.Error.ToString());
                return a2C_DeleteRole.Error;
            }

            int index = zoneScene.GetComponent<RoleInfoComponent>().RoleInfos.FindIndex((info) => { return info.Id == a2C_DeleteRole.DeleteRoleInfoId; });

            zoneScene.GetComponent<RoleInfoComponent>().RoleInfos.RemoveAt(index);

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> GetRealmKey(Scene zoneScene)
        {
            A2C_GetRealmKey a2C_GetRealmKey = null;

            try
            {
                a2C_GetRealmKey = (A2C_GetRealmKey)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRealmKey()
                {
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurrentServerId
                });

            }catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if ( a2C_GetRealmKey.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2C_GetRealmKey.Error.ToString());
                return a2C_GetRealmKey.Error;
            }

            zoneScene.GetComponent<AccountInfoComponent>().RealmKey = a2C_GetRealmKey.RealmKey;
            zoneScene.GetComponent<AccountInfoComponent>().RealmAddress = a2C_GetRealmKey.RealmAddress;
            zoneScene.GetComponent<SessionComponent>().Session.Dispose();

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> EnterGame(Scene zoneScene)
        {
            string realmAddress = zoneScene.GetComponent<AccountInfoComponent>().RealmAddress;

            //连接Realm 获取Gate网关
            R2C_LoginRealm r2C_LoginRealm = null;

            Session session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(realmAddress));
            try
            {
                r2C_LoginRealm = (R2C_LoginRealm)await session.Call(new C2R_LoginRealm()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    RealmTokenKey = zoneScene.GetComponent<AccountInfoComponent>().RealmKey
                });

            }catch (Exception e)
            {
                Log.Error(e);
                session?.Dispose();
                return ErrorCode.ERR_NetworkError;
            }
            session?.Dispose();
            
            if (r2C_LoginRealm.Error != ErrorCode.ERR_Success)
            {
                return r2C_LoginRealm.Error;
            }

            Log.Warning($"GateAddress : {r2C_LoginRealm.GateAddress}");
            Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2C_LoginRealm.GateAddress));
            gateSession.AddComponent<PingComponent>();
            zoneScene.GetComponent<SessionComponent>().Session = gateSession;

            // 开始连接Gate
            long currentRoleId = zoneScene.GetComponent<RoleInfoComponent>().CurrentRoleId;
            G2C_LoginGameGate g2C_LoginGameGate = null;
            try
            {
                long accountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId;
                g2C_LoginGameGate = (G2C_LoginGameGate)await gateSession.Call(new C2G_LoginGameGate() {
                    Key = r2C_LoginRealm.GateSessionKey,
                    AccountId = accountId,
                    RoleId = currentRoleId
                });

            }catch (Exception e)
            {
                Log.Error(e);
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return ErrorCode.ERR_NetworkError;
            }

            if (g2C_LoginGameGate.Error != ErrorCode.ERR_Success)
            {
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return g2C_LoginGameGate.Error;
            }

            Log.Debug("登录gate成功");

            // 3.角色正式请求进入游戏逻辑服
            G2C_EnterGame g2C_EnterGame = null;
            try
            {
                g2C_EnterGame = (G2C_EnterGame)await gateSession.Call(new C2G_EnterGame() { });

            }catch(Exception e)
            {
                Log.Error(e);
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return ErrorCode.ERR_NetworkError;
            }

            if (g2C_EnterGame.Error != ErrorCode.ERR_Success)
            {
                Log.Error(g2C_EnterGame.Error.ToString());
                return g2C_EnterGame.Error;
            }

            Log.Debug("角色进入游戏成功");
            zoneScene.GetComponent<PlayerComponent>().MyId = g2C_EnterGame.MyId;

            await zoneScene.GetComponent<ObjectWait>().Wait<WaitType.Wait_SceneChangeFinish>();

            return ErrorCode.ERR_Success;
        }
    }

}