using System;
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
        private WindowConfig mWindowConfig;

        private Dictionary<string, WindowBase> mAllWindowDic = new Dictionary<string, WindowBase>();
        private List<WindowBase> mAllWindowList = new List<WindowBase>(); //所有窗口列表
        private List<WindowBase> mVisibleWindowList = new List<WindowBase>(); //所有可见窗口列表

        private Queue<WindowBase> mWindowStack = new Queue<WindowBase>(); // 队列，用来管理弹窗的循环弹出
        private bool mStartPopStackWindStatus = false; //开始弹出堆栈的标志，可以用来处理多种情况
        private bool mSmartShowHide = true; //智能显影

        public void Initialize()
        {
            mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            mUIRoot = GameObject.Find("UIRoot").transform;
            mWindowConfig = Resources.Load<WindowConfig>("WindowConfig");

# if UNITY_EDITOR
            mWindowConfig.GeneratorWindowConfig();
#endif
        }

        #region 窗口管理

        /// <summary>
        /// 只加载物体，不调用生命周期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void PreLoadWindow<T>() where T : WindowBase, new()
        {
            Type type = typeof(T);
            string name = type.Name;
            T windowBase = new T();
            //克隆界面，初始化界面信息
            //1。生成对应的窗口预制体
            GameObject nWindow = TempLoadWindow(name);
            //2.初始化对应管理类
            if (nWindow != null)
            {
                windowBase.GameObject = nWindow;
                windowBase.Transform = nWindow.transform;
                windowBase.Canvas = nWindow.GetComponent<Canvas>();
                windowBase.Canvas.worldCamera = mUICamera;
                windowBase.Name = nWindow.name;
                windowBase.OnAwake();
                windowBase.SetVisible(false);
                RectTransform rectTrans = nWindow.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                mAllWindowDic.Add(name, windowBase);
                mAllWindowList.Add(windowBase);
            }
            else
            {
                Debug.LogError("预加载失败，界面名：" + name);
            }
        }

        /// <summary>
        /// 弹出弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PopUpWindow<T>() where T : WindowBase, new()
        {
            Type type = typeof(T);
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

            WindowBase maxOrderWindBase = null; //最大渲染层级的窗口
            int maxOrder = 0; //最大渲染层级
            int maxIndex = 0; //最大排序下标 在相同父节点下的位置下标

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
                    else if (maxOrder == windowBase.Canvas.sortingOrder &&
                             maxIndex < windowBase.Transform.GetSiblingIndex())
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
                ShowWindowAndModifyAllWindowCanvasGroup(windowBase, 0);
                return windowBase;
            }

            Debug.LogError("没有该预制体：" + windName);
            return null;
        }

        //临时处理
        private GameObject TempLoadWindow(string name)
        {
            var obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(mWindowConfig.GetWindowPath(name)),
                mUIRoot);
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

        private WindowBase ShowWindow(string name, bool maskAni = true)
        {
            if (mAllWindowDic.TryGetValue(name, out WindowBase window))
            {
                if (window.Visible == false)
                {
                    mVisibleWindowList.Add(window);
                    window.Transform.SetAsLastSibling();
                    window.SetVisible(true);
                    SetWindowMaskVisible();
                    ShowWindowAndModifyAllWindowCanvasGroup(window, 0);
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
                //在出栈的情况下，上一个界面隐藏时，自动打开栈中的下一个界面
                PopNextStackWindow(windowBase);
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
                //在出栈的情况下，上一个界面销毁时，自动打开栈中的下一个界面
                PopNextStackWindow(window);
            }
        }

        #endregion

        #region 堆栈系统

        /// <summary>
        /// 进栈一个界面
        /// </summary>
        /// <param name="popCallBack"></param>
        /// <typeparam name="T"></typeparam>
        public void PushWindowToStack<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
        {
            T windowBase = new T();
            windowBase.PopStackListener = popCallBack;
            mWindowStack.Enqueue(windowBase);
        }

        /// <summary>
        /// 弹出堆栈中第一个弹窗
        /// </summary>
        public void StartPopFirstStackWindow()
        {
            if (mStartPopStackWindStatus) return;
            mStartPopStackWindStatus = true; //已经开始进行堆栈弹出的流程
            PopStackWindow();
        }

        /// <summary>
        /// 弹出堆栈弹窗
        /// </summary>
        /// <returns></returns>
        public bool PopStackWindow()
        {
            if (mWindowStack.Count > 0)
            {
                WindowBase window = mWindowStack.Dequeue();
                WindowBase popWindow = PopUpWindow(window);
                popWindow.PopStackListener = window.PopStackListener;
                popWindow.PopStack = true;
                popWindow.PopStackListener?.Invoke(popWindow);
                popWindow.PopStackListener = null;
                return true;
            }
            else
            {
                mStartPopStackWindStatus = false;
                return false;
            }
        }

        /// <summary>
        /// 压入并弹出
        /// </summary>
        /// <param name="popCallBack"></param>
        /// <typeparam name="T"></typeparam>
        public void PushAndPopStackWindow<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
        {
            PushWindowToStack<T>(popCallBack);
            StartPopFirstStackWindow();
        }

        /// <summary>
        /// 弹出下一个
        /// </summary>
        /// <param name="windowBase"></param>
        private void PopNextStackWindow(WindowBase windowBase)
        {
            if (windowBase != null && mStartPopStackWindStatus && windowBase.PopStack)
            {
                windowBase.PopStack = false;
                PopStackWindow();
            }
        }

        private WindowBase PopUpWindow(WindowBase window)
        {
            Type type = window.GetType();
            string name = type.Name;
            WindowBase wind = GetWindow(name);
            if (wind != null)
            {
                return ShowWindow(name);
            }

            return InitializeWindow(window, name);
        }

        public void ClearStackWindows()
        {
            mWindowStack.Clear();
        }

        #endregion

        #region 智能显隐

        private void ShowWindowAndModifyAllWindowCanvasGroup(WindowBase window, int value)
        {
            if (!mSmartShowHide)
            {
                return;
            }

            //if (WorldManager.IsHallWorld && window.FullScreenWindow) 可以以此种方式决定智能显隐开启场景
            if (window.FullScreenWindow)
            {
                try
                {
                    //当显示的弹窗是大厅是，不对其他弹窗进行伪隐藏，
                    if (string.Equals(window.Name, "HallWindow"))
                    {
                        return;
                    }

                    if (mVisibleWindowList.Count > 1)
                    {
                        //处理先弹弹窗 后关弹窗的情况
                        WindowBase curShowBase = mVisibleWindowList[mVisibleWindowList.Count - 2];
                        if (!curShowBase.FullScreenWindow &&
                            window.Canvas.sortingOrder < curShowBase.Canvas.sortingOrder)
                        {
                            return;
                        }
                    }

                    for (int i = mVisibleWindowList.Count - 1; i >= 0; i--)
                    {
                        WindowBase item = mVisibleWindowList[i];
                        if (item.Name != window.Name)
                        {
                            item.PseudoHidden(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error:" + ex);
                }
            }
        }

        private void HideWindowAndModifyAllWindowCanvasGroup(WindowBase window, int value)
        {
            if (!mSmartShowHide)
            {
                return;
            }

            //if (WorldManager.IsHallWorld && window.FullScreenWindow) 可以以此种方式决定智能显隐开启场景
            if (window.FullScreenWindow)
            {
                for (int i = mVisibleWindowList.Count - 1; i >= 0; i--)
                {
                    if (i >= 0 && mVisibleWindowList[i] != window)
                    {
                        mVisibleWindowList[i].PseudoHidden(1);
                        //找到上一个窗口，如果是全屏窗口，将其设置可见，终止循转。否则循环至最终
                        if (mVisibleWindowList[i].FullScreenWindow)
                        {
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}