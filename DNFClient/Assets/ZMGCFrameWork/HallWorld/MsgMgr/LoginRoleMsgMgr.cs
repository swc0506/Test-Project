/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/6 19:44:53
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;

namespace ZMGC.Hall
{
    public class LoginRoleMsgMgr : IMsgBehaviour
    {

        public void OnCreate()
        {

        }

        /// <summary>
        /// 发送角色创建请求
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="nickName"></param>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public async FTask<CreateRoleResponse> SendCreateRoleRequest(int roleid,string nickName,long accountid)
        {
            CreateRoleReqest reqest = new CreateRoleReqest();
            reqest.roleid = roleid;
            reqest.accountid = accountid;
            reqest.roleName = nickName;
             return await  NetWorkManager.Instance.SendCallMessage<CreateRoleResponse>(reqest);
        }
        /// <summary>
        /// 发送选择角色请求
        /// </summary>
        /// <param name="roleUid"></param>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public async FTask<SelectRoleResponse> SendSelectRoleRoleRequest(long roleUid,long accountid)
        {
            SelectRoleReqest reqest = new SelectRoleReqest();
            reqest.roleUid = roleUid;
            reqest.accountid = accountid;
            return await NetWorkManager.Instance.SendCallMessage<SelectRoleResponse>(reqest);
        }
        public void OnDestroy()
        {

        }

    }
}
