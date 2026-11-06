using Fantasy;
using Hotfix.Gate;
using Hotfix.Share;
using Model.Gate;
using Model.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix 
{
    public class LoginGateRequestHandler : MessageRPC<LoginGateReqest, LoginGateResponse>
    {
        protected override async FTask Run(Session session, LoginGateReqest request, LoginGateResponse response, Action reply)
        {
            //验证Token是否合法
            RSAEncryptComponent rsaComponent = session.Scene.GetComponent<RSAEncryptComponent>();
            RoleManagerComponent  roleComponent= session.Scene.GetComponent<RoleManagerComponent>();
            bool verifyResult= rsaComponent.VerifyToken(request.token,request.accountid,session.Scene.SceneConfigId);
            if (!verifyResult)
            {
                //Token不合法，验证失败
                Log.Debug("Token不合法，验证失败 accountid:"+request.accountid);
                //断开连接，认为是非法登录
                session.Dispose();
                response.ErrorCode = 1011;
                return;
            }
            //挂载监听Session断开组件，用户处理客户端断开的逻辑
            var sessionComponent= session.AddComponent<SessionDisposeCompoent>();
            sessionComponent.accountId = request.accountid;
            //Token验证成功， 合法
            response.ErrorCode = 0;
            response.accountid = request.accountid;
            response.diamond = 99999;
            response.gold = 999999;
            List<Role> roleList= await roleComponent.GetAccountRoleList(response.accountid);
            if (roleList!=null)
            {
                response.roleList = new List<RoleData>();
                foreach (Role role in roleList)
                {
                    response.roleList.Add(new RoleData { level=  role.level, roleid=role.roleid, uid=role.Id, roleName=role.roleName });
                }
            }
           
            Log.Debug("玩家登录Gate服成功:" + request.accountid);
            //返回该账户下所有已经创建的角色列表和角色信息 TODO

            await FTask.CompletedTask;
        }
    }
}
