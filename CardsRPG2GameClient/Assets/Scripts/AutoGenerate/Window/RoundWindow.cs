/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/8/6 15:19:48
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using DG.Tweening;
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
        }

        public override void OnUpdate()
        {
            UpdateLogicFrameCount();
        }

        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
        }

        //物体销毁时执行
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region API Function

        public void UpdateLogicFrameCount()
        {
            dataCompt.LogicFrameText.text = $"LogicFrame:{LogicFrameSyncConfig.logicFrameId}";
        }

        public void RoundStart(int roundId)
        {
            // roundStartAnim.SetActive(true);
            // gameObject.SetActive(true);
            // roundStartAnim.transform.DOScale(1, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
            // {
            //     roundStartAnim.transform.DOScale(0, 0f).SetDelay(0.6f);
            // });
            // dataCompt.RoundText.text = roundId + "/" + maxRoundId;
        }
        
        public void NextRound(int roundId)
        {
            dataCompt.RoundText.text = roundId + "/" + maxRoundId;
        }

        #endregion

        #region UI组件事件

        public void OnQuickenButtonClick()
        {
            LogicLayer.BattleWorldManager.BattleWorld.QuickenBattle();
            //quickenText.text = "x" + LogicLayer.BattleWorldManager.BattleWorld.quickenMultiple;
        }

        public void OnPauseButtonClick()
        {
            LogicLayer.BattleWorldManager.BattleWorld.BattlePause();
        }

        public void OnJumpButtonClick()
        {
            MsgHandleCenter.Instance.SendBattleResultRequest(LogicLayer.BattleWorldManager.BattleWorld.battleId);
        }

        public void OnAutoButtonClick()
        {
        }

        #endregion
    }
}