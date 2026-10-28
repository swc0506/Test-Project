using Fantasy;
using Hotfix.Gate;
using Model.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class CreateRoleRequestHandler : MessageRPC<CreateRoleReqest, CreateRoleResponse>
    {
        protected override async FTask Run(Session session, CreateRoleReqest request, CreateRoleResponse response, Action reply)
        {
            RoleManagerComponent roleComponent = session.Scene.GetComponent<RoleManagerComponent>();
            var result= await roleComponent.CreateRole(request.roleid,request.accountid,request.roleName);
            if (result.errorCode==0)
            {
                response.roleData = new RoleData
                {
                    uid=result.role.Id,
                    level=result.role.level,
                    roleid=result.role.roleid,
                    roleName=result.role.roleName,
                };
                //roleComponent.useRoleCacheDic.Add(request.accountid,result.role);
                roleComponent.SelectUseRole(result.role);
             }
            response.ErrorCode=result.errorCode;
            result.role.session = session;
            Log.Debug("创建角色响应 Error Code:"+response.ErrorCode);
            await FTask.CompletedTask;
        }
    }
}
