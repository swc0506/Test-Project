using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Battle
{
    public class BattleDataMgr : IDataBehaviour
    {
        public void OnCreat()
        {
            Debug.Log("BattleDataMgr OnCreat");
        }

        public void OnDestroy()
        {
            Debug.Log("BattleDataMgr OnDestroy");
        }
    }
}