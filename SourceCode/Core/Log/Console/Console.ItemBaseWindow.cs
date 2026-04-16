using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Log
{
    internal partial class ConsoleController
    {
        private abstract class BaseWindow
        {
            //ArgumentException: Mesh can not have more than 65000 vertices
            protected const int MAX_RENDER_TEXT_LENGTH = (65000 / 4) - 1;

            protected ConsoleView View { get; private set; }

            public virtual void Start(ConsoleView view)
            {
                this.View = view;
            }

            public virtual void OnEnable()
            {
            }

            public virtual void OnDisable()
            {
            }

            public virtual void OnUpdate(float deltaTime)
            {
            }

            protected string GetRenderString(StringBuilder strBuilder)
            {
                return strBuilder.ToString(0, Math.Min(strBuilder.Length, MAX_RENDER_TEXT_LENGTH));
            }

            protected string GetRenderString(string str)
            {
                return str.Length > MAX_RENDER_TEXT_LENGTH ? str.Substring(0, MAX_RENDER_TEXT_LENGTH) : str;
            }
        }

        private abstract class ItemInfoWindow : BaseWindow
        {
            protected List<ConsoleItemInfo> infos;
            private float scrollRate;

            public override void Start(ConsoleView view)
            {
                base.Start(view);
                infos = new List<ConsoleItemInfo>();
                OnStart();

                view.DeviceList.SetProvider(new WidgetProvider());
                view.DeviceList.stretchItem = true;
                view.DeviceList.itemPath = "InfoItem";
            }

            protected abstract void OnStart();

            public override void OnEnable()
            {
                base.OnEnable();
                View.DeviceList.gameObject.SetActive(true);
                View.DeviceList.SizeHandler = OnDeviceItemSize;
                View.DeviceList.RenderHandler = OnDeviceItemRender;
                View.DeviceList.NumItems = infos.Count;
                View.DeviceList.ScrollToRate(scrollRate);
            }

            public override void OnDisable()
            {
                base.OnDisable();
                View.DeviceList.gameObject.SetActive(false);
                scrollRate = View.DeviceList.ScrollRate;
            }

            public override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                for (int i = 0; i < infos.Count; i++)
                {
                    if (infos[i].OnRefresh(deltaTime))
                    {
                        View.DeviceList.RefreshItemSize(i);
                        View.DeviceList.RefreshItemRender(i);
                    }
                }
            }

            private Vector2 OnDeviceItemSize(int index, GameObject go)
            {
                RectTransform rect = go.transform as RectTransform;
                float height = View.ToolbarHeight;
                if (infos[index].expanded)
                {
                    var text = go.transform.Find("Text").GetComponent<Text>();
                    text.text = GetRenderString(infos[index].Text);
                    height = height + text.preferredHeight + 40;
                }

                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                return rect.rect.size;
            }

            private void OnDeviceItemRender(int index, GameObject go)
            {
                ConsoleItemInfo info = infos[index];

                var button = go.transform.Find("Button").GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => { OnClickExpandedItem(index); });
                var text = button.transform.Find("Text").GetComponent<Text>();
                text.text = info.title;
                text = go.transform.Find("Text").GetComponent<Text>();
                var copyButton = go.transform.Find("CopyButton").GetComponent<Button>();
                if (info.expanded)
                {
                    text.text = GetRenderString(infos[index].Text);
                    copyButton.onClick.RemoveAllListeners();
                    copyButton.onClick.AddListener(() => { OnClickCopyItemText(index); });
                }

                button.targetGraphic.color =
                    info.expanded ? ConsoleWidgetCreator.ExpandedColor : ConsoleWidgetCreator.ShowyColor;
                text.gameObject.SetActive(info.expanded);
                copyButton.gameObject.SetActive(info.expanded);
            }

            private void OnClickExpandedItem(int index)
            {
                infos[index].expanded = !infos[index].expanded;
                View.DeviceList.RefreshItemSize(index);
                View.DeviceList.RefreshItemRender(index);
            }

            private void OnClickCopyItemText(int index)
            {
                GUIUtility.systemCopyBuffer = infos[index].ToString();
            }
        }
    }
}