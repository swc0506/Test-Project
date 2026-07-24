/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/7/24 15:53:24
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using UnityEngine.UI;
using UnityEngine;

namespace ZM.UI
{
    public class CreateUserWindow : WindowBase
    {
        public CreateUserWindowDataComponent dataCompt;
        private Gender mGender;

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            mDisableAnim = true;
            dataCompt = gameObject.GetComponent<CreateUserWindowDataComponent>();
            dataCompt.InitComponent(this);
            base.OnAwake();
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
            SelectGender(Gender.Male);
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
        
        private void SelectGender(Gender gender)
        {
            dataCompt.femaleUncheckedGameObject.SetVisible(gender == Gender.Male);
            dataCompt.maleUncheckedGameObject.SetVisible(gender == Gender.Female);
            dataCompt.femaleSelectIconGameObject.SetVisible(gender == Gender.Female);
            dataCompt.maleSelectIconGameObject.SetVisible(gender == Gender.Male);
            dataCompt.femaleButton.transform.Find("check").SetVisible(gender == Gender.Female);
            dataCompt.maleButton.transform.Find("check").SetVisible(gender == Gender.Male);
            mGender = gender;
        }
        
        #endregion

        #region UI组件事件

        public void OnmaleButtonClick()
        {
            SelectGender(Gender.Male);
        }

        public void OnfemaleButtonClick()
        {
            SelectGender(Gender.Female);
        }

        public void OnnicknameInputChange(string text)
        {
        }

        public void OnnicknameInputEnd(string text)
        {
        }

        public void OnCreateButtonClick()
        {
        }

        #endregion
    }
}