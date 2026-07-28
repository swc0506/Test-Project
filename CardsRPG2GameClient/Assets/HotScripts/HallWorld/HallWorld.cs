using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.UI;
using ZM.ZMAsset;

namespace ZMGC.Hall
{
    public class HallWorld : World
    {
        public static UserDataMgr UserData;
        
        public override void OnCreate()
        {
            base.OnCreate();
            UserData = GetExitsDataMgr<UserDataMgr>();
            Debug.Log("HallWorld OnCreate");
            NetWorkManager.Instance.ConnectSocket();
            UIModule.Instance.PopUpWindow<LoginWindow>();
        }

        /// <summary>
        /// 从登录界面进入大厅
        /// </summary>
        public static void EnterHallWorldFormLogin()
        {
            // 销毁登录界面
            UIModule.Instance.DestroyAllWindow();
            // 清理资源
            ZMAsset.ClearResourcesAssets(false);
            // 弹出大厅界面
            UIModule.Instance.PopUpWindow<HallWindow>();
            UIModule.Instance.PopUpWindow<HallButtonsWidow>();
        }
        
        /// <summary>
        /// 从游戏界面进入大厅
        /// </summary>
        /// <param name="worldEnum"></param>
        public static void EnterHallWorldFormGame(WorldEnum worldEnum)
        {
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
        }
    }
}