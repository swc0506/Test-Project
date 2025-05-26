using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZMUIFrameWork.Scripts.RunTime.Base;

public class UIModule
{
    private static UIModule instance;
    private Camera mUICamera;
    private Transform mUIRoot;
    private Dictionary<string, WindowBase> mAllWindDic = new Dictionary<string, WindowBase>();//所有窗口字典
    private List<WindowBase> mAllWindowList = new List<WindowBase>();//所有窗口列表
    private List<WindowBase> mVisibleWindowList = new List<WindowBase>();//所有可见窗口

    public static UIModule Instance
    {
        get { return instance ??= new UIModule(); }
    }

    public void Initialize()
    {
        mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        mUIRoot = GameObject.Find("UIRoot").transform;
    }

    public T PopUpWindow<T>() where T : WindowBase, new()
    {
        Type type = typeof(T);
        string windName = type.Name;
        WindowBase wind = GetWindow(windName);
        if (wind != null)
        {
            return ShowWindow(windName) as T;
        }

        T t = new T();
        return InitializeWindow(t, windName) as T;
    }

    public WindowBase InitializeWindow(WindowBase windowBase, string windName)
    {
        //生成对应窗口预制体
        GameObject newWindow = TempLoadWindow(windName);
        //初始化对应管理类
        if (newWindow != null)
        {
            windowBase.GameObject = newWindow;
            windowBase.Transform = newWindow.transform;
            windowBase.Canvas = newWindow.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = mUICamera;
            windowBase.Transform.SetAsLastSibling();
            windowBase.OnAwake();
            windowBase.SetVisible(true);
            windowBase.OnShow();
            RectTransform rectTransform = newWindow.GetComponent<RectTransform>();
            rectTransform.anchorMax= Vector2.one;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            mAllWindDic.Add(windName, windowBase);
            mAllWindowList.Add(windowBase);
            mVisibleWindowList.Add(windowBase);
            return windowBase;
        }
        
        Debug.LogError($"没有加载到对应窗口 {windName}");
        return null;
    }

    public WindowBase GetWindow(string winName)
    {
        if (mAllWindDic.ContainsKey(winName))
        {
            return mAllWindDic[winName];
        }

        return null;
    }

    public GameObject TempLoadWindow(string windName)
    { 
        GameObject window = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Window/" + windName));
        window.transform.SetParent(mUIRoot);
        window.transform.localScale = Vector3.one;
        window.transform.localPosition = Vector3.zero;
        window.transform.rotation = Quaternion.identity;
        return window;
    }

    private WindowBase ShowWindow(string winName)
    {
        WindowBase window = null;
        if (mAllWindDic.ContainsKey(winName))
        {
            window = mAllWindDic[winName];
            if (window.GameObject != null && window.Visible == false)
            {
                mVisibleWindowList.Add(window);
                window.Transform.SetAsLastSibling();
                window.SetVisible(true);
                window.OnShow();
            }
            return window;
        }
        else
        {
            Debug.LogError($"{winName} 窗口不存在， 请调用PopUpWindow 进行弹出");
            return null;
        }
    }
}
