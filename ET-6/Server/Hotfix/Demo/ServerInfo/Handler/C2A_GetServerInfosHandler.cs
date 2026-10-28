using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET.Demo.ServerInfo.Handler
{
    [FriendClassAttribute(typeof(ET.ServerInfoManagerComponent))]
    public class C2A_GetServerInfosHandler : AMRpcHandler<C2A_GetServerIfos, A2C_GetServerIfos>
    {
        protected override async ETTask Run(Session session, C2A_GetServerIfos request, A2C_GetServerIfos response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                response.Error = ErrorCode.ERR_RequestSceneTypeError;
                reply();
                session.Dispose();
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

            foreach (var serverInfo in session.DomainScene().GetComponent<ServerInfoManagerComponent>().ServerInfos)
            {
                response.ServerIfosList.Add(serverInfo.ToMessage());
            }

            reply();

            await ETTask.CompletedTask;
        }
    }
}
