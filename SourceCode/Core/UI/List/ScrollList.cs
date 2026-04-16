using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    public class ScrollList
    {
        private Transform itemParent;

        //排列方向
        public Arrangement arrangement;

        //最小外边距
        public RectOffset minMargin;

        //items之间的最小间距
        public Vector2 miniPadding;

        //是否锚点在居中 如果items大小不一样，可能会有问题。
        public bool anchorCenter;

        public Func<int, string> pathHandler;
        public Func<int, GameObject, Vector2> sizeHandler;
        public Action<int, GameObject> renderHandler;
        public Action scrollItemPositionHandler;

        private Vector2 viewportSize;
        private Vector2 containerSize;
        private Vector2 containerPosition;
        public RectOffset Margin { get; private set; }
        public Vector2 Padding { get; private set; }

        private bool layoutDirty;
        private bool viewportDirty;
        private bool renderDirty;

        private int numItems;
        private List<ItemRectInfo> itemsInfo;
        private List<int> pushTemp;
        private List<int> popTemp;
        public GameObjectPool goPool;
        private Dictionary<int, GameObject> renderMap;

        private int scrollTargetIndex;
        private Vector2 scrollTargetPos;
        private bool scrollSmooth;
        private int scrollElapseFrame;
        private float scrollTargetRate;

        public ScrollList(Transform itemParent)
        {
            this.itemParent = itemParent;
            scrollTargetIndex = -1;
            scrollTargetRate = -1;
            itemsInfo = new List<ItemRectInfo>();
            pushTemp = new List<int>();
            popTemp = new List<int>();
            goPool = new GameObjectPool();
            goPool.SetActiveHandler(GameObjectUtils.SetActiveByScale);
            renderMap = new Dictionary<int, GameObject>();
            Margin = new RectOffset();
        }

        public Vector2 ViewportSize
        {
            get { return viewportSize; }
        }

        public Vector2 ContainerSize
        {
            get { return containerSize; }
        }

        public Vector2 ContainerPosition
        {
            get { return containerPosition; }
            set
            {
                if (containerPosition != value)
                {
                    containerPosition = value;
                    viewportDirty = true;
                }
            }
        }

        /// <summary>
        /// 设置item的数量
        /// </summary>
        public int NumItems
        {
            get { return numItems; }
            set
            {
                numItems = value;
                CalcItemRectInfos();
                layoutDirty = true;
                viewportDirty = true;
            }
        }

        public void SetViewportSize(Vector2 value)
        {
            viewportSize = value;
            layoutDirty = true;
            viewportDirty = true;
            renderDirty = true;
        }

        private GameObject GetItemGo(string path)
        {
            GameObject go = goPool.Pop(path);
            if (null != go)
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                rectTrans.SetParent(itemParent, false);
                rectTrans.localScale = Vector3.one;
                rectTrans.pivot = new Vector2(0, 1);
            }

            return go;
        }

        private void ReleaseItemGo(GameObject go, string path)
        {
            goPool.Push(go, path);
        }

        private void CalcItemRectInfos()
        {
            ClearItemsRender();
            itemsInfo.Clear();
            for (int i = 0; i < numItems; i++)
            {
                ItemRectInfo rect = new ItemRectInfo();
                rect.index = i;
                rect.size = CalculateItemSize(i);
                itemsInfo.Add(rect);
            }

            Margin.left = minMargin.left;
            Margin.right = minMargin.right;
            Margin.top = minMargin.top;
            Margin.bottom = minMargin.bottom;
            Padding = new Vector2(miniPadding.x, miniPadding.y);
        }

        private Vector2 CalculateItemSize(int index)
        {
            Vector2 size = Vector2.zero;
            string path = pathHandler.Invoke(index);
            if (string.IsNullOrEmpty(path))
            {
                UnityEngine.Debug.LogErrorFormat("PathHandler path is null,index:{0}", index);
                return size;
            }

            GameObject go = GetItemGo(path);
            if (null == go)
            {
                UnityEngine.Debug.LogErrorFormat("GameObject is null,path:{0}", path);
                return size;
            }

            if (null != sizeHandler)
            {
                size = sizeHandler.Invoke(index, go);
            }
            else
            {
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                size = rectTrans.rect.size;
            }

            ReleaseItemGo(go, path);
            return size;
        }

        private void UpdateItemSize(int index)
        {
            ItemRectInfo rect = itemsInfo[index];
            rect.size = CalculateItemSize(rect.index);
            itemsInfo[index] = rect;
            layoutDirty = true;
            //当前index在视图内，刷新位置，大小信息
            if (renderMap.TryGetValue(rect.index, out var go))
            {
                viewportDirty = true;
                renderDirty = true;
            }
        }

        public Vector2 GetItemSize(int index)
        {
            if (index < 0 && index >= itemsInfo.Count)
            {
                return Vector2.zero;
            }

            return itemsInfo[index].size;
        }

        /// <summary>
        /// 刷新item大小
        /// </summary>
        /// <param name="index">数据的index</param>
        public void RefreshItemSize(int index)
        {
            if (index < 0 || index >= itemsInfo.Count)
            {
                return;
            }

            UpdateItemSize(index);
        }

        public void RefreshAllSize()
        {
            for (int i = 0; i < itemsInfo.Count; i++)
            {
                UpdateItemSize(i);
            }
        }

        /// <summary>
        ///  刷新item内容
        /// </summary>
        /// <param name="index">数据的index</param>
        public void RefreshItemRender(int index)
        {
            //当前index在视图内，刷新一下
            if (renderMap.TryGetValue(index, out var go))
            {
                renderHandler?.Invoke(index, go);
            }
        }

        public void RefreshAllRender()
        {
            foreach (var item in renderMap)
            {
                renderHandler?.Invoke(item.Key, item.Value);
            }
        }

        public void SetItemRenderAsLastSibling(int index)
        {
            if (renderMap.TryGetValue(index, out var go))
            {
                go.transform.SetAsLastSibling();
            }
        }

        private void ClearItemsRender()
        {
            var iter = renderMap.GetEnumerator();
            while (iter.MoveNext())
            {
                int index = iter.Current.Key;
                string path = pathHandler.Invoke(index);
                goPool.Push(iter.Current.Value, path);
            }

            renderMap.Clear();
        }

        private void RefreshLayout()
        {
            if (itemsInfo.Count == 0)
            {
                containerSize = viewportSize;
                containerPosition = Vector2.zero;
                return;
            }

            for (int i = 0; i < itemsInfo.Count; i++)
            {
                SetItemRectPosition(i);
            }

            SetContainerSize();
        }

        private void SetItemRectPosition(int index)
        {
            //左上角为锚点算坐标
            var rect = itemsInfo[index];
            Vector2 beginPos = Vector2.zero;
            if (index == 0)
            {
                beginPos = new Vector2(Margin.left, -Margin.top);
            }
            else if (index > 0)
            {
                var prevRect = itemsInfo[index - 1];
                if (arrangement == Arrangement.Horizontal)
                {
                    beginPos = new Vector2(prevRect.position.x, prevRect.yMin - Padding.y);
                    if (beginPos.y - itemsInfo[index].size.y < -viewportSize.y)
                    {
                        var flexible = FindFlexibleRect(index - 1);
                        beginPos = new Vector2(flexible.xMax + Padding.x, -Margin.top);
                    }
                }
                else if (arrangement == Arrangement.Vertical)
                {
                    beginPos = new Vector2(prevRect.xMax + Padding.x, prevRect.position.y);
                    if (beginPos.x + itemsInfo[index].size.x > viewportSize.x)
                    {
                        var flexible = FindFlexibleRect(index - 1);
                        beginPos = new Vector2(Margin.left, flexible.yMin - Padding.y);
                    }
                }
            }

            rect.position = beginPos;
            itemsInfo[index] = rect;
        }

        private ItemRectInfo FindFlexibleRect(int index)
        {
            ItemRectInfo flexible = itemsInfo[index];
            for (int i = index; i >= 0; i--)
            {
                if (arrangement == Arrangement.Horizontal)
                {
                    if (flexible.xMin == itemsInfo[i].xMin)
                    {
                        if (flexible.xMax < itemsInfo[i].xMax)
                        {
                            flexible = itemsInfo[i];
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else if (arrangement == Arrangement.Vertical)
                {
                    if (flexible.yMax == itemsInfo[i].yMax)
                    {
                        if (flexible.yMin > itemsInfo[i].yMin)
                        {
                            flexible = itemsInfo[i];
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return flexible;
        }

        private void AdjustAnchorCenter()
        {
            float viewportValue = 0;
            float totalSize = 0;
            float maxValue = 0;
            int index = 0;
            int maxIndex = itemsInfo.Count - 1;
            while (maxIndex >= 0 && index <= maxIndex)
            {
                //取第一行items作为参考值
                ItemRectInfo rect = itemsInfo[index];
                float size = 0;
                if (arrangement == Arrangement.Horizontal)
                {
                    viewportValue = viewportSize.y;
                    size = rect.size.y;
                    float offset = index == 0 ? (minMargin.top + minMargin.bottom) : miniPadding.y;
                    maxValue += offset + size;
                }
                else if (arrangement == Arrangement.Vertical)
                {
                    viewportValue = viewportSize.x;
                    size = rect.size.x;
                    float offset = index == 0 ? (minMargin.left + minMargin.right) : miniPadding.x;
                    maxValue += offset + size;
                }

                if (maxValue > viewportValue)
                {
                    break;
                }

                totalSize += size;
                index++;
            }

            if (index > 0)
            {
                Vector2 padding = Padding;
                RectOffset margin = Margin;
                float fitPadding = viewportValue - totalSize;
                fitPadding = fitPadding / (index + 1);
                if (arrangement == Arrangement.Horizontal)
                {
                    padding.y = fitPadding;
                    margin.top = margin.bottom = (int)fitPadding;
                }
                else if (arrangement == Arrangement.Vertical)
                {
                    padding.x = fitPadding;
                    margin.left = margin.right = (int)fitPadding;
                }

                Margin = margin;
                Padding = padding;
            }
        }

        private void SetContainerSize()
        {
            if (arrangement == Arrangement.Horizontal)
            {
                var flexible = FindFlexibleRect(numItems - 1);
                containerSize = new Vector2(flexible.xMax + minMargin.right, viewportSize.y);
            }
            else if (arrangement == Arrangement.Vertical)
            {
                var flexible = FindFlexibleRect(numItems - 1);
                containerSize = new Vector2(viewportSize.x, -(flexible.yMin - minMargin.bottom));
            }
        }

        private void RefreshViewItems()
        {
            for (int i = 0; i < itemsInfo.Count; i++)
            {
                if (IsInViewport(i))
                {
                    pushTemp.Add(i);
                }
                else
                {
                    popTemp.Add(i);
                }
            }

            foreach (var item in popTemp)
            {
                PopItemRender(itemsInfo[item]);
            }

            foreach (var item in pushTemp)
            {
                PushItemRender(itemsInfo[item]);
            }

            popTemp.Clear();
            pushTemp.Clear();
            renderDirty = false;
        }

        private void SetItemRenderRect(ItemRectInfo info, GameObject go)
        {
            RectTransform rectTrans = go.GetComponent<RectTransform>();
            //anchor会影响坐标和大小
            rectTrans.offsetMin = rectTrans.offsetMax = Vector2.zero;
            if (rectTrans.anchorMin == rectTrans.anchorMax && rectTrans.anchorMin == Vector2.up)
            {
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, info.size.x);
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, info.size.y);
            }
            else if (rectTrans.anchorMin == Vector2.up && rectTrans.anchorMax == Vector2.one)
            {
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, info.size.y);
            }
            else if (rectTrans.anchorMin == Vector2.zero && rectTrans.anchorMax == Vector2.up)
            {
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, info.size.x);
            }

            rectTrans.anchoredPosition = info.position;
            rectTrans.SetSiblingIndex(info.index);
        }

        private void PushItemRender(ItemRectInfo info)
        {
            if (!renderMap.TryGetValue(info.index, out var go))
            {
                go = GetItemGo(pathHandler.Invoke(info.index));
                if (Application.isEditor)
                {
                    go.name = string.Format("Item:{0}", info.index);
                }

                SetItemRenderRect(info, go);
                renderHandler?.Invoke(info.index, go);
                renderMap.Add(info.index, go);
            }
            else
            {
                if (renderDirty)
                {
                    SetItemRenderRect(info, go);
                }
            }
        }

        private void PopItemRender(ItemRectInfo info)
        {
            if (renderMap.ContainsKey(info.index))
            {
                string path = pathHandler.Invoke(info.index);
                ReleaseItemGo(renderMap[info.index], path);
                renderMap.Remove(info.index);
            }
        }

        // 是否在视图区域内,只要有交集就算
        private bool IsInViewport(int index)
        {
            ItemRectInfo rect = itemsInfo[index];
            if (arrangement == Arrangement.Horizontal)
            {
                float xMin = rect.position.x + containerPosition.x;
                float xMax = xMin + rect.size.x;
                return xMax > 0 && xMin < viewportSize.x;
            }
            else if (arrangement == Arrangement.Vertical)
            {
                float yMax = rect.position.y + containerPosition.y;
                float yMin = yMax - rect.size.y;
                return yMax > -viewportSize.y && yMin < 0;
            }

            return true;
        }

        /// <summary>
        /// 追加item到尾部  适合类似聊天情景等使用
        /// </summary>
        public void Append()
        {
            int index = numItems++;
            ItemRectInfo rect = new ItemRectInfo();
            rect.index = index;
            rect.size = CalculateItemSize(index);
            itemsInfo.Add(rect);

            SetItemRectPosition(index);
            SetContainerSize();
            if (IsInViewport(rect.index))
            {
                viewportDirty = true;
            }
        }

        /// <summary>
        /// 滑动待对应item
        /// </summary>
        /// <param name="index">数据的index</param>
        /// <param name="smooth">是否需要平滑过去</param>
        public void ScrollToItem(int index, bool smooth)
        {
            if (index < 0 && index >= itemsInfo.Count)
            {
                return;
            }

            scrollTargetIndex = index;
            scrollSmooth = smooth;
        }

        private void CalculateScrollIndexPosition()
        {
            if (scrollTargetIndex < 0 || scrollTargetIndex >= itemsInfo.Count)
            {
                return;
            }

            ItemRectInfo rect = itemsInfo[scrollTargetIndex];
            //已经在视图窗口内了
            if (renderMap.ContainsKey(rect.index))
            {
                if (IsInsideViewport(rect.index))
                {
                    scrollSmooth = false;
                    return;
                }
            }

            Vector2 pos = Vector2.zero;
            if (arrangement == Arrangement.Horizontal)
            {
                while (true)
                {
                    if (--scrollTargetIndex < 0)
                    {
                        break;
                    }

                    if (rect.position.x != itemsInfo[scrollTargetIndex].position.x)
                    {
                        pos.x = -itemsInfo[scrollTargetIndex].xMax;
                        break;
                    }
                }

                float xMin = -(containerSize.x - viewportSize.x);
                pos.x = pos.x < xMin ? xMin : pos.x;
            }
            else if (arrangement == Arrangement.Vertical)
            {
                while (true)
                {
                    if (--scrollTargetIndex < 0)
                    {
                        break;
                    }

                    if (rect.position.y != itemsInfo[scrollTargetIndex].position.y)
                    {
                        pos.y = -itemsInfo[scrollTargetIndex].yMin;
                        break;
                    }
                }

                float yMax = containerSize.y - viewportSize.y;
                pos.y = pos.y > yMax ? yMax : pos.y;
            }

            scrollTargetIndex = -1;
            if (scrollSmooth)
            {
                scrollElapseFrame = 0;
                scrollTargetPos = pos;
            }
            else
            {
                containerPosition = pos;
                viewportDirty = true;
                scrollItemPositionHandler?.Invoke();
            }
        }

        public float ScrollRate
        {
            get
            {
                float value = 0;
                if (arrangement == Arrangement.Horizontal)
                {
                    float delta = -(containerSize.x - viewportSize.x);
                    value = containerPosition.x / delta;
                }
                else if (arrangement == Arrangement.Vertical)
                {
                    float delta = containerSize.y - viewportSize.y;
                    value = containerPosition.y / delta;
                }

                value = Mathf.Clamp(value, 0, 1);
                return value;
            }
        }

        public void ScrollToRate(float rate)
        {
            rate = Mathf.Clamp(rate, 0, 1);
            scrollTargetRate = rate;
        }

        private void CalculateScrollRatePosition()
        {
            Vector2 pos = Vector2.zero;
            if (arrangement == Arrangement.Horizontal)
            {
                float delta = -(containerSize.x - viewportSize.x);
                pos.x = delta * scrollTargetRate;
            }
            else if (arrangement == Arrangement.Vertical)
            {
                float delta = containerSize.y - viewportSize.y;
                pos.y = delta * scrollTargetRate;
            }

            scrollTargetRate = -1;

            containerPosition = pos;
            viewportDirty = true;
            scrollItemPositionHandler?.Invoke();
        }

        // 是否在视图区域内,要整个包含在内才算
        private bool IsInsideViewport(int index)
        {
            ItemRectInfo rectInfo = itemsInfo[index];
            if (arrangement == Arrangement.Horizontal)
            {
                float xMin = rectInfo.position.x + containerPosition.x;
                float xMax = xMin + rectInfo.size.x;
                return xMin >= 0 && xMax <= viewportSize.x;
            }
            else if (arrangement == Arrangement.Vertical)
            {
                float yMax = rectInfo.position.y + containerPosition.y;
                float yMin = yMax - rectInfo.size.y;
                return yMin >= -viewportSize.y && yMax <= 0;
            }

            return true;
        }

        public void OnUpdate()
        {
            if (layoutDirty)
            {
                //如果要左对齐固定在中心的，计算边距和间距
                if (viewportDirty && anchorCenter)
                {
                    AdjustAnchorCenter();
                }

                layoutDirty = false;
                RefreshLayout();
            }

            if (viewportDirty)
            {
                viewportDirty = false;
                RefreshViewItems();
            }

            if (scrollTargetIndex >= 0)
            {
                CalculateScrollIndexPosition();
            }

            if (scrollTargetRate >= 0)
            {
                CalculateScrollRatePosition();
            }

            if (scrollSmooth)
            {
                SmoothScrollPosition();
            }
        }

        private void SmoothScrollPosition()
        {
            scrollElapseFrame++;
            containerPosition = Vector2.Lerp(containerPosition, scrollTargetPos, UnityEngine.Time.deltaTime * 15);
            Vector2 delta = scrollTargetPos - containerPosition;
            if (Vector2.SqrMagnitude(delta) < 1)
            {
                containerPosition = scrollTargetPos;
                scrollSmooth = false;
            }

            //优化每帧都更刷新视图
            if (!scrollSmooth || scrollElapseFrame % 4 == 0)
            {
                viewportDirty = true;
            }

            scrollItemPositionHandler?.Invoke();
        }

        public void Dispose()
        {
            ClearItemsRender();
            if (null != goPool)
            {
                goPool.Dispose();
                goPool = null;
            }

            itemsInfo = null;
            renderMap = null;
        }
    }
}