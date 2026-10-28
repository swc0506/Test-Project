using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class A2C_DisconnectHandler : AMHandler<A2C_Disconnect>
    {
        protected override async void Run(Session session, A2C_Disconnect message)
        {
            Log.Debug(message.Error.ToString());
            await ETTask.CompletedTask;
        }
    }
}
