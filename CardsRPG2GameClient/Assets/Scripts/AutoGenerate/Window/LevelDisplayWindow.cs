/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/7/31 10:39:09
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using UnityEngine.UI;
using UnityEngine;

namespace ZM.UI
{
    public class LevelDisplayWindow : WindowBase
    {
        public LevelDisplayWindowDataComponent dataCompt;
        
        private LevelData levelData;

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            dataCompt = gameObject.GetComponent<LevelDisplayWindowDataComponent>();
            dataCompt.InitComponent(this);
            base.OnAwake();
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
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

        public void Init(LevelData data)
        {
            levelData = data;
            dataCompt.LevelTitleText.text = $"第{levelData.levelID}关";
            for (int i = 0; i < levelData.enemys.Count; i++)
            {
                dataCompt.RootEnemyHeadItemArray[i].SetItemData(levelData.enemys[i]);
            }
        }
        
        #endregion

        #region UI组件事件

        public void OnFightButtonClick()
        {
            PopUpWindow<ChoosFormationWindow>().Init(levelData.enemys);
        }

        public void OnRePlayButtonClick()
        {
        }

        public void OnCloseButtonClick()
        {
            HideWindow();
        }

        #endregion
    }
}