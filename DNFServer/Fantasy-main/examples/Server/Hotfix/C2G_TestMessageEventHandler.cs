using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class C2G_TestMessageEventHandler : Message<C2G_TestMessageEvent>
    {
        protected override async FTask Run(Session session, C2G_TestMessageEvent message)
        {
            Log.Debug(message.message);
            Log.Debug("frameOpCode:"+ message.frameOpCode);
            //处理某些事情，等待其他玩家副本加载完成
            session.Send(new G2C_TestSeverToClientMessage{ message="这是一条服务端主动推送到客户端的消息",frameOpCode=message.frameOpCode});
            await FTask.CompletedTask;
        }
    }
}
