using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoundWindow : MonoBehaviour
{
    public GameObject roundStartAnim;
    public Text roundText;
    public Text logicFrameText;
    public Text quickenText;
    private int maxRoundId = 15;

    public void RoundStart(int roundId)
    {
        roundStartAnim.SetActive(true);
        gameObject.SetActive(true);
        roundStartAnim.transform.DOScale(1, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            roundStartAnim.transform.DOScale(0, 0f).SetDelay(0.6f);
        });
        roundText.text = roundId + "/" + maxRoundId;
    }

    public void NextRound(int roundId)
    {
        roundText.text = roundId + "/" + maxRoundId;
    }

    public void Update()
    {
        UpdateLogicFrameCount();
    }

    public void UpdateLogicFrameCount()
    {
        logicFrameText.text = "LogicFrame:" + LogicFrameSyncConfig.logicFrameId;
    }

    public void OnGamePauseClick()
    {
        WorldManager.BattleWorld.BattlePause();
    }

    public void OnQuickenBattle()
    {
        WorldManager.BattleWorld.QuickenBattle();
        quickenText.text = "x" + WorldManager.BattleWorld.quickenMultiple;
    }
}
