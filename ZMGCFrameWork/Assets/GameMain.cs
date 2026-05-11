using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZMGC.Battle;
using ZMGC.Hall;

public class GameMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        WorldManager.CreateWorld<HallWorld>();
        HallWorld.GetExitsLogicCtrl<HallLoigcCtrl>().Test();
        HallWorld.GetExitsDataMgr<TaskDataMgr>().Test();
        HallWorld.GetExitsMsgMgr<TaskMsgMgr>().Test();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            WorldManager.CreateWorld<BattleWorld>();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            WorldManager.DestroyWorld<BattleWorld>();
        }
    }
    private void OnApplicationQuit()
    {
        WorldManager.DestroyWorld<HallWorld>();
    }
}
