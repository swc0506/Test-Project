using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public abstract class ResInfoBase{}

public class ResInfo<T> : ResInfoBase
{
    public T asset;

    public UnityAction<T> callBack;

    //是否需要删除
    public bool isDel;

    //用于存储异步加载对象
    public Coroutine coroutine;
}

public class ResMgr : BaseManager<ResMgr>
{
    private Dictionary<string, ResInfoBase> _resDic = new Dictionary<string, ResInfoBase>();
    
    private ResMgr(){}

    public T Load<T>(string path) where T : Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info;
        if (_resDic.ContainsKey(resName))
        {
            info = new ResInfo<T>();
            info.asset = Resources.Load<T>(path);
            _resDic.Add(resName, info);
            return info.asset;
        }
        else
        {
            info = _resDic[resName] as ResInfo<T>;
            //异步加载中
            if (info.asset == null)
            {
                //停止异步，直接同步
                MonoMgr.Instance.StopCoroutine(info.coroutine);
                info.asset = Resources.Load<T>(path);
                info.callBack?.Invoke(info.asset);

                info.callBack = null;
                info.coroutine = null;
                return info.asset;
            }
            else
            {
                return info.asset;
            }
        }
    }
    
    public void LoadAsync<T>(string path, UnityAction<T> callBack) where T : Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info;
        if (!_resDic.ContainsKey(resName))
        {
            info = new ResInfo<T>();
            _resDic.Add(resName, info);
            info.callBack += callBack;

            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            info = _resDic[resName] as ResInfo<T>;
            if (info.asset == null)
                info.callBack += callBack;
            else
                callBack?.Invoke(info.asset);
        }

        //MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path, callBack));
    }

    private IEnumerator ReallyLoadAsync<T>(string path) where T : Object
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        yield return rq;

        string resName = path + "_" + typeof(T).Name;
        if (_resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = _resDic[resName] as ResInfo<T>;
            resInfo.asset = rq.asset as T;

            //如果发现需要删除 移去资源
            if (resInfo.isDel)
            {
                UnloadAsset<T>(path);
            }
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }
    
    [Obsolete("注意：建议使用泛型加载")]
    public void LoadAsync(string path, Type type, UnityAction<Object> callBack)
    {
        string resName = path + "_" + type.Name;
        ResInfo<Object> info;
        if (!_resDic.ContainsKey(resName))
        {
            info = new ResInfo<Object>();
            _resDic.Add(resName, info);
            info.callBack += callBack;

            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, type));
        }
        else
        {
            info = _resDic[resName] as ResInfo<Object>;
            if (info.asset == null)
                info.callBack += callBack;
            else
                callBack?.Invoke(info.asset);
        }
    }

    private IEnumerator ReallyLoadAsync(string path, Type type)
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync(path, type);
        yield return rq;

        string resName = path + "_" + type.Name;
        if (_resDic.ContainsKey(resName))
        {
            ResInfo<Object> resInfo = _resDic[resName] as ResInfo<Object>;
            resInfo.asset = rq.asset;

            if (resInfo.isDel)
            {
                UnloadAsset(path, type);
            }
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }

    //卸载指定资源
    public void UnloadAsset<T>(string path)
    {
        string resName = path + "_" + typeof(T).Name;
        if (_resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = _resDic[resName] as ResInfo<T>;
            //资源加载结束
            if (resInfo.asset != null)
            {
                _resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as Object);
            }
            else //异步加载中
            {
                resInfo.isDel = true;
            }
        }
    }
    
    public void UnloadAsset(string path, Type type)
    {
        string resName = path + "_" + type.Name;
        if (_resDic.ContainsKey(resName))
        {
            ResInfo<Object> resInfo = _resDic[resName] as ResInfo<Object>;
            //资源加载结束
            if (resInfo.asset != null)
            {
                _resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            else //异步加载中
            {
                resInfo.isDel = true;
            }
        }
    }

    public void UnloadUnusedAssets(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callBack));
    }

    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callBack)
    {
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;

        callBack();
    }
}
