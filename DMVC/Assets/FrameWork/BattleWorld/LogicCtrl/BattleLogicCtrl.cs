using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Battle
{
    public class BattleLogicCtrl : ILogicBehaviour
    {
        public void OnCreat()
        {
            Debug.Log("BattleLogicCtrl OnCreat");
        }

        public void OnDestroy()
        {
            Debug.Log("BattleLogicCtrl OnDestroy");
        }
    }
}