/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/8/6 15:16:03
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，再次生成后会以代码追加的形式新增,若手动修改后,尽量避免自动生成
---------------------------------*/

using System;
using DG.Tweening;
using LogicLayer;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
    public enum BattleCardState
    {
        Normal,
        Select,
    }
    
    public class BattleCardItem : MonoBehaviour
    {
        #region 自定义字段

        public Transform RootTransform;

        public Button CardButton;

        public Image bgImage;

        public Image iconImage;

        public Image SliderImage;

        public Transform EffectParentTransform;

        public GameObject MaskGameObject;

        private HeroData mHeroData;

        private GameObject mEffectObj;

        #endregion


        #region 生命周期

        //脚本初始化接口 (为保证生命周期的执行顺序，请在View层调用该接口确保需要初始化的数据正常执行)
        public void OnInitialize()
        {
            //按钮事件自动注册绑定
            CardButton.onClick.AddListener(OnCardButtonClick);
            UIEventControl.AddEvent(UIEventEnum.AngerChange, OnAngerChange);
            UIEventControl.AddEvent(UIEventEnum.ReleaseSkill, OnReleaseSkill);
            UIEventControl.AddEvent(UIEventEnum.HeroDeath, OnHeroDeath);
        }

        //物体设置数据接口 (请自定以你的参数，方便外部调用传参)
        public void SetItemData(HeroData heroData)
        {
            mHeroData = heroData;
            int quality = (int)heroData.quality;
            bgImage.sprite = ZMAsset.ZMAsset.LoadSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}Card/Cardbg{quality}");
            iconImage.sprite =
                ZMAsset.ZMAsset.LoadSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}HeroIcon/X1_card_{heroData.name}");
            // 关闭卡牌按钮交互
            CardButton.interactable = false;
        }

        //物体销毁时执行 (为保证生命周期的执行顺序，请在View层调用该接口确保需要释放时的接口正常调用)
        public void OnDispose()
        {
            UIEventControl.RemoveEvent(UIEventEnum.AngerChange, OnAngerChange);
            UIEventControl.RemoveEvent(UIEventEnum.ReleaseSkill, OnReleaseSkill);
            UIEventControl.RemoveEvent(UIEventEnum.HeroDeath, OnHeroDeath);
            ReleaseObject();
        }

        #endregion

        /// <summary>
        /// 英雄怒气值变化
        /// </summary>
        /// <param name="obj"></param>
        private void OnAngerChange(object obj)
        {
            HeroLogic logic = (HeroLogic)obj;
            if (logic.Id != mHeroData.id) return;
            
            float rate = logic.Rage.RawFloat / logic.MaxRage.RawFloat;
            SliderImage.DOFillAmount(rate, 0.4f);

            if (rate >= 1.0f && mEffectObj == null)
            {
                ReleaseObject();
                CardButton.interactable = true;
                mEffectObj = ZMAsset.ZMAsset.InstantiateObject($"{AssetsPathConfig.BATTLE_EFFECTS_PATH}Effect_OutChange", EffectParentTransform);
            }
        }

        private void OnReleaseSkill(object obj)
        {
            HeroData heroData = (HeroData)obj;
            if (heroData.id != mHeroData.id)
                return;
            SwitchCardStatus(BattleCardState.Normal);
        }

        private void OnHeroDeath(object obj)
        {
            HeroData heroData = (HeroData)obj;
            if (heroData.id != mHeroData.id)
                return;
            
            SwitchCardStatus(BattleCardState.Normal);
            CardButton.interactable = false;
            MaskGameObject.SetVisible(true);
            SliderImage.fillAmount = 0;
        }

        private void SwitchCardStatus(BattleCardState status)
        {
            switch (status)
            {
                case BattleCardState.Normal:
                    RootTransform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBounce);
                    ReleaseObject();
                    break;
                case BattleCardState.Select:
                    CardButton.interactable = false;
                    RootTransform.DOLocalMoveY(40, 0.5f).SetEase(Ease.OutBounce);
                    break;
            }
        }

        private void ReleaseObject()
        {
            if (mEffectObj != null)
            {
                ZMAsset.ZMAsset.Release(mEffectObj);
                mEffectObj = null;
            }
        }


        #region UI组件事件

        private void OnCardButtonClick()
        {
            // 释放技能操作
            BattleWorldManager.BattleWorld.heroLogicCtrl.InputReleaseSkillOperate(mHeroData.id);
            SwitchCardStatus(BattleCardState.Select);
        }

        #endregion
    }
}