using System.Collections.Generic;
using UnityEngine;
using ZMUIFrameWork.Scripts.Runtime.Base;

namespace ZMUIFrameWork.Scripts.Runtime.Core
{
    public class UIModule
    {
        private static UIModule instance;
        public static UIModule Instance => instance ?? (instance = new UIModule());

        private Camera mUICamera;
        private Transform mUIRoot;

        private Dictionary<string, WindowBase> mAllWindowDic = new Dictionary<string, WindowBase>();
        private List<WindowBase> mAllWindowList = new List<WindowBase>();//所有窗口列表
        private List<WindowBase> mVisibleWindowList = new List<WindowBase>();//所有可见窗口列表

        public void Initialize()
        {
            mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            mUIRoot = GameObject.Find("UIRoot").transform;
        }

        public T PopUpWindow<T>() where T:WindowBase, new()
        {
            System.Type type = typeof(T);
            string name = type.Name;
            WindowBase wind = GetWind(name);
            if (wind != null)
            {
                return ShowWindow(name) as T;
            }

            T t = new T();
            return InitializeWindow(t, name) as T;
        }

        private WindowBase InitializeWindow(WindowBase windowBase, string windName)
        {
            //生成对应窗口预制体
            GameObject nWindow = TempLoadWindow(windName);
            //初始化对应管理类
            if (nWindow != null)
            {
                windowBase.GameObject = nWindow;
                windowBase.Transform = nWindow.transform;
                windowBase.Canvas = nWindow.GetComponent<Canvas>();
                windowBase.Canvas.worldCamera = mUICamera;
                windowBase.Transform.SetAsLastSibling();
                windowBase.OnAwake();
                windowBase.SetVisible(true);
                windowBase.OnShow();
                RectTransform rectTrans = nWindow.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                mAllWindowDic.Add(windName, windowBase);
                mAllWindowList.Add(windowBase);
                mVisibleWindowList.Add(windowBase);
                return windowBase;
            }
            
            Debug.LogError("没有该预制体：" + windName);
            return null;
        }

        //临时处理
        private GameObject TempLoadWindow(string name)
        {
            var obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Window/" + name), mUIRoot, true);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            return obj;
        }
        
        private WindowBase GetWind(string name)
        {
            if (mAllWindowDic.ContainsKey(name))
            {
                return mAllWindowDic[name];
            }

            return null;
        }

        private WindowBase ShowWindow(string name)
        {
            if (mAllWindowDic.TryGetValue(name, out WindowBase window))
            {
                if (window.Visible == false)
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
                Debug.LogError(name + "窗口不存在，调用Pop弹出");
                return null;
            }
        }
    }
}