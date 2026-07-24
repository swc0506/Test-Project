using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LogicLayer;

public class BattleResultWindow : MonoBehaviour
{
    public Text resultText;
    
    public void SetBattleResult(bool isWin)
    {
        gameObject.SetActive(true);
        resultText.text = isWin ? "You Win" : "You Lose";
    }
    
    public void OnBackPlayButtonClick()
    {
        gameObject.SetActive(false);
        string json = PlayerPrefs.GetString(BattleDataModel.key);
        BattleDataModel battleData = Newtonsoft.Json.JsonConvert.DeserializeObject<BattleDataModel>(json);
        LogicLayer.BattleWorldManager.CreateBattleWorld(battleData.heroList, battleData.enemyList, battleData.battleSite, battleData.battleId);
        //1.本地回放
        //2.服务端回放
    }

    public void OnResetGameButtonClick()
    {
        gameObject.SetActive(false);
        MsgHandleCenter.Instance.SendStartBattleRequest(BattleDataMgr.heroSeatDataList);
        // //Test
        // List<HeroData> heroList = new List<HeroData>();
        // List<HeroData> enemyList = new List<HeroData>();
        // // 10个测试英雄
        // List<int> heroIdList = new List<int>{101, 102, 103, 104, 105, 501, 502, 503, 504, 505};
        // for (int i = 0; i < heroIdList.Count; i++)
        // {
        //     HeroData heroData = ConfigCenter.GetHeroData(heroIdList[i]);
        //     if (i < 5)
        //     { 
        //         heroData.seatId = i;
        //         heroList.Add(heroData);
        //     }
        //     else
        //     {
        //         heroData.seatId = i - 5;
        //         enemyList.Add(heroData);
        //     }
        // }
        
        //LogicLayer.WorldManager.CreateBattleWorld(heroList, enemyList);
        
    }
}
