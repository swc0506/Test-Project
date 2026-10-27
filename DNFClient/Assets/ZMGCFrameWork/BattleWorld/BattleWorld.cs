using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

namespace ZMGC.Battle
{
    public class BattleWorld : World
    {
        /// <summary>
        /// 逻辑帧累计运行时间
        /// </summary>
        private float mAccLogicRuntime;

        /// <summary>
        /// 下一个逻辑帧开始的时间
        /// </summary>
        private float mNextLogicFrameTime;

        /// <summary>
        /// 逻辑帧增量时间
        /// </summary>
        public float logicDeltaTime;

        /// <summary>
        /// 英雄控制器
        /// </summary>
        public HeroLogicCtrl HeroLogicCtrl { get; private set; }

        /// <summary>
        /// 怪物控制器
        /// </summary>
        public MonsterLogicCtrl MonsterLogicCtrl { get; private set; }

        /// <summary>
        /// 世界构建完成后执行
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            ConfigCenter.Instance.InitGameCfg();
            HeroLogicCtrl = BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>();
            MonsterLogicCtrl = BattleWorld.GetExitsLogicCtrl<MonsterLogicCtrl>();
            HeroLogicCtrl.InitHero();
            MonsterLogicCtrl.InitMonster();

            UIModule.PopUpWindow<BattleWindow>();
            Debug.Log("BattleWorld OnCretae");
            BuffSystem.Instance.OnCreate();
            AudioController.GetInstance().PlayMusicFade(AssetPathConfig.GAME_AUIDO_PATH + "BG/jizhou.mp3", 2);
        }

        /// <summary>
        /// Unity渲染帧更新 模拟逻辑帧更新
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();

            //逻辑帧累计运行时间累加
            mAccLogicRuntime += Time.deltaTime;

            //当前逻辑帧时间如果大于下一个逻辑帧时间，就需要更新逻辑帧
            //追帧操作。
            //控制帧数，保证所有设备逻辑帧帧数的一致性，并进行追帧操作。
            while (mAccLogicRuntime > mNextLogicFrameTime)
            {
                //更新逻辑帧
                OnLogicFrameUpdate();
                //计算下一个逻辑帧的运行时间
                mNextLogicFrameTime += LogicFrameConfig.LogicFrameInterval;
                //逻辑帧id进行自增
                LogicFrameConfig.LogicFrameId++;
            }

            //逻辑帧 1秒15帧 渲染帧1秒60帧
            //0-1 ---- L
            //mAccLogicRuntime 0.01 LogicFrameInterval 0.066 mNextLogicFrameTime 0.066 /0.066
            // 0.01+0.066-0.066 /0.066 =0.01 /0.066= 当前值/最大值=0-1 与血条的计算比例是一样的。
            // 0.05 +0.066-0.066 /0.066= 
            logicDeltaTime = (mAccLogicRuntime + LogicFrameConfig.LogicFrameInterval - mNextLogicFrameTime) /
                             LogicFrameConfig.LogicFrameInterval;
        }

        /// <summary>
        /// 逻辑帧更新（后期通过服务端进行调用）
        /// </summary>
        public void OnLogicFrameUpdate()
        {
            HeroLogicCtrl.OnLogicFrameUpdate();
            MonsterLogicCtrl.OnLogicFrameUpdate();
            LogicActionController.Instance.OnLogicFrameUpdate();
            BuffSystem.Instance.OnLogicFrameUpdate();
            LogicTimerManager.Instance.OnLogicFrameUpdate();
        }

        /// <summary>
        /// 世界销毁时执行
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
            LogicActionController.Instance.OnDestroy();
            BuffSystem.Instance.OnDestory();
        }

        /// <summary>
        /// 世界完全销毁后执行
        /// </summary>
        /// <param name="args"></param>
        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
        }
    }
}