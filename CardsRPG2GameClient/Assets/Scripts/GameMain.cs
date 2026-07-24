using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.UI;
using ZM.ZMAsset;
using ZMGC.Hall;

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
        // 构建大厅事件
        WorldManager.CreateWorld<HallWorld>();
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
