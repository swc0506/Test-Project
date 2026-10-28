using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [Timer(TimerType.AccountSessionCheckTimeout)]
    public class AccountSessionCheckTimeout : ATimer<AccountCheckoutTimeComponent>
    {
        public override void Run(AccountCheckoutTimeComponent self)
        {
            try
            {
                self.DeleteSession();

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }

    [ObjectSystem]
    public class AccountCheckoutTimeComponentAwakeSystem : AwakeSystem<AccountCheckoutTimeComponent, long>
    {
        public override void Awake(AccountCheckoutTimeComponent self, long accountId)
        {
            self.AccountId = accountId;
            TimerComponent.Instance.Remove(ref self.Timer);
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 600000, TimerType.AccountSessionCheckTimeout, self);
        }
    }

    [ObjectSystem]
    public class AccountCheckoutTimeComponentDestroySystem : DestroySystem<AccountCheckoutTimeComponent>
    {
        public override void Destroy(AccountCheckoutTimeComponent self)
        {
            self.AccountId = 0;
            TimerComponent.Instance.Remove(ref self.Timer);
        }
    }

    [FriendClass(typeof(AccountCheckoutTimeComponent))]
    public static class AccountCheckoutTimeComponentSystem
    {
        public static void DeleteSession(this AccountCheckoutTimeComponent self)
        {
            Session session = self.GetParent<Session>();
            long sessionInstanId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(self.AccountId);
            if (sessionInstanId == session.InstanceId)
            {
                session.DomainScene().GetComponent<AccountSessionsComponent>().Remove(self.AccountId);
            }

            session?.Send(new A2C_Disconnect() { Error = 1 });
            session.Disconnect().Coroutine();
        }
    }
}
