/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/24 11:29:17
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

namespace ZMGC.Hall
{
    public class LoginLogicCtrl : ILogicBehaviour
    {
        private UserDataMgr userDataMgr;
        
        public void OnCreate()
        {
            Debugger.Log("LoginLogicCtrl OnCreate");
            userDataMgr = HallWorld.GetExitsDataMgr<UserDataMgr>();
        }

        public void OnDestroy()
        {
        }
        
        public void LoginSuccess(UserData userData)
        {
            userDataMgr.CacheUserData(userData);
        }
        
        public void LoginFailed(ResultCode resultCode)
        {
            if (resultCode == ResultCode.AccountNotFind)
            {
                //弹出创建角色
                UIEventControl.DispensEvent(UIEventEnum.ShowCreateRoleWindow);
            }
        }

        public void EnterHall()
        {
            Debugger.Log("EnterHall");
        }
    }
}