using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [FriendClassAttribute(typeof(ET.RoleInfo))]
    public class C2A_CreateRoleHandler : AMRpcHandler<C2A_CreaetRole, A2C_CreateRole>
    {
        protected override async ETTask Run(Session session, C2A_CreaetRole request, A2C_CreateRole response, Action reply)
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

            if (string.IsNullOrEmpty(request.Name))
            {
                response.Error = ErrorCode.ERR_RoleNameError;
                reply();
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole, request.AccountId.GetHashCode()))
                {
                    var roleInfos = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<RoleInfo>(
                        d => d.Name == request.Name && d.ServerId == request.ServerId);

                    if (roleInfos != null && roleInfos.Count > 0)
                    {
                        response.Error = ErrorCode.ERR_LoginInfoError;
                        reply();
                        return;
                    }

                    RoleInfo newRoleInfo = session.AddChildWithId<RoleInfo>(IdGenerater.Instance.GenerateUnitId(request.ServerId));
                    newRoleInfo.Name = request.Name;
                    newRoleInfo.State = (int)RoleInfoState.Normal;
                    newRoleInfo.ServerId = request.ServerId;
                    newRoleInfo.AccountId = request.AccountId;
                    newRoleInfo.CreatTime = TimeHelper.ServerNow();
                    newRoleInfo.LastLoginTine = 0;

                    await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save<RoleInfo>(newRoleInfo);

                    response.RoleInfo = newRoleInfo.ToMessage();

                    reply();

                    newRoleInfo?.Dispose();
                }

            }

            await ETTask.CompletedTask;
        }
    }
}
