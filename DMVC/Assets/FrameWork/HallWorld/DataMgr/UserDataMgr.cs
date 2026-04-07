using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Hall
{
    public class UserDataMgr : IDataBehaviour
    {
        public void OnCreat()
        {
            Debug.Log("UserDataMgr OnCreat");
        }

        public void OnDestroy()
        {
            Debug.Log("UserDataMgr OnDestroy");
        }
    }
}