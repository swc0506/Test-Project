using Fantasy;
using Model.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix 
{
    public class RegisterAccountRequestHandler : MessageRPC<RegisterAccountReqest, RegisterAccountResponse>
    {
        protected override async FTask Run(Session session, RegisterAccountReqest request, RegisterAccountResponse response, Action reply)
        {
            //1. 通过 Http API 接口实现登录注册，以及账号鉴权，（鉴权：）校验账号是否合法和存在，若没有问题 ，则返回加密的唯一凭证 Token，客户端使用该Token进行登录游戏
            //2. 通过 Socket 实现登录注册
            // fantasy 服务端 基于组件形式进行业务逻辑开发

            //1.获取账号鉴权组件
            AuthenticationAccountComponent accountComponent= session.Scene.GetComponent<AuthenticationAccountComponent>();
            //2.注册账号
            uint result= await accountComponent.RegisterAccount(request.account,request.passWorld);
            if (result == 0)
            {
                response.account = request.account;
                response.passWorld = request.passWorld;
            }
            //返回错误码
            response.ErrorCode = result;
            Log.Info($"注册账号完成 结果:{response.ErrorCode}");

            await FTask.CompletedTask;
        }
    }
}
