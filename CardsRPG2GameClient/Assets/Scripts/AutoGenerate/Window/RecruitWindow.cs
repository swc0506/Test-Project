/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/7/27 18:13:11
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using ZMGC.Hall;

namespace ZM.UI
{
    public class RecruitWindow : WindowBase
    {
        public RecruitWindowDataComponent dataCompt;
        
        private GameObject recruitObj;
        private Vector3 origPos = Vector3.zero;
        private RecruitLogicCtrl logicCtrl;

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            base.OnAwake();
            logicCtrl = HallWorld.GetExitsLogicCtrl<RecruitLogicCtrl>();
            FullScreenWindow = true;
            mDisableAnim = true;
            dataCompt = gameObject.GetComponent<RecruitWindowDataComponent>();
            dataCompt.InitComponent(this);
            origPos = dataCompt.choukalihuiSkeletonGraphic.transform.localPosition;
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
            recruitObj = ZMAsset.ZMAsset.InstantiateObject(AssetPathConfig.HALL_EFFECT_PATH + "Recruit/ChoukaUIScene", null);
            dataCompt.particleGameObject.SetActive(true);
        }

        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
            dataCompt.particleGameObject.SetActive(false);
            if (recruitObj != null)
            {
                ZMAsset.ZMAsset.Release(recruitObj);
                recruitObj = null;
            }
        }

        //物体销毁时执行
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region API Function

        private async void PlayRecruitAnimation()
        {
            int result = logicCtrl.RecruitSingle(false);
            if (result != 0)
            {
                switch (result)
                {
                    case 1: ToastManager.ShowToast("钻石不足");
                        break;
                }
                return;
            }
            
            dataCompt.MaskGameObject.SetVisible(true);
            dataCompt.choukalihuiSkeletonGraphic.AnimationState.SetAnimation(0, "idle1", false);
            recruitObj.GetComponent<Animator>().SetBool("play", true);
            await UniTask.Delay(3000);
            dataCompt.choukalihuiSkeletonGraphic.GetComponent<Animation>().Play();
            dataCompt.DownHorizationiGameObject.SetVisible(false);
            
            await UniTask.Delay(3600);
            dataCompt.MaskGameObject.SetVisible(false);
            dataCompt.DownHorizationiGameObject.SetVisible(true);
            recruitObj.GetComponent<Animator>().SetBool("play", false);
            dataCompt.choukalihuiSkeletonGraphic.transform.localPosition = origPos;
            dataCompt.choukalihuiSkeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
            
            bool isJump = dataCompt.chooseGameObject.transform.localScale.x > 0;
            if (isJump)
            {
                PopUpWindow<TenRecruitWindow>();
            }
            else
            {
                PopUpWindow<GetHeroWindow>();
            }
        }
        
        #endregion

        #region UI组件事件

        public void OnNormalButtonClick()
        {
            PlayRecruitAnimation();
        }

        public void OnFirendButtonClick()
        {
            PlayRecruitAnimation();
        }

        public void OnSeniorButtonClick()
        {
            PlayRecruitAnimation();
        }

        public void OnHelpButtonClick()
        {
        }

        public void OnCloseButtonClick()
        {
            HideWindow();
        }

        public void OnJumpButtonClick()
        {
            dataCompt.chooseGameObject.SetVisible(dataCompt.chooseGameObject.transform.localScale.x == 0);
        }

        #endregion
    }
}