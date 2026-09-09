/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/8/6 15:19:48
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using System.Collections.Generic;
using DG.Tweening;
using LogicLayer;
using UnityEngine.UI;
using UnityEngine;

namespace ZM.UI
{
    public class RoundWindow : WindowBase
    {
        public RoundWindowDataComponent dataCompt;
        
        public GameObject roundStartAnim;
        private int maxRoundId = 15;

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            base.Update = true;
            dataCompt = gameObject.GetComponent<RoundWindowDataComponent>();
            dataCompt.InitComponent(this);
            base.OnAwake();
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
            UIEventControl.AddEvent(UIEventEnum.RoundStart, RoundStart);
            UIEventControl.AddEvent(UIEventEnum.NextRound, NextRound);
        }

        public override void OnUpdate()
        {
            UpdateLogicFrameCount();
        }

        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
            UIEventControl.RemoveEvent(UIEventEnum.RoundStart, RoundStart);
            UIEventControl.RemoveEvent(UIEventEnum.NextRound, NextRound);
            
            foreach (var cardItem in dataCompt.RootBattleCardItemArray)
            {
                cardItem.OnDispose();
            }
        }

        //物体销毁时执行
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region API Function

        public void InitViewState(List<HeroData> heroList)
        {
            for (var index = 0; index < dataCompt.RootBattleCardItemArray.Length; index++)
            {
                var cardItem = dataCompt.RootBattleCardItemArray[index];
                cardItem.OnInitialize();
                cardItem.SetItemData(heroList[index]);
            }
        }
        
        public void UpdateLogicFrameCount()
        {
            dataCompt.LogicFrameText.text = $"LogicFrame:{LogicFrameSyncConfig.logicFrameId}";
        }

        public void RoundStart(object obj)
        {
            maxRoundId = BattleWorldManager.BattleWorld.roundLogicCtrl.MaxRoundId;
            int roundId = BattleWorldManager.BattleWorld.roundLogicCtrl.RoundId;
            dataCompt.RoundText.text = Mathf.Clamp(roundId, 1, maxRoundId) + "/" + maxRoundId;
        }
        
        public void NextRound(object obj)
        {
            int roundId = BattleWorldManager.BattleWorld.roundLogicCtrl.RoundId;
            dataCompt.RoundText.text = Mathf.Clamp(roundId, 1, maxRoundId) + "/" + maxRoundId;
        }

        #endregion

        #region UI组件事件

        public void OnQuickenButtonClick()
        {
            LogicLayer.BattleWorldManager.BattleWorld.QuickenBattle();
            dataCompt.scaleImage.sprite = ZMAsset.ZMAsset.LoadSprite(
                $"{AssetsPathConfig.BATTLE_TEXTURE_PATH}Battle/x{BattleWorldManager.BattleWorld.quickenMultiple}");
            //quickenText.text = "x" + LogicLayer.BattleWorldManager.BattleWorld.quickenMultiple;
        }

        public void OnPauseButtonClick()
        {
            bool isPause = LogicLayer.BattleWorldManager.BattleWorld.BattlePause();
            
            dataCompt.PauseText.text = isPause ? "继续" : "暂停";
        }

        public void OnJumpButtonClick()
        {
            MsgHandleCenter.Instance.SendBattleResultRequest(LogicLayer.BattleWorldManager.BattleWorld.battleId,
                BattleWorldManager.BattleWorld.heroLogicCtrl.skillInputLogicFrameList);
        }

        public void OnAutoButtonClick()
        {
        }

        #endregion
    }
}