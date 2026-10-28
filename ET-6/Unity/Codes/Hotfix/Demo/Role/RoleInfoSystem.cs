using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{

    [FriendClass(typeof(RoleInfo))]
    public static class RoleInfoSystem 
    {
        public static void FromMessage(this RoleInfo self, RoleInfoProto roleInfoProto)
        {
            self.Id = roleInfoProto.Id;
            self.Name = roleInfoProto.Name;
            self.State = roleInfoProto.State;
            self.AccountId = roleInfoProto.AccountId;
            self.CreatTime = roleInfoProto.CreateTime;
            self.ServerId = roleInfoProto.ServerId;
            self.LastLoginTine = roleInfoProto.LastLoginTime;
        }

        public static RoleInfoProto ToMessage(this RoleInfo self)
        {
            return new RoleInfoProto
            {
                Id = self.Id,
                Name = self.Name,
                State = self.State,
                AccountId = self.AccountId,
                CreateTime = self.CreatTime,
                ServerId = self.ServerId,
                LastLoginTime = self.LastLoginTine
            };
        }
    }
}
