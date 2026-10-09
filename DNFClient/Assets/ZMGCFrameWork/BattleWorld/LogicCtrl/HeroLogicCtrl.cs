/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2026/9/15 19:13:28
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using UnityEngine;
using ZM.AssetFrameWork;

namespace ZMGC.Battle
{
    public class HeroLogicCtrl : ILogicBehaviour
    {
        public HeroLogic HeroLogic { get; private set; }

        public void OnCreate()
        {
        }

        public void OnDestroy()
        {
        }

        /// <summary>
        /// 初始化场景中的英雄
        /// </summary>
        public void InitHero()
        {
            GameObject heroObj = ZMAssetsFrame.Instantiate(AssetPathConfig.GAME_PREFABS_HERO + "1001", null);
            // //获取英雄渲染层
            // HeroRender heroRender = heroObj.GetComponent<HeroRender>();
            // HeroLogic heroLogic = new HeroLogic(1001, heroRender);
            // HeroLogic = heroLogic;
            // heroRender.SetLoigcObject(heroLogic);
            // //初始化英雄渲染层和逻辑层
            // heroLogic.OnCreate();
            // heroRender.OnCreate();
        }

        public void OnLogicFrameUpdate()
        {
            HeroLogic.OnLogicFrameUpdate();
        }
    }
}