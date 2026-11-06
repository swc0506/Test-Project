using Fantasy;
using Model.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Gate
{
    public static class RoleManagerComponentSystem
    {
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roleid"></param>
        /// <param name="accountid"></param>
        /// <param name="roleName"></param>
        public static async FTask<(uint errorCode, Role? role)> CreateRole(this RoleManagerComponent self, int roleid, long accountid, string roleName)
        {
            if (roleid == 0 || accountid == 0 || string.IsNullOrEmpty(roleName))
            {
                //账户 角色 昵称不符合规范
                return (1200, null);
            }
            int nameHashCode = roleName.GetHashCode();
            using (await self.Scene.CoroutineLockComponent.Lock(nameHashCode, nameHashCode,"CreateRole Lock"))
            {
                //获取数据库
                IDateBase dateBase = self.Scene.World.DateBase;
                //开始创建角色
                //1.验证缓存中是否存在这个角色

                if (self.roleDataCacheDic.ContainsKey(nameHashCode))
                {
                    //改昵称已经被使用
                    return (1201, null);
                }
                //2.查询数据库中是否存在该昵称的角色
                bool isExist = await dateBase.Exist<Role>(d => d.roleName == roleName);
                if (isExist)
                {
                    return (1201, null);
                }
                //3.检验该账户下创建的角色是否达到了上限
                List<Role> roleList = await dateBase.Query<Role>(d => d.accountid == accountid);
                //一般来说默认为5个角色栏，剩余的角色可以花钻石进行购买，这个功能留给同学们
                if (roleList.Count > 10)
                {
                    return (1202, null);
                }
                //4.验证roleid是否存在，防止恶意 ，通过查询配置表进行验证。TODO

                //5.所有验证都已通过，可以创建角色了
                Role role = Entity.Create<Role>(self.Scene, true);
                role.accountid = accountid;
                role.roleid = roleid;
                role.roleName = roleName;
                role.level = 1;
                //把该角色添加至缓存中
                self.roleDataCacheDic.Add(nameHashCode, role);
                //把Role数据写到数据库中
                await dateBase.Save(role);
                return (0, role);
            }
        }
        
        /// <summary>
        /// 获取该账户下所有已经创建的角色列表
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public static async FTask<List<Role>> GetAccountRoleList(this RoleManagerComponent self, long accountid)
        { //获取数据库
            IDateBase dateBase = self.Scene.World.DateBase;
            return await dateBase.Query<Role>(d => d.accountid == accountid);
        }
        /// <summary>
        /// 获取该账户下所有已经创建的角色
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public static async FTask<Role?> GetAccountRole(this RoleManagerComponent self, long roleUid)
        { 
            //有限从缓存中进行查询 

            //获取数据库
            IDateBase dateBase = self.Scene.World.DateBase;
            return await dateBase.First<Role>(d => d.Id == roleUid);
        }
        /// <summary>
        /// 获取玩家选择的角色
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roleUid"></param>
        /// <returns></returns>
        public static Role GetPlayerUseRole(this RoleManagerComponent self, long accountid)
        {
            self.useRoleCacheDic.TryGetValue(accountid, out Role role);
            return role;
        }

        /// <summary>
        /// 选择使用的角色
        /// </summary>
        /// <param name="self"></param>
        /// <param name="role"></param>
        public static void SelectUseRole(this RoleManagerComponent self, Role role)
        {
            if (self.useRoleCacheDic.ContainsKey(role.accountid))
            {
                self.useRoleCacheDic[role.accountid] = role;
            }
            else
            {
                self.useRoleCacheDic.Add(role.accountid,role);
            }
        }
    }
}
