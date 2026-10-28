using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ObjectSystem]
    public class AccountSessionsComponentDestroySystem : DestroySystem<AccountSessionsComponent>
    {
        public override void Destroy(AccountSessionsComponent self)
        {
            self.AccountSessionDictionary.Clear();
        }
    }


    [FriendClass(typeof(AccountSessionsComponent))]
    public static class AccountSessionsComponentSystem
    {
        public static long Get(this AccountSessionsComponent self, long accountId)
        {
            if (!self.AccountSessionDictionary.TryGetValue(accountId, out long instanceId))
            {
                return 0;
            }

            return instanceId;
        }

        public static void Add(this AccountSessionsComponent self, long accountId, long sessionInstanceId)
        {
            if (self.AccountSessionDictionary.ContainsKey(accountId))
            {
                self.AccountSessionDictionary[accountId] = sessionInstanceId;
                return;
            }
            self.AccountSessionDictionary.Add(accountId, sessionInstanceId);
        }

        public static void Remove(this AccountSessionsComponent self, long accountId)
        {
            if (self.AccountSessionDictionary.ContainsKey((accountId)))
            {
                self.AccountSessionDictionary.Remove((accountId));
            }
        }
    }
}
