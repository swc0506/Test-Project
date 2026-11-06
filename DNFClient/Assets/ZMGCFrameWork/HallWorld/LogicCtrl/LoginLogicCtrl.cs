/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/6/29 23:11:26
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using System.Security.Principal;
using Unity.VisualScripting;
namespace ZMGC.Hall
{
    public class LoginLogicCtrl : ILogicBehaviour
    {
        private LoginMsgMgr mLoginMsgLayer;
        public void OnCreate()
        {
            mLoginMsgLayer = HallWorld.GetExitsMsgMgr<LoginMsgMgr>();
        }
        /// <summary>
        /// 注册账号
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passworld"></param>
        /// <param name="resultCallBack"></param>
        public void RegisterAccount(string account, string passworld,System.Action<int> resultCallBack=null)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(passworld))
            {
                ToastManager.ShowToast("账号或密码不能为空");
                resultCallBack?.Invoke(1);
                return;
            }
            //连接鉴权服务器，发起账号注册请求
            NetWorkManager.Instance.Connect("192.168.2.113:20000", NetworkProtocolType.KCP, async () =>
            {
                //连接服务器成功
                RegisterAccountResponse response= await mLoginMsgLayer.SendRegisterAccountRequest(account,passworld);
                //账号注册失败
                if (response.ErrorCode!=0)
                {
                    resultCallBack?.Invoke(2);
                    ToastManager.ShowToast("账号注册失败:"+response.ErrorCode);
                    return;
                }
                //账号注册成功
                Debuger.LogGreen($"账号注册成功 Account:{response.account} passwold:{response.passWorld}");
                ToastManager.ShowToast("账号注册成功");
                NetWorkManager.Instance.DisConnect();
                resultCallBack?.Invoke(0);
            },
            () =>
            {
                // 连接服务器失败
                ToastManager.ShowToast("请检查网络后重试");
                resultCallBack?.Invoke(1);
            }, null);
        }

        /// <summary>
        /// 获取登录Token
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passworld"></param>
        /// <param name="resultCallBack"></param>
        public async void GetToken(string account,string passworld, System.Action<int> resultCallBack = null)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(passworld))
            {
                ToastManager.ShowToast("账号或密码不能为空");
                resultCallBack?.Invoke(1);
                return;
            }
            //连接鉴权服务器
            NetWorkManager.Instance.Connect("192.168.2.113:20000", NetworkProtocolType.KCP, async () =>
            {
                Debuger.Log("发送获取Token 请求...");
                GetTokenResponse response = await mLoginMsgLayer.SendGetTokenRequest(account, passworld);
                if (response.ErrorCode != 0)
                {
                    resultCallBack?.Invoke(2);
                    ToastManager.ShowToast("获取Token失败:" + response.ErrorCode);
                    return;
                }
                NetWorkManager.Instance.DisConnect();
                Debuger.LogYellow("获取Token成功 token: " + response.token);
                Debuger.LogYellow("获取Token成功 address: " + response.address);
                //发送登录到Gate服请求
                LoginWithAccountToken(response.accountid, response.token, response.address, resultCallBack);
            },
            () =>
            {
                // 连接服务器失败
                ToastManager.ShowToast("请检查网络后重试");
                resultCallBack?.Invoke(1);
            }, null);
        }
        /// <summary>
        /// 登录账号通过Token
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="token"></param>
        /// <param name="address"></param>
        /// <param name="resultCallBack"></param>
        public void LoginWithAccountToken(long accountId,string token,string address, System.Action<int> resultCallBack = null)
        {
            //连接Gate服务器
            NetWorkManager.Instance.Connect(address, NetworkProtocolType.KCP, async () =>
            {
                Debuger.Log("发送获取Token 请求...");
                LoginGateResponse response = await mLoginMsgLayer.SendLoginGateRequest(accountId, token);
                if (response.ErrorCode != 0)
                {
                    resultCallBack?.Invoke(2);
                    ToastManager.ShowToast("登录Gate服失败:" + response.ErrorCode);
                    return;
                }
                resultCallBack?.Invoke(0);
                LoginSuccess(response);
            },
            () =>
            {
                // 连接服务器失败
                ToastManager.ShowToast("请检查网络后重试");
                resultCallBack?.Invoke(1);
            }, null);
        }
        public void LoginSuccess(LoginGateResponse response)
        {
            Debuger.LogGreen("登录Gate服成功: " + response.accountid);
            HallWorld.UserData.CacheAccountData(response);
            UIEventControl.DispensEvent(UIEventEnum.LoginSuccess);
        }
        public void OnDestroy()
        {

        }

    }
}
