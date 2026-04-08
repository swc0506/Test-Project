using System.Collections;
using System.Collections.Generic;
using GC.Battle;
using GC.Hall;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Awake()
    {
        WorldManager.CreateWorld<HallWorld>();
        HallWorld.GetExitsLogicCtrl<HallLogicCtrl>().Test();
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     WorldManager.CreateWorld<BattleWorld>();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     WorldManager.DestroyWorld<BattleWorld>();
        // }
    }
    
    private void OnApplicationQuit()
    {
        WorldManager.DestroyWorld<HallWorld>();
    }
}
