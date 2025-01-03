using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//同一个对象只能挂载一个
[DisallowMultipleComponent]
public abstract class SingletonMono<T> : MonoBehaviour where T:MonoBehaviour
{

    private static T _instance;

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this as T;
        DontDestroyOnLoad(this.gameObject);
    }
}
