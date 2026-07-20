using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWindow : MonoBehaviour
{
    public void OnLoginButtonClick()
    {
        MsgHandleCenter.Instance.SendLoginRequest();
    }
}
