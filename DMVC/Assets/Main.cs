using System.Collections;
using System.Collections.Generic;
using GC.Hall;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        WorldManager.CreateWorld<HallWorld>();
        HallWorld.GetExitsLogicCtrl<HallLogicCtrl>().Test();
    }

    void Update()
    {
        
    }
    
    private void OnApplicationQuit()
    {
        WorldManager.DestroyWorld<HallWorld>();
    }
}
