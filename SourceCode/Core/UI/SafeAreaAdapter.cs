using System;
using System.Collections.Generic;
using UnityEngine;

#if FAIRYGUI
using FairyGUI;
#endif

namespace Core.UI
{
    public class SafeAreaAdapter
    {
        private float designRatio;
        private Func<Rect> safeAreaFunc;

        private Rect safeArea;
        private Rect viewArea;

        private const string FILL_KEY = "fill";
        private Dictionary<object, int> widgetMap = new Dictionary<object, int>();
        private Dictionary<string, List<object>> fillMap = new Dictionary<string, List<object>>();

        public Rect SafeArea
        {
            get { return safeArea; }
        }

        public Rect ViewArea
        {
            get { return viewArea; }
        }

        internal void Initial()
        {
            int designWidth = UIManager.Instance.DesignWidth;
            int designHeight = UIManager.Instance.DesignHeight;
            designRatio = (float)designWidth / designHeight;

            ResetCanvasSize();
#if FAIRYGUI
            GRoot.inst.onSizeChanged.Add(ResetCanvasSize);
#else
            MonoEventProxy.Instance.ScreenSizeChangeEvent += ResetCanvasSize;
#endif
            UIManager.Instance.AddListener(UIEvent.BeforeStart, OnAddPanelFillWidgets);
            UIManager.Instance.AddListener(UIEvent.Destroy, OnRemovePanelFillWidgets);
        }

        public void SetSafeAreaFunc(Func<Rect> func)
        {
            this.safeAreaFunc = func;
        }

        private Rect GetSafeRect()
        {
            Rect rect = Screen.safeArea;
            if (rect == Rect.zero && null != safeAreaFunc)
            {
                rect = safeAreaFunc.Invoke();
            }

            return rect;
        }

        private bool IsLandscape()
        {
            return designRatio > 1 && Screen.width > Screen.height;
        }

        private void ResetCanvasSize()
        {
            safeArea = GetSafeRect();
            if (UIManager.Instance.UIMode == UIMode.FairyGUI)
            {
#if FAIRYGUI
                viewArea.width = GRoot.inst.width;
                viewArea.height = GRoot.inst.height;
#endif
            }

            float x = 0;
            float y = 0;
            float w = viewArea.width;
            float h = viewArea.height;

            if (IsLandscape())
            {
                x = safeArea.x / Screen.width * viewArea.width;
                y = 0;
                w = viewArea.width - x;
                h = viewArea.height;
            }
            else
            {
                x = 0;
                y = safeArea.y / Screen.height * viewArea.height;
                w = viewArea.width;
                h = viewArea.height - y;
            }

            viewArea.x = x;
            viewArea.y = y;
            ResetLayersSize(w, h, x, y);
            ResetWidgetsLocation();

            UIManager.Instance.DispatchListener(UIEvent.Resize, null);
        }

        private void ResetLayersSize(float w, float h, float x, float y)
        {
            if (UIManager.Instance.UIMode == UIMode.FairyGUI)
            {
#if FAIRYGUI
                int begin = (int)UILayer.HUD;
                int end = (int)UILayer.Dialog + 1;
                for (int i = begin; i < end; i++)
                {
                    GObject comp = UIManager.Instance.GetLayer<GObject>((UILayer)i);
                    comp.SetSize(w, h);
                    comp.SetPosition(x, y, 0);
                }
#endif
            }
        }

        private void ResetWidgetsLocation()
        {
            foreach (var item in widgetMap)
            {
                ResetWidgetLocation(item.Key, item.Value);
            }
        }

