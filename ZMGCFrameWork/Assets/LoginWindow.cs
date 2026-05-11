using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZMGC.Hall;
public class LoginWindow : MonoBehaviour
{
    public InputField accountInput;
    public InputField passInput;
    private LoginLogicCtrl mLoginLogic;
    void Start()
    {
        mLoginLogic=HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>();

    }

    public void OnEnable()
    {
        UIEventControl.AddEvent(UIEventEnum.LoginSuccess, OnLoginSuccess);
    }
    public void OnDisable()
    {
        UIEventControl.RemoveEvent(UIEventEnum.LoginSuccess, OnLoginSuccess);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnLoginSuccess(object data)
    {
        Debug.Log("LoginWindow OnLoginSuccess");
        UIManager.Instance.loginWindow.SetActive(false);
        UIManager.Instance.hallWindow.SetActive(true);
    }
     
    public void LoginButtonClick()
    {
        int result= mLoginLogic.AccountLogin(accountInput.text, passInput.text);
        if (result==1)
        {
            Debug.Log("账号不符合规范！");
        }
        else if (result == 2)
        {
            Debug.Log("密码不符合规范！");
        }
    }

}
