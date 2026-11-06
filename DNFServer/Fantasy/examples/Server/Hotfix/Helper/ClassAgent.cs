using Fantasy;
using Model;
using Model.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Helper
{
    public static class ClassAgent
    {
        public static CSVector3 ToCSVector3(this SVector3 s3)
        {
            CSVector3 csv3= new CSVector3();
            csv3.x = s3.x;
            csv3.y = s3.y;
            csv3.z = s3.z;
            return csv3;
        }

        public static SVector3 ToSVector3(this CSVector3 cs3)
        {
          return  new SVector3(cs3.x,cs3.y,cs3.z);
        }

        public static RoleData ToRoleData(this Role role)
        {
            return new RoleData { uid = role.Id, level = role.level, roleid = role.roleid, roleName = role.roleName,accountid=role.accountid };
        }
    }
}
