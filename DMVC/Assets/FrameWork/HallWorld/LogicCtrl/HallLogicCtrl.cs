using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Hall
{
    public class HallLogicCtrl : ILogicBehaviour
    {
        public void OnCreat()
        {
            Debug.Log("HallLogicCtrl OnCreat");
        }

        public void OnDestroy()
        {
            Debug.Log("HallLogicCtrl OnDestroy");
        }

        public void Test()
        {
            Debug.Log("HallLogicCtrl Test");
        }
    }
}