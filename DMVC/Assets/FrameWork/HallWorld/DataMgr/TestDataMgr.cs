using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Hall
{
    public class TestDataMgr : IDataBehaviour
    {
        public void OnCreat()
        {
            Debug.Log("TestDataMgr OnCreat");
        }

        public void OnDestroy()
        {
            Debug.Log("TestDataMgr OnDestroy");
        }
    }
}