using Fantasy;
using Hotfix.System.Dungeons;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class FrameOperateEventMessageHandler : Message<FrameOperateEventMessage_C2G>
    {
        protected override async FTask Run(Session session, FrameOperateEventMessage_C2G message)
        {

            //1.收集并缓存所有客户端发送过来的帧操作请求，在下一个逻辑帧广播给所有的客户端 
            //这样就能够实现操作的同步 即：相同的时机+相同输入
            var dungeonComponent = session.Scene.GetComponent<DungeonsManagerComponent>();
            Log.Info("同步玩家帧操作数据"+message.battleid);
            dungeonComponent.OnPlayerOperateFrameInput(message.battleid, message.frameDataList);

            await FTask.CompletedTask;
        }
    }
}
