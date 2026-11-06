/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/28 15:30:32
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;

namespace ZMGC.Hall
{
    public class HallRoleMsgMgr : IMsgBehaviour
    {

        public void OnCreate()
        {

        }

        public class OtherPlayerStatusSyncMessageHandler : Message<OtherPlayerStateSyncMessage>
        {
            protected override async FTask Run(Session session, OtherPlayerStateSyncMessage message)
            {
                ProtoBuffConvert.ToJson(message);
                HallWorld.GetExitsLogicCtrl<HallRoleLogicCtrl>().SyncOtherRoleState(message);

                await FTask.CompletedTask;
            }
        }
        public void OnDestroy()
        {

        }

    }
}
