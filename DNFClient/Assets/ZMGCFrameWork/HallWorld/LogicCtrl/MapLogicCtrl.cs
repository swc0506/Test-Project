/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/7 14:31:36
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using ZM.AssetFrameWork;

namespace ZMGC.Hall
{
    public class MapLogicCtrl : ILogicBehaviour
    {
        private AssetsRequest mAssetRequest;
        /// <summary>
        /// 当前的地图
        /// </summary>
        public Map Map { get; private set; }
        /// <summary>
        /// 地图类型
        /// </summary>
        public MapType MapType { get; private set; }
        /// <summary>
        /// 地图的门
        /// </summary>
        public DoorType DoorType { get; private set; }

        private MapMsgMgr mMapMsgLayer;
        public void OnCreate()
        {
            mMapMsgLayer = HallWorld.GetExitsMsgMgr<MapMsgMgr>();
        }

        /// <summary>
        /// 初始化地图
        /// </summary>
        public async void Initlize()
        {
           await LoadMapAsync(MapType.Home, DoorType.Home,0);
        }
        public async FTask<Map> LoadMapAsync(MapType mapType, DoorType doorType, float awaitTime=1)
        {
            if (MapType == mapType)
            {
                Debuger.LogError($"{mapType} already Load...");
                return null;
            }
        

            //释放上一个地图资源
            ReleaseMapAsset();

            //黑屏动画
            if (awaitTime > 0)
                UIEventControl.DispensEvent(UIEventEnum.BlackScreen, BlackScreenType.Show);
            else
                UIEventControl.DispensEvent(UIEventEnum.BlackScreen, BlackScreenType.Hide);

            //等待1秒黑屏动画
            await new WaitForSeconds(awaitTime);

            //发送进入地图请求
            EnterMapResponse response=  await mMapMsgLayer.SendEnterMapRequest(HallWorld.UserData.AccountId, MapType, mapType, doorType);
            if (response.ErrorCode != 0) //地图加载失败
            {
                ToastManager.ShowToast("地图加载失败");
                return Map;
            }
            

            MapType = mapType;
            DoorType = doorType;
            //加载对应的地图
            await LoadMapAsset(mapType.ToString());
            //初始化地图角色环境
            InitMapRoleEnv(response.roleInitPos);
            //黑屏淡出动画
            UIEventControl.DispensEvent(UIEventEnum.BlackScreen, BlackScreenType.Hide);
            return Map;
        }
        /// <summary>
        /// 加载地图资源
        /// </summary>
        /// <param name="mapName"></param>
        private async FTask<Map> LoadMapAsset(string mapName)
        {
            //加载地图资源
            mAssetRequest= await ZMAsset.InstantiateAsync(AssetPathConfig.Hall_MAP_PATH+mapName);
            GameObject mapObj = mAssetRequest.obj;
            mapObj.transform.SetParent(null);
            Map= mapObj.GetComponent<Map>();
            //初始化地图位置
            mapObj.transform.position=Map.mapInitPos;
     
            return Map;
        }
        /// <summary>
        /// 初始化角色环境
        /// </summary>
        public void InitMapRoleEnv(CSVector3 roleinitPos)
        {
            //获取当前角色
            Role_Hall role= HallWorld.GetExitsLogicCtrl<HallRoleLogicCtrl>().Role;
            //初始化角色要去的门的位置
            role.transform.position = Map.GetMapCfgData(DoorType).roleInitPos.position;
            //初始化角色碰撞环境
            role.InitCollider(Map);
            //初始化角色跟随摄像机
            CameraFollow.Instance.InitlizateCamera(role.transform,Map.maxPosition,Map.minPosition,10);
            //初始化角色状态
            role.InitPlayerStae(roleinitPos.ToVector3());
            //同步玩家位置
            role.SyncPosition(roleinitPos,new CSVector3());
            //激活角色移动
            role.ActiveMove(true);
        }
        /// <summary>
        /// 释放地图资源
        /// </summary>
        private void ReleaseMapAsset()
        {
            if (Map != null)
            {
                //移除角色对地图碰撞体的监听
                HallWorld.GetExitsLogicCtrl<HallRoleLogicCtrl>().Role.ReleaseCollider();
                Map.ReleaseMapCollider();
                Map = null;
            }

            if (mAssetRequest != null)
            {
                mAssetRequest.Release();
                mAssetRequest=null;
            }
            MapType = MapType.None;
            DoorType= DoorType.None;
        }
        public void OnDestroy()
        {
            ReleaseMapAsset();
        }

    }
}
