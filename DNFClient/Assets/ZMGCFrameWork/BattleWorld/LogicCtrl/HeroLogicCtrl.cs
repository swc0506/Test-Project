/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/3/3 12:11:36
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using ZM.AssetFrameWork;
using UnityEngine;
using ZMGC.Hall;

namespace ZMGC.Battle
{
    /// <summary>
    /// 管理所有英雄逻辑
    /// </summary>
    public class HeroLogicCtrl : ILogicBehaviour
    {
        public int HeroId { get; private set; }
        public HeroLogic HeroLogic { get; private set; }

        public void OnCreate()
        {
            UnityEngine.Debug.Log("HeroLogicCtrl OnCreate");
        }

        /// <summary>
        /// 初始化场景中的英雄
        /// </summary>
        public void InitHero()
        {
            //在游戏内或者其他游戏世界是可以访问大厅世界的。但是在大厅世界是不允许访问其他游戏时间
            //原因：大厅作为常驻世界，保存着很多用户相关的数据，大厅世界是不允许销毁的，可以通过大厅世界获取用户的信息
            HeroId = HallWorld.GetExitsDataMgr<UserDataMgr>().RoleId;
            Debug.Log("UserName:" + HallWorld.GetExitsDataMgr<UserDataMgr>().userName);
            GameObject heroObj = ZMAssetsFrame.Instantiate(AssetPathConfig.GAME_PREFABS_HERO + HeroId, null);
            //获取英雄渲染层
            HeroRender heroRender = heroObj.GetComponent<HeroRender>();
            HeroLogic heroLogic = new HeroLogic(HeroId, heroRender);
            HeroLogic = heroLogic;
            heroRender.SetLoigcObject(heroLogic);
            //初始化英雄渲染层和逻辑层
            heroLogic.OnCreate();
            heroRender.OnCreate();
            GameObject.Find("Main Camera").GetComponent<CameraFollow>().target = heroObj.transform;
        }

        public void OnLogicFrameUpdate()
        {
            HeroLogic.OnLogicFrameUpdate();
        }

        public void OnDestroy()
        {
        }
    }
}