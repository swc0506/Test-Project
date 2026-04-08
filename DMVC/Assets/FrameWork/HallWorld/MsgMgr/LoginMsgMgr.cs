using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Hall
{
    public class LoginMsgMgr : IMsgBehaviour
    {
        public void OnCreat()
        {
            
        }

        public void OnDestroy()
        {
            
        }
        
        public void SendLoginReq(string account, string password)
        {
            //发送
            
            OnLoginRsp();
        }
        
        private void OnLoginRsp()
        {
            //处理
            UserData data = new UserData();
            data.id = 1;
            data.name = "test";
            data.gold = 100;
            HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>().OnLogin(data);
        }
    }
}