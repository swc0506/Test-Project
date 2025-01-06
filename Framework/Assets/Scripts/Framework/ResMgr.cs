using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public abstract class ResInfoBase
{
    //引用计数
    public int refCount;
}

public class ResInfo<T> : ResInfoBase
{
    public T asset;

    public UnityAction<T> callBack;

    //引用计数为0时 是否需要删除
    public bool isDel;

    //用于存储异步加载对象
    public Coroutine coroutine;
    
    // //引用计数
    // public int refCount;

    public void AddRefCount()
    {
        ++refCount;
    }

    public void SubRefCount()
    {
        --refCount;
    }
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
            //计数
            info.AddRefCount();
            _resDic.Add(resName, info);
            return info.asset;
        }
        else
        {
            info = _resDic[resName] as ResInfo<T>;
            //计数
            info.AddRefCount();
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
            //计数
            info.AddRefCount();
            _resDic.Add(resName, info);
            info.callBack += callBack;

            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            info = _resDic[resName] as ResInfo<T>;
            //计数
            info.AddRefCount();
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
            if (resInfo.refCount <= 0)
            {
                UnloadAsset<T>(path, resInfo.isDel, null, false);
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
            //计数
            info.AddRefCount();
            _resDic.Add(resName, info);
            info.callBack += callBack;

            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, type));
        }
        else
        {
            info = _resDic[resName] as ResInfo<Object>;
            //计数
            info.AddRefCount();
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

            if (resInfo.refCount <= 0)
            {
                UnloadAsset(path, type, resInfo.isDel, null, false);
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
    public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callBack = null, bool isSub = true)
    {
        string resName = path + "_" + typeof(T).Name;
        if (_resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = _resDic[resName] as ResInfo<T>;
            //计数
            if(isSub)
                resInfo.SubRefCount();
            resInfo.isDel = isDel;
            //资源加载结束
            if (resInfo.asset != null && resInfo.refCount <= 0 && resInfo.isDel)
            {
                _resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as Object);
            }
            else if (resInfo.asset == null)//异步加载中
            {
                //resInfo.isDel = true;
                //删除异步调用的回调
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }
    }
    
    [Obsolete("注意：建议使用泛型加载")]
    public void UnloadAsset(string path, Type type, bool isDel = false, UnityAction<Object> callBack = null, bool isSub = true)
    {
        string resName = path + "_" + type.Name;
        if (_resDic.ContainsKey(resName))
        {
            ResInfo<Object> resInfo = _resDic[resName] as ResInfo<Object>;
            //计数
            if(isSub)
                resInfo.SubRefCount();
            resInfo.isDel = isDel;
            //资源加载结束
            if (resInfo.asset != null && resInfo.refCount <= 0 && resInfo.isDel)
            {
                _resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            else if (resInfo.asset == null)//异步加载中
            {
                //resInfo.isDel = true;
                //删除异步调用的回调
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }
    }

    public void UnloadUnusedAssets(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callBack));
    }

    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callBack)
    {
        List<string> list = new List<string>();
        foreach (string path in _resDic.Keys)
        {
            if (_resDic[path].refCount == 0)
            {
                list.Add(path);
            }
        }

        foreach (var path in list)
        {
            _resDic.Remove(path);
        }
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;

        callBack();
    }

    public int GetRefCount<T>(string path)
    {
        string resName = path + "_" + typeof(T).Name;
        if (_resDic.ContainsKey(resName))
        {
            return (_resDic[resName] as ResInfo<T>).refCount;
        }

        return 0;
    }

    public void ClearDic(UnityAction callBack)
    {
        _resDic.Clear();
        MonoMgr.Instance.StartCoroutine(ReallyClearDic(callBack));
    }

    private IEnumerator ReallyClearDic(UnityAction callBack)
    {
        _resDic.Clear();
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callBack();
    }
}
