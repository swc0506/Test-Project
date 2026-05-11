using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZMGC.Battle;
public class BattleWindow : MonoBehaviour
{

    private BattleLogicCtrl mBattleLogic;
    void Start()
    {
        mBattleLogic = BattleWorld.GetExitsLogicCtrl<BattleLogicCtrl>();

        //ZMGC.Hall.HallWorld.GetExitsLogicCtrl<ZMGC.Hall.HallLoigcCtrl>();
    }

 
    void Update()
    {
        
    }
 
     
    public void CloseButtonClick()
    {
        UIManager.Instance.battleWindow.SetActive(false);
        mBattleLogic.ExitbattleGame();
    }
    public void PokerButtonClick()
    {

    }

}
