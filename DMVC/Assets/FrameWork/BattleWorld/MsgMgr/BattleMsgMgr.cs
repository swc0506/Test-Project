using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Battle
{
    public class BattleMsgMgr : IMsgBehaviour
    {
        public void OnCreat()
        {
            Debug.Log("BattleMsgMgr OnCreat");
        }

        public void OnDestroy()
        {
            Debug.Log("BattleMsgMgr OnDestroy");
        }
    }
}