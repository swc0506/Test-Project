using Fantasy;
using Hotfix.Share;
using Model.Authentication;
using Model.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Authentication
{
    public class GetTokenReqeuestHandler : MessageRPC<GetTokenReqest, GetTokenResponse>
    {
        protected override async FTask Run(Session session, GetTokenReqest request, GetTokenResponse response, Action reply)
        {
            AuthenticationAccountComponent accountComponent= session.Scene.GetComponent<AuthenticationAccountComponent>();
            var result= await accountComponent.LoginAccount(request.account,request.passWorld);
            response.ErrorCode = result.erorCode;
            //登录失败
            if (result.erorCode!=0)
            {
                return;
            }


            //分配Gate服务
            var gateResult= SceneConfigData.Instance.GetGate(result.accountData.Id);
            //生成对应的登录令牌，让游戏服进行验证，防止第三方进行恶意登录和攻击
            RSAEncryptComponent rsaComponent= session.Scene.GetComponent<RSAEncryptComponent>();
            //生成登录令牌Token 用来验证身份
            response.token= rsaComponent.GenerateToken(result.accountData.Id,gateResult.sceneConfigid);
            //分配一个客户端连接的网关地址
            response.address=gateResult.address;
            response.accountid = result.accountData.Id;
           await FTask.CompletedTask; 
        }
    }
}
