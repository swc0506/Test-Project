using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI
{
    public class UIScrollList : UIBehaviour
    {
        [HideInInspector] public RectTransform rectTransform;
        public ScrollRect scrollRect = null;
        public Arrangement arrangement = Arrangement.Vertical;
        public RectOffset minMargin = new RectOffset();

        public Vector2 miniPadding = Vector2.zero;

        //因为item的size可能会适配viewport的大小,设置true在视图窗口大小变化的时候item会重新计算size
        public bool stretchItem = false;

        //是否居中对齐
        public bool anchorCenter = false;
        public string itemPath = null;

        private Func<int, string> pathHandler;
        private ScrollList scrollList = null;
        private float elasticity = 0;

        public void Initial()
        {
            rectTransform = transform as RectTransform;
            if (null == scrollRect)
            {
                scrollRect = GetComponent<ScrollRect>();
            }

            elasticity = scrollRect.elasticity;
            scrollRect.content.pivot = Vector2.up;
            if (arrangement == Arrangement.Horizontal)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
            }
            else if (arrangement == Arrangement.Vertical)
            {
                scrollRect.vertical = true;
                scrollRect.horizontal = false;
            }

            scrollList = new ScrollList(scrollRect.content);
            scrollList.arrangement = arrangement;
            scrollList.minMargin = minMargin;
            scrollList.miniPadding = miniPadding;
            scrollList.anchorCenter = anchorCenter;
            scrollList.pathHandler = OnItemPath;
            scrollList.scrollItemPositionHandler = OnUpdateContainerPosition;
            scrollList.SetViewportSize(scrollRect.viewport.rect.size);
            scrollRect.onValueChanged.AddListener(OnPositionChanged);
        }

        public void SetProvider(IGameObjectProvider goProvider)
        {
            scrollList.goPool.SetProvider(goProvider);
        }

        public RectOffset Margin
        {
            get { return scrollList.Margin; }
        }

        public int NumItems
        {
            get { return scrollList.NumItems; }
            set { scrollList.NumItems = value; }
        }

        public Func<int, string> PathHandler
        {
            set { pathHandler = value; }
        }

        public Func<int, GameObject, Vector2> SizeHandler
        {
            set { scrollList.sizeHandler = value; }
        }

        public Action<int, GameObject> RenderHandler
        {
            set { scrollList.renderHandler = value; }
        }

        private string OnItemPath(int index)
        {
            if (null != pathHandler)
            {
                return pathHandler.Invoke(index);
            }

            return itemPath;
        }

        public Vector2 GetItemSize(int index)
        {
            return scrollList.GetItemSize(index);
        }

        public void RefreshItemSize(int index)
        {
            scrollList.RefreshItemSize(index);
        }

        public void RefreshAllSize()
        {
            scrollList.RefreshAllSize();
        }

        public void RefreshItemRender(int index)
        {
            scrollList.RefreshItemRender(index);
        }

        public void RefreshAllRender()
        {
            scrollList.RefreshAllRender();
        }

        public void SetItemRenderAsLastSibling(int index)
        {
            scrollList.SetItemRenderAsLastSibling(index);
        }

        public void Append()
        {
            scrollList.Append();
        }

        public void Append(int count)
        {
            for (int i = 0; i < count; i++)
            {
                scrollList.Append();
            }
        }

        public void ScrollToItem(int index, bool smooth = false)
        {
            if (index < 0 || index >= scrollList.NumItems)
            {
                return;
            }

            CloseElasticity();
            scrollList.ScrollToItem(index, smooth);
        }

        public float ScrollRate
        {
            get { return scrollList.ScrollRate; }
        }

        public void ScrollToRate(float rate)
        {
            CloseElasticity();
            scrollList.ScrollToRate(rate);
        }

        private void CloseElasticity()
        {
            //防止和scrollRect的计算movement冲突
            scrollRect.StopMovement();
            scrollRect.elasticity = 5000;
        }

        private void OnUpdateContainerPosition()
        {
            if (scrollRect.content.anchoredPosition != scrollList.ContainerPosition)
            {
                scrollRect.content.anchoredPosition = scrollList.ContainerPosition;
            }
        }

        private void OnPositionChanged(Vector2 pos)
        {
            scrollList.ContainerPosition = scrollRect.content.anchoredPosition;
            scrollRect.elasticity = elasticity;
        }

        private void LateUpdate()
        {
            if (scrollList.ViewportSize != scrollRect.viewport.rect.size)
            {
                scrollList.SetViewportSize(scrollRect.viewport.rect.size);
                if (stretchItem)
                {
                    //因为item的size可能会适配viewport的大小,所以直接刷新所有大小
                    scrollList.RefreshAllSize();
                }
            }

            if (scrollRect.content.rect.size != scrollList.ContainerSize)
            {
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollList.ContainerSize.x);
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scrollList.ContainerSize.y);
            }

            scrollList.OnUpdate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (null != scrollList)
            {
                scrollList.Dispose();
                scrollList = null;
            }
        }
    }
}