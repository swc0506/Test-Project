using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class C2A_GetRealmKeyHandler : AMRpcHandler<C2A_GetRealmKey, A2C_GetRealmKey>
    {
        protected override async ETTask Run(Session session, C2A_GetRealmKey request, A2C_GetRealmKey response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                response.Error = ErrorCode.ERR_RequestSceneTypeError;
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
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.AccountId.GetHashCode()))
                {
                    StartSceneConfig realmStarSceneConfig = RealmGateAddressHelper.GetRealm(request.ServerId);

                    R2A_GetRealmKey r2A_GetRealmKey = (R2A_GetRealmKey)await MessageHelper.CallActor(realmStarSceneConfig.InstanceId, new A2R_GetRealmKey() { AccountId = request.AccountId });

                    if (r2A_GetRealmKey.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = r2A_GetRealmKey.Error;
                        reply();
                        session.Disconnect().Coroutine();
                        return;
                    }

                    response.RealmKey = r2A_GetRealmKey.RealmKey;
                    response.RealmAddress = realmStarSceneConfig.OuterIPPort.ToString();
                    reply();
                    session?.Disconnect().Coroutine();
                }
            }

            await ETTask.CompletedTask;
        }
    }
}
