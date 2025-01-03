using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class BaseManager<T> where T:class//, new()
{
    private static T _instance;

    protected static readonly object lockObj = new object();
    
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (lockObj)
                {
                    if (_instance == null)
                    {
                        //_instance = new T();
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                            null, Type.EmptyTypes, null);

                        if(info != null)
                            _instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("无无参构造函数");
                    }
                }
            }
            return _instance;
        }
    }
}