        private void ResetWidgetLocation(object widget, int type)
        {
            if (UIManager.Instance.UIMode == UIMode.FairyGUI)
            {
                if (IsLandscape())
                {
#if FAIRYGUI
                    GObject obj = (GObject)widget;
                    float x = -viewArea.x;
                    float y = obj.y;
                    float w = viewArea.width + viewArea.x;
                    float h = viewArea.height;

                    var hr = w / obj.initWidth;
                    var vr = h / obj.initHeight;
                    var r = Mathf.Max(hr, vr);

                    if (type == 1) //等比缩放
                    {
                        var lw = obj.initWidth * r;
                        var lh = obj.initHeight * r;
                        obj.SetSize(lw, lh);
                    }
                    else if (type == 2) //直接拉伸
                    {
                        obj.SetSize(w, h);
                    }
                    else if (type == 3) //水平拉伸
                    {
                        obj.SetSize(w, obj.height);
                    }

                    float delta = viewArea.x + (obj.width - w);
                    float lx = x - delta * obj.pivotX;

                    float ly = obj.y;
                    if (type == 1 || type == 2)
                    {
                        delta = obj.height - viewArea.height;
                        ly = 0 - delta * obj.pivotY;
                    }
                    obj.SetPosition(lx, ly, 0);
#endif
                }
                else
                {
#if FAIRYGUI
                    GObject obj = (GObject)widget;
                    float x = obj.x;
                    float y = -viewArea.y;
                    float w = viewArea.width;
                    float h = viewArea.height + viewArea.y;

                    //控件高小于屏幕高
                    if (obj.height < h)
                    {
                        if (type == 1) //等比缩放
                        {
                            float rw = obj.initWidth * obj.scaleX;
                            float rh = obj.initHeight * obj.scaleY;
                            float ratio = h / rh;
                            float lw = rw * ratio;
                            float lh = rh * ratio;

                            x = 0;
                            float delta = viewArea.y + (h - viewArea.height);
                            y += -delta * obj.pivotY;
                            obj.SetSize(lw, lh);
                        }
                        else if (type == 2) //直接拉伸
                        {
                            obj.SetSize(w, h);
                        }
                        else if (type == 3) //垂直拉伸
                        {
                            float lw = obj.width;
                            obj.SetSize(lw, h);
                        }
                    }
                    else //控件高大于屏幕高
                    {
                        float delta = viewArea.y + (obj.height - h);
                        y += -delta * obj.pivotY;
                    }

                    //控件宽小于屏幕宽
                    if (obj.width < w)
                    {
                        if (type == 1) //等比缩放
                        {
                            float rw = obj.initWidth * obj.scaleX;
                            float rh = obj.initHeight * obj.scaleY;
                            float ratio = w / rw;
                            float lw = rw * ratio;
                            float lh = rh * ratio;
                            obj.SetSize(lw, lh);

                            float delta = lw - viewArea.width;
                            x = -delta * obj.pivotX;
                        }
                        else if (type == 2) //直接拉伸
                        {
                            obj.SetSize(w, h);
                        }
                    }

                    obj.SetPosition(x, y, 0);
#endif
                }
            }
        }

        private bool AddFillWidget(object widget, int fillType)
        {
            if (null != widget && !widgetMap.ContainsKey(widget))
            {
                widgetMap[widget] = fillType;
                if (UIManager.Instance.UIMode == UIMode.FairyGUI)
                {
#if FAIRYGUI
                    GObject obj = (GObject)widget;
                    obj.relations.ClearAll();
                    if (obj is GLoader loader)
                    {
                        loader.fill = FillType.ScaleFree;
                    }
#endif
                }

                ResetWidgetLocation(widget, fillType);
                return true;
            }

            return false;
        }

        private void RemoveFileWidget(object widget)
        {
            if (null != widget)
            {
                widgetMap.Remove(widget);
            }
        }

        private void OnAddPanelFillWidgets(IUIController ctrl)
        {
            FindPanelFillWidgets(ctrl.UIName, ctrl.GUI);
        }

        private void FindPanelFillWidgets(string uiName, object gui)
        {
            if (UIManager.Instance.UIMode == UIMode.FairyGUI)
            {
#if FAIRYGUI
                GComponent comp = (GComponent)gui;
                int count = comp.numChildren;
                for (int i = 0; i < count; i++)
                {
                    GObject child = comp.GetChildAt(i);
                    FillWidgetNode(uiName, child);

                    if (child is GComponent)
                    {
                        FindPanelFillWidgets(uiName, child);
                    }
                }
#endif
            }
        }

        private void FillWidgetNode(string uiName, object node)
        {
#if FAIRYGUI
            GObject child = (GObject)node;
            string key = null;
            if (null != child.data)
            {
                key = child.data.ToString();
            }

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            int index = key.IndexOf(FILL_KEY);
            if (index < 0)
            {
                return;
            }

            var type = 0;
            if (key.Length > index + FILL_KEY.Length)
            {
                string typeStr = key.Substring(index + FILL_KEY.Length);
                int.TryParse(typeStr, out type);
            }

            bool res = AddFillWidget(child, type);
            if (res)
            {
                if (!fillMap.TryGetValue(uiName, out var list))
                {
                    list = new List<object>();
                    fillMap.Add(uiName, list);
                }

                list.Add(child);
            }
#endif
        }

        private void OnRemovePanelFillWidgets(IUIController ctrl)
        {
            if (fillMap.TryGetValue(ctrl.UIName, out var list))
            {
                foreach (var item in list)
                {
                    RemoveFileWidget(item);
                }

                fillMap.Remove(ctrl.UIName);
            }
        }

        internal void Dispose()
        {
            widgetMap = null;
            MonoEventProxy.Instance.ScreenSizeChangeEvent -= ResetCanvasSize;
            UIManager.Instance.RemoveListener(UIEvent.BeforeStart, OnAddPanelFillWidgets);
            UIManager.Instance.RemoveListener(UIEvent.Destroy, OnRemovePanelFillWidgets);
        }
    }
}