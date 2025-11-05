/*------------------------------
 *Title:UI表现层脚本自动化代码生成工具
 *Author:SWC
 *Data:2025/11/5 14:04:40
 *Description: 表现层只负责页面逻辑与交互，不允许编写业务逻辑代码
 *注意:生成不会覆盖原有代码，会进行新增操作
------------------------------*/

using UnityEngine;
using UnityEngine.UI;
using ZMUIFrameWork;
using ZMUIFrameWork.Scripts.Runtime.Base;
using ZMUIFrameWork.Scripts.Runtime.Core;

namespace ZMUIFrameWork.Scripts.Window
{
    public class LoginWindow : WindowBase
    {
        public LoginWindowDataComponent dataComp;

        #region 声明周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            base.OnAwake();
            dataComp = GameObject.GetComponent<LoginWindowDataComponent>();
            dataComp.InitComponent(this);
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

        #endregion

        #region UI组件事件

        public void OnLoginButtonButtonClick()
        {
            UIModule.Instance.PopUpWindow<HallWindow>();
            HideWindow();
        }

        #endregion
    }
}