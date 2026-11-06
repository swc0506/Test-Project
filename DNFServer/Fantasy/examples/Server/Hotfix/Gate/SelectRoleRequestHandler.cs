using Fantasy;
using Model.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Gate
{
    public class SelectRoleRequestHandler : MessageRPC<SelectRoleReqest, SelectRoleResponse>
    {
        protected override async FTask Run(Session session, SelectRoleReqest request, SelectRoleResponse response, Action reply)
        {
            //在数据库中查询该账户是否拥有该角色
            RoleManagerComponent roleComponent = session.Scene.GetComponent<RoleManagerComponent>();
            Role role= await roleComponent.GetAccountRole(request.roleUid);
            if (role==null)
            {
                response.ErrorCode = 1204;//该角色不存在
                return;
            }
            response.ErrorCode = 0;
            response.roleData = new RoleData { level = role.level, roleid = role.roleid, roleName = role.roleName, uid = role.Id };
            //选择角色
            roleComponent.SelectUseRole(role);
            role.session = session;
            await FTask.CompletedTask;
        }
    }
}
