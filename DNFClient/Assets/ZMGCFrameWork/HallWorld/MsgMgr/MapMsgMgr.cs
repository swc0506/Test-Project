/*--------------------------------------------------------------------------------------
* Title: 网络消息层脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/28 0:04:44
* Description:网络消息层,主要负责游戏网络消息的收发
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;

namespace ZMGC.Hall
{
    public class MapMsgMgr : IMsgBehaviour
    {

        public void OnCreate()
        {

        }

        /// <summary>
        /// 发送进入地图请求
        /// </summary>
        /// <param name="playerid"></param>
        /// <param name="curMapType"></param>
        /// <param name="enterMapType"></param>
        /// <param name="doorType"></param>
        /// <returns></returns>
        public async FTask<EnterMapResponse> SendEnterMapRequest(long playerid, MapType curMapType, MapType enterMapType, DoorType doorType)
        {
            EnterMapReqest request = new EnterMapReqest();
            request.playerid = playerid;
            request.mapType = (int)enterMapType;
            request.curMap = (int)curMapType;
            request.doorType = (int)doorType;
            return await NetWorkManager.Instance.SendCallMessage<EnterMapResponse>(request);

        }
        public void OnDestroy()
        {

        }

    }
}
