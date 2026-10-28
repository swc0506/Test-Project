using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class TestLoginRequestHandler:MessageRPC<TestLoginRequest,TestLoginResponse>
    {
        protected override async FTask Run(Session session, TestLoginRequest request, TestLoginResponse response, Action reply)
        {
            Log.Debug("Account:" + request.account);
            Log.Debug("Pass:" + request.pass);

            response.ErrorCode = 5;
            await FTask.CompletedTask; 
        }
    }
}
