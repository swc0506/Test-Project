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
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
using UnityEngine.Experimental.AI;

namespace ZMGC.Battle
{
    /// <summary>
    /// 管理所有英雄逻辑
    /// </summary>
    public class HeroLogicCtrl : ILogicBehaviour
    {
        public HeroLogic HeroLogic { get; private set; }
        /// <summary>
        /// 英雄数据层
        /// </summary>
        private HeroDataMgr mHeroDataLayer;
        /// <summary>
        /// 英雄逻辑列表
        /// </summary>
        public List<HeroLogic> heroLogicList=new List<HeroLogic>();
        public void OnCreate()
        {
            UnityEngine.Debug.Log("HeroLogicCtrl OnCreate");
            mHeroDataLayer = BattleWorld.GetExitsDataMgr<HeroDataMgr>();
        }
        /// <summary>
        /// 初始化场景中的英雄
        /// </summary>
        public void InitHero()
        {
            foreach (var hero in mHeroDataLayer.BattleRoleList)
            {
                //在游戏内或者其他游戏世界是可以访问大厅世界的。但是在大厅世界是不允许访问其他游戏时间
                //原因：大厅作为常驻世界，保存着很多用户相关的数据，大厅世界是不允许销毁的，可以通过大厅世界获取用户的信息

                GameObject heroObj = ZMAsset.Instantiate(AssetPathConfig.GAME_PREFABS_HERO + hero.roleid, null);
                //获取英雄渲染层
                HeroRender heroRender = heroObj.GetComponent<HeroRender>();
                HeroLogic heroLogic = new HeroLogic(hero.roleid, heroRender, hero.accountid);
                heroLogicList.Add(heroLogic);
               
                bool isLocalPlayer = HallWorld.UserData.AccountId == hero.accountid;
                heroRender.SetLoigcObject(heroLogic,  isLocalPlayer:isLocalPlayer);
                //初始化英雄渲染层和逻辑层
                heroLogic.OnCreate();
                heroRender.OnCreate();
                //只有本地的英雄才会触发相机的跟随
                if (isLocalPlayer)
                {
                    HeroLogic = heroLogic;
                    CameraFollow cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
                    cameraFollow.target = heroObj.transform;
                }
            }
            Debuger.Log("初始化英雄完成 heroCount:"+ heroLogicList.Count +" this:"+GetHashCode());
         }
        /// <summary>
        /// 获取英雄逻辑层，通过玩家id
        /// </summary>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public HeroLogic GetHeroLogicByPlayerid(long playerid)
        {
            foreach (var item in heroLogicList)
            {
                if (item.PlayerId== playerid)
                {
                    return item;
                }
            }
            foreach (var item in heroLogicList)
            {
                Debuger.Log("当前所有英雄:"+item.PlayerId);
            }
            Debuger.LogError("获取HeroLogic失败，没有找到对应的英雄 playerid："+playerid +"heroListCount:"+heroLogicList.Count + " this:" + GetHashCode());

            return null;
        }
        public void OnLogicFrameUpdate()
        {
            //HeroLogic.OnLogicFrameUpdate();
            for (int i = 0; i < heroLogicList.Count; i++)
            {
                heroLogicList[i].OnLogicFrameUpdate();
            }
      
        }
        public void OnDestroy()
        {
            foreach (var item in heroLogicList)
            {
                item.OnDestroy();
            }
            heroLogicList.Clear();
            HeroLogic = null;
            mHeroDataLayer= null;
        }

    }
}
