/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/7/29 20:41:09
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using ZMGC.Hall;

namespace ZM.UI
{
    public class TenRecruitWindow : WindowBase
    {
        public TenRecruitWindowDataComponent dataCompt;

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            FullScreenWindow = true;
            mDisableAnim = true;
            dataCompt = gameObject.GetComponent<TenRecruitWindowDataComponent>();
            dataCompt.InitComponent(this);
            base.OnAwake();
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
            foreach (var item in dataCompt.ItemRootGetHeroCardItemArray)
            {
                item.OnInitialize();
            }

            dataCompt.choukaUI_jiesuanGameObject.SetActive(true);
            ShowHeroCardAnim();
        }

        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
            dataCompt.choukaUI_jiesuanGameObject.SetActive(false);
            foreach (var item in dataCompt.ItemRootGetHeroCardItemArray)
            {
                item.OnDispose();
            }
        }

        //物体销毁时执行
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region API Function

        private async void ShowHeroCardAnim()
        {
            dataCompt.MaskGameObject.SetVisible(true);
            var heroIdList = HallWorld.GetExitsDataMgr<RecruitDataMgr>().RecruitHeroList;
            for (int i = 0; i < dataCompt.ItemRootGetHeroCardItemArray.Length; i++)
            {
                GetHeroCardItem cardItem = dataCompt.ItemRootGetHeroCardItemArray[i];
                cardItem.SetItemData(heroIdList[i]);
                await UniTask.Delay(500);
            }
            dataCompt.MaskGameObject.SetVisible(false);
        }

        #endregion

        #region UI组件事件

        public void OnRetrunButtonClick()
        {
            HideWindow();
        }

        #endregion
    }
}