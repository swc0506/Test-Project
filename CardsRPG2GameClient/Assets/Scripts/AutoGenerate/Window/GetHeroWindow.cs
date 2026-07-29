/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/7/29 11:34:32
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using ZMGC.Hall;

namespace ZM.UI
{
    public class GetHeroWindow : WindowBase
    {
        public GetHeroWindowDataComponent dataCompt;
        private List<int> recruitHeroList = new List<int>();
        private int curHeroIndex = 0;
        
        // 英雄立绘对象
        private GameObject portraitObj;
        private bool playAnim;

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            FullScreenWindow = true;
            mDisableAnim = true;
            dataCompt = gameObject.GetComponent<GetHeroWindowDataComponent>();
            dataCompt.InitComponent(this);
            base.OnAwake();
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
            curHeroIndex = 0;
            recruitHeroList = HallWorld.GetExitsDataMgr<RecruitDataMgr>().RecruitHeroList;
            PlayGetHeroAnim();
        }

        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
            curHeroIndex = 0;
            ReleasePortraitObj();
        }

        //物体销毁时执行
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region API Function

        private async void PlayGetHeroAnim()
        {
            ReleasePortraitObj();
            int heroId = recruitHeroList[curHeroIndex];
            HeroData dataCfg = ConfigCenter.GetHeroData(heroId);
            portraitObj = ZMAsset.ZMAsset.InstantiateObject($"{AssetsPathConfig.HALL_PREFABS_PATH}Portrait2D/lihui_{dataCfg.name}",
                dataCompt.PortraitParentTransform);
            ReActiveEffect(dataCfg.quality);
            
            // 设置品质颜色
            dataCompt.ColorChangeEffectColorList.SetColors((int)dataCfg.quality);
            dataCompt.CoreGameObject.SetVisible(dataCfg.quality == QualityEnum.Red);
            dataCompt.HeroNameText.text = dataCfg.nameChinese;
            dataCompt.UIAnimator.SetBool("play", false);
            curHeroIndex++;
            
            await UniTask.Delay(1500);
            playAnim = false;
        }

        private void PlayOutAnim()
        {
            if (curHeroIndex >= recruitHeroList.Count)
            {
                PopUpWindow<TenRecruitWindow>();
                HideWindow();
                return;
            }

            dataCompt.UIAnimator.SetBool("play", true);
            dataCompt.CoreGameObject.SetVisible(false);
            portraitObj.transform.DOLocalMoveX(-2000, 0.8f).OnComplete(PlayGetHeroAnim);
        }
        
        private void ReleasePortraitObj()
        {
            if (portraitObj != null)
            {
                ZMAsset.ZMAsset.Release(portraitObj);
                portraitObj = null;
            }
        }
        
        private void ReActiveEffect(QualityEnum quality)
        {
            for (int i = 0; i < dataCompt.EffectTransform.childCount; i++)
            {
                dataCompt.EffectTransform.GetChild(i).gameObject.SetActive(false);
            }
            
            dataCompt.EffectTransform.Find(quality.ToString()).gameObject.SetActive(true);
        }
        
        #endregion

        #region UI组件事件

        public void OnNextButtonClick()
        {
            if (playAnim)
                return;
            
            playAnim = true;
            PlayOutAnim();
        }

        #endregion
    }
}