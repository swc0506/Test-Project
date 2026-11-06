/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/6 19:44:37
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;

namespace ZMGC.Hall
{
    public class LoginRoleLogicCtrl : ILogicBehaviour
    {
        private LoginRoleMsgMgr mLoginRoleMsgLayer;
        public void OnCreate()
        {
            mLoginRoleMsgLayer= HallWorld.GetExitsMsgMgr<LoginRoleMsgMgr>();
        }

        public async FTask<uint> CreateRole(int roleid,string roleName)
        {
            if (roleid == 0||string.IsNullOrEmpty(roleName))
            {
                return 1;
            }
            //发送创建角色请求
            CreateRoleResponse response= await mLoginRoleMsgLayer.SendCreateRoleRequest(roleid,roleName,HallWorld.UserData.AccountId);
            if (response.ErrorCode!=0) 
            {
                ToastManager.ShowToast("创建角色失败:"+response.ErrorCode);
                return response.ErrorCode;
            }
            
            //创建角色成功 加载大厅场景，跳转至大厅
            Debuger.LogGreen("创建角色成功...");
            HallWorld.UserData.CacheSelectRole(response.roleData);
            //进入大厅
            HallWorld.EnterHallWorldFormLogin();
            return 0;
        }

        /// <summary>
        /// 选择角色进入大厅
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public async FTask<uint> SelectRoleEnterHall()
        {
            if (HallWorld.UserData.Role.uid == 0 )
            {
                return 1;
            }
            //发送创建角色请求
            SelectRoleResponse response = await mLoginRoleMsgLayer.SendSelectRoleRoleRequest(HallWorld.UserData.Role.uid,HallWorld.UserData.AccountId);
            if (response.ErrorCode != 0)
            {
                ToastManager.ShowToast("角色不存在:" + response.ErrorCode);
                return response.ErrorCode;
            }
            //创建角色成功 加载大厅场景，跳转至大厅
            Debuger.LogGreen("角色选择成功...");
            //进入大厅
            HallWorld.EnterHallWorldFormLogin();
            return 0;
        }
        public void OnDestroy()
        {

        }

    }
}
