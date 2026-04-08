using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Hall
{
    public class UserData
    {
        public int id;
        public string name;
        public int gold;
    }
    
    public class UserDataMgr : IDataBehaviour
    {
        private UserData mUserData;
        
        public void OnCreat()
        {
            Debug.Log("UserDataMgr OnCreat");
        }

        public void OnDestroy()
        {
            Debug.Log("UserDataMgr OnDestroy");
        }
        
        public void CacheUserData(UserData userData)
        {
            mUserData = userData;
        }
    }
}