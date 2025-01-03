﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonAutoMono<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                _instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            } 
            return _instance;
        }
    }
}
