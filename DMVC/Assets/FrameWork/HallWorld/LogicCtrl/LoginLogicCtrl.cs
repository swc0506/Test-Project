using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Hall
{
    public class LoginLogicCtrl : ILogicBehaviour
    {
        private LoginMsgMgr mLoginMsg;
        
        public void OnCreat()
        {
            mLoginMsg = HallWorld.GetExitsMsgMgr<LoginMsgMgr>();
        }

        public void OnDestroy()
        {
        }

        public int AccountLogin(string account, string password)
        {
            if (account.Length < 6)
            {
                return 1;
            }

            if (password.Length < 4)
            {
                return 2;
            }

            mLoginMsg.SendLoginReq(account, password);
            return 0;
        }
        
        public void OnLogin(UserData data)
        {
            UserDataMgr userDataMgr = HallWorld.GetExitsDataMgr<UserDataMgr>();
            userDataMgr.CacheUserData(data);
            
            Debug.Log("登录成功 id:" + data.id + " name:" + data.name + " gold:" + data.gold);
            UIEventControl.DispatchEvent(UIEventEnum.LoginSuccess);
        }
    }
}