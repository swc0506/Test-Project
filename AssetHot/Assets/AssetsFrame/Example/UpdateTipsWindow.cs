using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTipsWindow : MonoBehaviour
{
    Action onUpdateCallBack;
    Action onCancelCallBack;
    public Text contentText;

    public void InitView(string content, Action updateCallBack, Action cancelCallBack)
    {
        onUpdateCallBack = updateCallBack;
        onCancelCallBack = cancelCallBack;
        contentText.text = content;
    }
    
    public void OnUpdateButtonClick()
    {
        onUpdateCallBack?.Invoke();
        Destroy(gameObject);
    }
    
    public void OnCancelButtonClick()
    {
        onCancelCallBack?.Invoke();
        Destroy(gameObject);
    }
}
