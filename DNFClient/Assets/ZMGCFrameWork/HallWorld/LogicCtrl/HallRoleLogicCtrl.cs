/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/7 15:05:04
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using ZM.AssetFrameWork;

namespace ZMGC.Hall
{
    public enum PlayerMapStatus
    {
        None = 0,
        InMap = 1,
        ExitMap = 2,
    }

    /// <summary>
    /// 大厅角色逻辑脚本，后续会控制不同玩家之间的状态同步 包括 位置、状态等
    /// </summary>
    public class HallRoleLogicCtrl : ILogicBehaviour
    {
        public int RoleId => HallWorld.UserData.RoleId;

        private AssetsRequest mRoleAsset;
        /// <summary>
        /// 当前场景中的角色
        /// </summary>
        public Role_Hall Role { get; private set; }
        /// <summary>
        /// 其他玩家字典
        /// </summary>
        public Dictionary<long, Role_Hall> otherRoleDic = new Dictionary<long, Role_Hall>();
        public void OnCreate()
        {

        }
        /// <summary>
        /// 初始化角色
        /// </summary>
        public async FTask<bool> Initlize()
        {
            mRoleAsset = await ZMAsset.InstantiateAsync(AssetPathConfig.Hall_ROLE_PATH + "Role_" + RoleId);
            GameObject roleObj = mRoleAsset.obj;
            //初始化位置
            roleObj.transform.position = new Vector3(0, 0, 9);
            Role = roleObj.AddComponent<Role_Hall>();
            Role.Initlizate(RoleSource.Self,RoleId);
            Role.ActiveMove(true);
            return true;
        }

        /// <summary>
        /// 同步当前玩家状态
        /// </summary>
        /// <param name="syncData"></param>
        /// <param name="statePacketId"></param>
        public async void SyncRoleState(StateSyncData syncData, long statePacketId)
        {
            //状态同步请求
            StateSyncReqest request = new StateSyncReqest();
            request.statePacketId = statePacketId;
            request.roleSyncData = syncData;
            //状态同步数据
            syncData.mapType = (int)HallWorld.GetExitsLogicCtrl<MapLogicCtrl>().MapType;
            syncData.roleid = RoleId;
            syncData.playerid = HallWorld.UserData.AccountId;
            //发送状态同步请求
            StateSyncResponse response = await NetWorkManager.Instance.SendCallMessage<StateSyncResponse>(request);

            //同步角色状态
            Role.SyncPosition(response.roleSyncData.position, response.roleSyncData.inputDir);
        }
        /// <summary>
        /// 同步其他玩家状态
        /// </summary>
        /// <param name="message"></param>
        public void SyncOtherRoleState(OtherPlayerStateSyncMessage message)
        {
            //设置新位置
            //更新状态表现等等
            if (message.roleData.playerMapStatus == (int)PlayerMapStatus.InMap)
            {
                //玩家处于地图中，
                Debuger.LogYellow($"{message.roleData.playerid} 进行了移动");
                //1.创建该玩家的角色
                Role_Hall otherRole=  GetOtherRole(message.roleData.playerid,message.roleData.roleid, message.roleData.position);
                //2.更新该玩家角色的位置和状态
                otherRole.SyncPosition(message.roleData.position,message.roleData.inputDir);
            }
            else if (message.roleData.playerMapStatus == (int)PlayerMapStatus.ExitMap)
            {
                //玩家离开地图
                ReleaseOtherRoleAsset(message.roleData.playerid);
                Debuger.LogYellow($"{message.roleData.playerid} 玩家离开地图");
            }
        }
        /// <summary>
        /// 获取其他玩家
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="roleCfgId"></param>
        /// <returns></returns>
        private Role_Hall GetOtherRole(long playerId, int roleCfgId,CSVector3 initPos)
        {
            otherRoleDic.TryGetValue(playerId, out var otherRole);
            if (otherRole == null)
            {
                GameObject roleObj = ZMAsset.Instantiate(AssetPathConfig.Hall_ROLE_PATH + "Role_" + roleCfgId, null);
                otherRole= roleObj.AddComponent<Role_Hall>();
                otherRole.Initlizate(RoleSource.OtherPlayer, roleCfgId);
                otherRoleDic.Add(playerId, otherRole);
                otherRole.ActiveMove(true);
                otherRole.InitPlayerStae(initPos.ToVector3());
               
            }
            return otherRole;
        }
        /// <summary>
        /// 释放其他玩家
        /// </summary>
        /// <param name="playerid"></param>
        public void ReleaseOtherRoleAsset(long playerid)
        {
            if (otherRoleDic.TryGetValue(playerid, out var otherRole))
            {
                if (!UnityEngine.Object.ReferenceEquals(otherRole, null))//!object.Equals(otherRole, null) && 
                {
                    otherRole.Release();
                }
                otherRoleDic.Remove(playerid);
            }
        }
        public void OnDestroy()
        {
            //其他玩家的资源释放
            foreach (var item in otherRoleDic)
            {
                if (!UnityEngine.Object.ReferenceEquals(item.Value.gameObject, null))//!object.Equals(otherRole, null) && 
                {
                    item.Value.Release();
                }
            }

            otherRoleDic.Clear();

            mRoleAsset?.Release(true);
            mRoleAsset = null;
        }

    }
}
