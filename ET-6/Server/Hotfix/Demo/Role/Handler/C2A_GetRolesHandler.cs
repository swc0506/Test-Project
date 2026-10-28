using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [FriendClassAttribute(typeof(ET.RoleInfo))]
    public class C2A_GetRolesHandler : AMRpcHandler<C2A_GetRoles, A2C_GetRoles>
    {
        protected override async ETTask Run(Session session, C2A_GetRoles request, A2C_GetRoles response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                response.Error = ErrorCode.ERR_RequestSceneTypeError;
                reply();
                session.Dispose();
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);

            if (token != request.Token || token == null)
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole, request.AccountId.GetHashCode()))
                {
                    var roleInfos = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<RoleInfo>(
                        d => d.AccountId == request.AccountId && d.ServerId == request.ServerId && d.State == (int)RoleInfoState.Normal);

                    if (roleInfos == null || roleInfos.Count == 0)
                    {
                        reply();
                        return;
                    }

                    foreach (var roleInfo in roleInfos)
                    {
                        response.RoleInfoList.Add(roleInfo.ToMessage());
                        roleInfo?.Dispose();
                    }
                    roleInfos.Clear();

                    reply();
                }
            }
        }
    }
}
