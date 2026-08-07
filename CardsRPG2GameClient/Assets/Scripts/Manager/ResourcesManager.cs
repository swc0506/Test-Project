using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.ZMAsset;

public class ResourcesManager : Singleton<ResourcesManager>
{
    public GameObject LoadObject(string path, Transform parent, bool restPos = false, bool restScale = false,
        bool restRot = false)
    {
        GameObject obj = ZMAsset.InstantiateObject(path, parent);
        if (restPos)
        {
            obj.transform.localPosition = Vector3.zero;
        }
        if (restScale)
        {
            obj.transform.localScale = Vector3.one;
        }
        if (restRot)
        {
            obj.transform.localRotation = Quaternion.identity;
        }
        return obj;
    }

    /// <summary>
    /// 加载对象并获取对象身上的组件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T LoadObject<T>(string path, Transform parent = null)
    {
        //GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        T t = ZMAsset.InstantiateObject(path, parent).GetComponent<T>();
        return t;
    }
    
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T LoadAsset<T>(string path) where T : Object
    { 
        return Resources.Load<T>(path);   
    }
}
