/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2024/6/29 20:06:56
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using ZMGC.Hall;

public class LoginWindow : WindowBase
{

    public LoginWindowDataComponent dataCompt;

    #region 声明周期函数
    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<LoginWindowDataComponent>();
        dataCompt.InitComponent(this);
        base.OnAwake();
   
    }
    //物体显示时执行
    public override void OnShow()
    {
        base.OnShow();
        //窗口是通过CanvasGroup =0 只是单纯隐藏窗口，
        UIEventControl.AddEvent(UIEventEnum.LoginSuccess, OnLoginSuccess);
    }
    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
        UIEventControl.RemoveEvent(UIEventEnum.LoginSuccess, OnLoginSuccess);
    }
    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
       
    }
    #endregion
    #region API Function
    private void OnLoginSuccess(object data)
    {
        //HideWindow();
        UIModule.Instance.PopUpWindow<SelectRoleWindow>();
    }
    #endregion
    #region UI组件事件
    public void OnCloseButtonClick()
    {

        HideWindow();
    }
    public void OnServerSelectButtonClick()
    {

    }
    public void OnAccountInputChange(string text)
    {

    }
    public void OnAccountInputEnd(string text)
    {

    }
    public void OnPasswordInputChange(string text)
    {

    }
    public void OnPasswordInputEnd(string text)
    {

    }
    public void OnEnterGameButtonClick()
    {
        PopUpWindow<ReConnectWindow>();
        HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>().GetToken(dataCompt.AccountInputField.text, dataCompt.PasswordInputField.text, (result) => {
            UIModule.Instance.HideWindow<ReConnectWindow>();
        });
    }
    public void OnPublishButtonClick()
    {

    }
    public void OnClickEnterButtonClick()
    {

    }
    public void OnRegisterAccountButtonClick()
    {
        PopUpWindow<RegisterWindow>();
    }
    #endregion
}
