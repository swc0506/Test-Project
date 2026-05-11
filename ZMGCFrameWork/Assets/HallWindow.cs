using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZMGC.Hall;
public class HallWindow : MonoBehaviour
{

    private HallLoigcCtrl mHallLogic;
    void Start()
    {
        mHallLogic = HallWorld.GetExitsLogicCtrl<HallLoigcCtrl>();
    }

 
    void Update()
    {
        
    }
 
     
    public void BattleButtonClick()
    {
        UIManager.Instance.hallWindow.SetActive(false);
        mHallLogic.EnterBattleWorld();
    }
    public void PokerButtonClick()
    {

    }

}
