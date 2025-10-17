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
        private List<WindowBase> mAllWindowList = new List<WindowBase>(); //所有窗口列表
        private List<WindowBase> mVisibleWindowList = new List<WindowBase>(); //所有可见窗口列表

        public void Initialize()
        {
            mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            mUIRoot = GameObject.Find("UIRoot").transform;
        }

        /// <summary>
        /// 弹出弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PopUpWindow<T>() where T : WindowBase, new()
        {
            System.Type type = typeof(T);
            string name = type.Name;
            WindowBase wind = GetWindow(name);
            if (wind != null)
            {
                return ShowWindow(name) as T;
            }

            T t = new T();
            return InitializeWindow(t, name) as T;
        }

        /// <summary>
        /// 获取已经弹出的弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetVisibleWindow<T>() where T : WindowBase
        {
            System.Type type = typeof(T);
            foreach (var item in mVisibleWindowList)
            {
                if (item.Name == type.Name)
                {
                    return (T)item;
                }
            }

            Debug.LogError("该窗口没有获取到：" + type.Name);
            return null;
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HideWindow<T>() where T : WindowBase
        {
            HideWindow(typeof(T).Name);
        }

        /// <summary>
        /// 销毁窗口
        /// </summary>
        public void DestroyWindow<T>() where T : WindowBase
        {
            DestroyWindow(typeof(T).Name);
        }

        /// <summary>
        /// 销毁所有窗口
        /// </summary>
        /// <param name="filterList">过滤列表</param>
        public void DestroyAllWindow(List<string> filterList = null)
        {
            for (int i = mAllWindowList.Count - 1; i >= 0; i--)
            {
                WindowBase window = mAllWindowList[i];
                if (window == null || (filterList != null && filterList.Contains(window.Name)))
                {
                    continue;
                }

                DestroyWindow(window.Name);
            }

            Resources.UnloadUnusedAssets();
        }

        private void SetWindowMaskVisible()
        {
            if (!UISetting.Instance.SINGMASK_SYSTRM)
            {
                return;
            }

            WindowBase maxOrderWindBase = null;//最大渲染层级的窗口
            int maxOrder = 0;//最大渲染层级
            int maxIndex = 0;//最大排序下标 在相同父节点下的位置下标
            
            //关闭所有窗口的Mask 设置为不可见
            //从所有可见窗口中找到一个层级最大的窗口，把Mask设置为可见
            for (int i = 0; i < mVisibleWindowList.Count; i++)
            {
                WindowBase windowBase = mVisibleWindowList[i];
                if (windowBase != null && windowBase.GameObject != null)
                {
                    maxOrderWindBase = windowBase;
                    maxOrder = windowBase.Canvas.sortingOrder;
                    maxIndex = windowBase.Transform.GetSiblingIndex();
                }
                else
                {
                    //找到最大渲染层级的窗口，拿到它
                    if (maxOrder < windowBase.Canvas.sortingOrder)
                    {
                        maxOrder = windowBase.Canvas.sortingOrder;
                        maxOrderWindBase = windowBase;
                    }
                    //如果两个窗口的渲染层级相同，找到同节点最靠下的一个物体
                    else if (maxOrder == windowBase.Canvas.sortingOrder && maxIndex < windowBase.Transform.GetSiblingIndex())
                    {
                        maxOrderWindBase = windowBase;
                        maxIndex = windowBase.Transform.GetSiblingIndex();
                    }
                }
            }

            maxOrderWindBase?.SetMaskVisible(true);
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
                windowBase.Name = nWindow.name;
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
                SetWindowMaskVisible();
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
            obj.name = name;
            return obj;
        }

        private WindowBase GetWindow(string name)
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
                    SetWindowMaskVisible();
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

        public void HideWindow(string windName)
        {
            if (mAllWindowDic.TryGetValue(windName, out WindowBase window))
            {
                HideWindow(window);
            }
        }

        private void HideWindow(WindowBase windowBase)
        {
            if (windowBase != null && windowBase.Visible)
            {
                mVisibleWindowList.Remove(windowBase);
                windowBase.SetVisible(false);
                SetWindowMaskVisible();
                windowBase.OnHide();
            }
        }

        private void DestroyWindow(string windName)
        {
            if (mAllWindowDic.TryGetValue(windName, out WindowBase window))
            {
                DestroyWindow(window);
            }
        }

        private void DestroyWindow(WindowBase window)
        {
            if (window != null)
            {
                mAllWindowDic.Remove(window.Name);
                mAllWindowList.Remove(window);
                mVisibleWindowList.Remove(window);
                window.SetVisible(false);
                SetWindowMaskVisible();
                window.OnHide();
                window.OnDestroy();
                GameObject.Destroy(window.GameObject);
            }
        }
    }
}