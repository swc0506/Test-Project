using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.Hall
{
    public class LoginLogicCtrl : ILogicBehaviour
    {
        private LoginMsgMgr mLoginMsg;
        public void OnCreate()
        {
            mLoginMsg = HallWorld.GetExitsMsgMgr<LoginMsgMgr>();
        }

        public int AccountLogin(string account, string pass)
        {
            if (account.Length < 6)
            {
                return 1;
            }

            if (pass.Length < 4)
            {
                return 2;
            }
            mLoginMsg.SendLoginReqeust(account, pass);
            return 0;
        }
        public void OnLoginResult(UserDataServerModelTest user)
        {
            UserDataMgr userData= HallWorld.GetExitsDataMgr<UserDataMgr>();
            userData.CacheUserData(user);

            //通过事件发送到UI层 让UI去更新界面
            UIEventControl.DispensEvent(UIEventEnum.LoginSuccess);
            Debug.Log("登录成功 userid:"+user.id+"  userName:"+user.name +" userGold:"+user.gold);
        }
        public void OnDestroy()
        {
          
        }
    }
}