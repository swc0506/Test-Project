/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2024/6/29 23:31:55
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;

namespace ZMGC.Hall
{
    public class LoginMsgMgr : IMsgBehaviour
    {

        public void OnCreate()
        {
            
        }

        /// <summary>
        /// 发送注册账号请求
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async FTask<RegisterAccountResponse> SendRegisterAccountRequest(string account,string password)
        {
            //连接服务器成功
            RegisterAccountReqest reqest = new RegisterAccountReqest();
            reqest.account = account;
            reqest.passWorld = password;
            RegisterAccountResponse response = await NetWorkManager.Instance.SendCallMessage<RegisterAccountResponse>(reqest);
            return response;
        }
        /// <summary>
        /// 发送获取Token请求
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async FTask<GetTokenResponse> SendGetTokenRequest(string account, string password)
        {
            //连接服务器成功
            GetTokenReqest reqest = new GetTokenReqest();
            reqest.account = account;
            reqest.passWorld = password;
            GetTokenResponse response = await NetWorkManager.Instance.SendCallMessage<GetTokenResponse>(reqest);
            return response;
        }
        /// <summary>
        /// 发送登录Gate服请求
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async FTask<LoginGateResponse> SendLoginGateRequest(long accountId, string token)
        {
            //连接服务器成功
            LoginGateReqest reqest = new LoginGateReqest();
            reqest.accountid = accountId;
            reqest.token = token;
            LoginGateResponse response = await NetWorkManager.Instance.SendCallMessage<LoginGateResponse>(reqest);
            return response;
        }
        public void OnDestroy()
        {

        }

    }
}
