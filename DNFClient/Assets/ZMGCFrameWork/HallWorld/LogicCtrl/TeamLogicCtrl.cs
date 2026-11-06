/*--------------------------------------------------------------------------------------
/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/7/29 7:57:39
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using System;

namespace ZMGC.Hall
{
    public enum TeamState
    {
        None,
        JoinPlayer,//玩家加入
        PlayerExit,//玩家退出
        TeamDissolved,//队伍解散
    }

    public class TeamLogicCtrl : ILogicBehaviour
    {

        private TeamMsgMgr mTeamMsgLayer;
        private TeamDataMgr mTeamDataLayer;
        public void OnCreate()
        {
            mTeamMsgLayer = HallWorld.GetExitsMsgMgr<TeamMsgMgr>();
            mTeamDataLayer = HallWorld.GetExitsDataMgr<TeamDataMgr>();
        }

        /// <summary>
        /// 创建队伍
        /// </summary>
        /// <returns></returns>
        public async FTask<bool> CreateTeam()
        {
            //发送创建队伍请求
            MapType mapType = HallWorld.GetExitsLogicCtrl<MapLogicCtrl>().MapType;
            CreateTeamResponse response = await mTeamMsgLayer.SendCreateTeamRequest(mapType, HallWorld.UserData.AccountId);
            if (response.ErrorCode!=0)
            {
                ToastManager.ShowToast("创建队伍失败:"+response.ErrorCode);
                return false;
            }
            ToastManager.ShowToast("创建队伍成功");
            //缓存队伍数据
            mTeamDataLayer.CacheTeamRole(response.roleData,response.tameid);
            //派发UI事件，让UI进行表现
            UIEventControl.DispensEvent(UIEventEnum.RefreshTeamList);
            return true;
        }
        /// <summary>
        /// 加入队伍
        /// </summary>
        /// <returns></returns>
        public async FTask<bool> JoinTeam(string teamid)
        {
            if (string.IsNullOrEmpty(teamid)||teamid.Length<5)
            {
                ToastManager.ShowToast($"队伍id不合法"+teamid);
                return false;
            }
            //发送创建队伍请求
            MapType mapType = HallWorld.GetExitsLogicCtrl<MapLogicCtrl>().MapType;
            JoinTeamResponse response = await mTeamMsgLayer.SendJoinTeamRequest(mapType, HallWorld.UserData.AccountId,int.Parse(teamid));
            if (response.ErrorCode != 0)
            {
                ToastManager.ShowToast("加入队伍失败:" + response.ErrorCode);
                return false;
            }
            ToastManager.ShowToast("加入队伍成功");
            foreach (var roleData in response.roleList)
            {
                //缓存队伍数据
                mTeamDataLayer.CacheTeamRole(roleData, response.tameid);
            }
         
            //派发UI事件，让UI进行表现
            UIEventControl.DispensEvent(UIEventEnum.RefreshTeamList);
            return true;
        }
        //1.为什么要分层编写逻辑，好处是什么？
        //2.解耦，规范化设计，数据即数据、消息即消息、逻辑即逻辑、秉持设计模式中的单一原则，互相驱动，并不相互绑定，
        //消息层、数据层、逻辑层、别的地方都可以直接使用。好拓展、好管理、好移植。
        //3.符合设计模式中单一原则(数据层只负责数据的维护、逻辑层只负责业务逻辑的处理，消息层只负责消息的收发)
        //4.代码清晰、简洁、美观、如果三层都混在一起，耦合性非常高，别处访问脚本也会对内部类产生影响。混在一起编写代码可以说是一坨代码，而不是一套有规范的代码
        // 代码都一坨了，你还妄想他不会出问题？
        //5.模块化设计、解耦、避免牵一发而动全身的问题。
        /// <summary>
        /// 队伍玩家变化通知
        /// </summary>
        public void OnTeamRoleStateChangeNotify(TeamState teamState,RoleData roleData)
        {
            if (teamState == TeamState.JoinPlayer)//玩家加入
            {
                mTeamDataLayer.CacheTeamRole(roleData,-1);
            }
            else if (teamState == TeamState.PlayerExit)
            {
                mTeamDataLayer.RemoveTeamRole(roleData.uid);
            }
            else if (teamState == TeamState.TeamDissolved)
            {
                mTeamDataLayer.ClearTeamData();
            }
            //派发UI事件，让UI进行表现
            UIEventControl.DispensEvent(UIEventEnum.RefreshTeamList);
        }
        public void OnDestroy()
        {

        }

    }
}
