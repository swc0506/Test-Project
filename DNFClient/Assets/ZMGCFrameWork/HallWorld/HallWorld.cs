using Fantasy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZM.AssetFrameWork;
using ZMGC.Battle;
//using ZM.AssetFrameWork;

namespace ZMGC.Hall
{
    /// <summary>
    /// 当前世界在构建完成时，该世界下的所属逻辑层、数据层、消息层脚本全都由框架统一构建，可自由获取。
    /// 当世界销毁时，该世界下的所属逻辑层、数据层、消息层脚本全都统一由框架从内存中释放，无法获取。
    /// 判断当前属于哪个世界，可通过 WorldManager.IsHallWorld/IsBattleWorld...
    /// HallWorld一般常驻内存不销毁，销毁的是游戏世界。因为大厅数据层一般存放的有用户各种数据和信息。
    /// </summary>
    public class HallWorld : World
    {
        public static UserDataMgr UserData { get; private set; }
        public override void OnCretae()
        {
            base.OnCretae();
            UserData = GetExitsDataMgr<UserDataMgr>();
            Debug.Log("HallWorld  OnCretae>>>");
            //大厅世界构建完成，弹出大厅弹窗
            AudioController.GetInstance().PlayMusicFade(AssetPathConfig.GAME_AUIDO_PATH+ "BG/Login.mp3",2);
            UIModule.PopUpWindow<LoginWindow>();
        }
        /// <summary>
        /// 从登录页进入大厅
        /// </summary>
        public static void EnterHallWorldFormLogin()
        {
            UIModule.Instance.PopUpWindow<LoadingWindow>();
            //进入游戏流程：加载大厅场景- 销毁所有弹窗，解除资源引用 =>  释放内存中无引用的资源 
            LoadSceneManager.Instance.LoadSceneAsync(AssetPathConfig.SCENE_PATH+"Hall", async () => {
                //弹出大厅弹窗
                UIModule.PopUpWindow<HallWindow>();
                //弹出组队弹窗
                UIModule.PopUpWindow<TeamWindow>();
                //初始化大厅角色
                await GetExitsLogicCtrl<HallRoleLogicCtrl>().Initlize();
                //加载指定的地图
                GetExitsLogicCtrl<MapLogicCtrl>().Initlize();
            });
        }
        /// <summary>
        /// 进入战斗世界
        /// </summary>
        public static void EnterBattleWorld(List<RoleData> teamRoleInfoList)
        {
            UIModule.PopUpWindow<DungeonLoadingWindow>().InitView(teamRoleInfoList);
            //创建英雄
            GetExitsLogicCtrl<HallRoleLogicCtrl>().OnDestroy();
            GetExitsLogicCtrl<MapLogicCtrl>().OnDestroy();
            //加载战斗场景
            LoadSceneManager.Instance.LoadSceneAsync("Battle",()=> {
                GetExitsDataMgr<TeamDataMgr>().OnDestroy();
            });
        }

        /// <summary>
        /// 从游戏内返回至大厅  初始化一些大厅数据和状态
        /// </summary>
        /// <param name="args">游戏退出携带的一些相关参数，如:是否在来一局，跳转至某界面</param>
        public static void EnterHallWorldFormGame(object args=null)
        {
            //退出游戏流程：销毁所有弹窗，解除资源引用 =>  释放内存中无引用的资源 
            WorldManager.SetDefaultWorld(WorldEnum.HallWorld);
            HallWorld.UIModule.DestroyAllWindow();
            ZMAsset.ClearResourcesAssets(false);
            UIModule.Instance.PopUpWindow<LoadingWindow>();
            LoadSceneManager.Instance.LoadSceneAsync(AssetPathConfig.SCENE_PATH + "Hall",async () =>
            {   
                //弹出大厅弹窗
                UIModule.PopUpWindow<HallWindow>();
                //弹出组队弹窗
                UIModule.PopUpWindow<TeamWindow>();
                //初始化大厅角色
                await GetExitsLogicCtrl<HallRoleLogicCtrl>().Initlize();
                //加载指定的地图
                //GetExitsLogicCtrl<MapLogicCtrl>().Initlize();
                await GetExitsLogicCtrl<MapLogicCtrl>().LoadMapAsync(MapType.HeDunMaer, DoorType.Dungeons, 0);
            });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("HallWorld  OnDestroy>>>");
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            Debug.Log("HallWorld  OnDestroyPostProcess>>>");

        }
    }
}