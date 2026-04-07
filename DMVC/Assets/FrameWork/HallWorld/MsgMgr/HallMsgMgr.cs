using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Hall
{
    public class HallMsgMgr : IMsgBehaviour
    {
        public void OnCreat()
        {
            Debug.Log("HallMsgMgr OnCreat");
        }

        public void OnDestroy()
        {
            Debug.Log("HallMsgMgr OnDestroy");
        }
    }
}