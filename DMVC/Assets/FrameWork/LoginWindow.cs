using GC.Hall;
using TMPro;
using UnityEngine;

public class LoginWindow : MonoBehaviour
{
    public TMP_InputField accountInput;
    public TMP_InputField passInput;
    
    private LoginLogicCtrl mLoginLogic;
    
    void Start()
    {
        mLoginLogic = HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>();
    }
    
    void OnEnable()
    {
        UIEventControl.AddEvent(UIEventEnum.LoginSuccess, OnLoginSuccess);
    }

    void OnDisable()
    {
        UIEventControl.RemoveEvent(UIEventEnum.LoginSuccess, OnLoginSuccess);
    }

    void Update()
    {
        
    }

    public void OnLoginBtnClick()
    {
        int res = mLoginLogic.AccountLogin(accountInput.text, passInput.text);
        switch (res)
        {
            case 1:
                Debug.Log("账号长度不对");
                break;
            case 2:
                Debug.Log("密码长度不对");
                break;
            default:
                break;
        }
    }
    
    private void OnLoginSuccess(object data)
    {
        Debug.Log("OnLoginSuccess");
    }
}
