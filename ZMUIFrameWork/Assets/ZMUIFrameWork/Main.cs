using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZMUIFrameWork.Scripts.Runtime.Core;
using ZMUIFrameWork.Scripts.Window;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        UIModule.Instance.Initialize();
    }

    void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIModule.Instance.PopUpWindow<LoginWindow>();
        }
    }
}
