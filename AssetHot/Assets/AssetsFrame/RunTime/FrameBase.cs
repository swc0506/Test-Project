using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public class FrameBase : MonoBehaviour
    {
        protected static AssetsFrame instance = null;
        protected static bool isQuitting = false;

        public static AssetsFrame Instance
        {
            get
            {
                if (isQuitting) return null;

                if (instance == null)
                {
                    instance = Object.FindObjectOfType<AssetsFrame>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("AssetsFrame");
                        DontDestroyOnLoad(obj);
                        instance = obj.AddComponent<AssetsFrame>();
                        instance.OnInit();
                    }
                }

                return instance;
            }
        }
        
        protected virtual void OnInit()
        {

        }

        protected void OnApplicationQuit()
        {
            isQuitting = true;
        }

        public static bool IsQuitting => isQuitting;

        protected void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}