using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.UI;
using ZM.ZMAsset;

public class GameMain : MonoBehaviour
{
    public void Awake()
    {
        //ZMUI，ZMAsset，ZMGC
        ZMAsset.InitFrameWork();
        
        StartGame();
    }
    
    public void StartGame()
    {
        UIModule.Instance.Initialize();

        UIModule.Instance.PopUpWindow<LoginWindow>();
    }
    
    public void Update()
    {
    }
    
    
    public void OnDestroy()
    {
    }
    
    public void OnApplicationQuit()
    {
    }
}
