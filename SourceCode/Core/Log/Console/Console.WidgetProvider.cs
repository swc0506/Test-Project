using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Log
{
    internal partial class ConsoleController
    {
        private class WidgetProvider : IGameObjectProvider
        {
            public GameObject Load(string path)
            {
                if (path == "LogItem")
                {
                    return CreateLogItem();
                }
                else if (path == "SuggestItem")
                {
                    return CreateSuggestItem();
                }
                else if (path == "InfoItem")
                {
                    return CreateInfoItem();
                }

                return null;
            }

            public int LoadAsync(string path, Action<string, GameObject> callback)
            {
                return 0;
            }

            public void CancelAsync(int id)
            {
            }

            private void StretchHorizontal(Transform transform)
            {
                RectTransform rect = transform as RectTransform;
                rect.pivot = Vector2.up;
                rect.anchorMin = Vector2.up;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }

            private Button CreateCopyButton(Transform parent, string title)
            {
                var copy = ConsoleWidgetCreator.CreateButton();
                copy.gameObject.name = "CopyButton";
                copy.GetComponent<Image>().color = ConsoleWidgetCreator.ToggleColor;
                copy.transform.SetParent(parent, false);
                var rect = copy.transform as RectTransform;
                rect.pivot = new Vector2(0.5f, 0);
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.right;
                rect.offsetMin = new Vector2(20, 8);
                rect.offsetMax = new Vector2(-20, 40);

                var text = copy.transform.Find("Text").GetComponent<Text>();
                text.text = title;
                text.color = ConsoleWidgetCreator.TextColor;
                text.raycastTarget = false;

                return copy;
            }

            private GameObject CreateLogItem()
            {
                var button = ConsoleWidgetCreator.CreateButton();
                StretchHorizontal(button.transform);

                var text = button.transform.Find("Text").GetComponent<Text>();
                text.color = ConsoleWidgetCreator.TextColor;
                text.alignment = TextAnchor.UpperLeft;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.raycastTarget = false;
                text.rectTransform.pivot = Vector2.up;
                text.rectTransform.offsetMin = new Vector2(36, 0);
                text.rectTransform.offsetMax = new Vector2(0, -10);

                var icon = ConsoleWidgetCreator.CreateImage();
                icon.gameObject.name = "IconImage";
                icon.raycastTarget = false;
                icon.rectTransform.SetParent(button.transform, false);
                icon.rectTransform.pivot = Vector2.up;
                icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = Vector2.up;
                icon.rectTransform.sizeDelta = new Vector2(32, 32);
                icon.rectTransform.anchoredPosition = Vector2.zero;

                var image = ConsoleWidgetCreator.CreateImage();
                image.gameObject.name = "CountImage";
                image.sprite = ConsoleWidgetCreator.LoadSprite("BackgroundSliced2");
                image.type = Image.Type.Sliced;
                image.color = ConsoleWidgetCreator.ToggleColor;
                image.raycastTarget = false;
                image.rectTransform.SetParent(button.transform, false);
                image.rectTransform.pivot = Vector2.one;
                image.rectTransform.anchorMin = image.rectTransform.anchorMax = Vector2.one;
                image.rectTransform.sizeDelta = new Vector2(42, 26);
                image.rectTransform.anchoredPosition = new Vector2(-10, -5);

                text = ConsoleWidgetCreator.CreateText();
                text.color = ConsoleWidgetCreator.TextColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.raycastTarget = false;
                text.rectTransform.SetParent(image.transform, false);
                text.rectTransform.anchorMin = Vector2.zero;
                text.rectTransform.anchorMax = Vector2.one;
                text.rectTransform.sizeDelta = Vector2.zero;

                CreateCopyButton(button.transform, "Copy Log");

                return button.gameObject;
            }

            private GameObject CreateSuggestItem()
            {
                var button = ConsoleWidgetCreator.CreateButton();
                RectTransform rect = button.transform as RectTransform;
                rect.pivot = Vector2.up;
                rect.anchorMin = rect.anchorMax = Vector2.up;
                var image = button.GetComponent<Image>();
                image.sprite = ConsoleWidgetCreator.LoadSprite("BackgroundSliced2");
                image.type = Image.Type.Sliced;
                image.color = ConsoleWidgetCreator.SuggestColor;

                var text = button.transform.Find("Text").GetComponent<Text>();
                text.color = ConsoleWidgetCreator.TextColor;
                text.fontStyle = FontStyle.Bold;
                text.alignment = TextAnchor.MiddleLeft;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
                text.raycastTarget = false;
                text.rectTransform.pivot = Vector2.up;
                text.rectTransform.anchoredPosition = Vector2.zero;
                text.rectTransform.offsetMin = new Vector2(5, 2);
                text.rectTransform.offsetMax = new Vector2(-5, -2);
                return button.gameObject;
            }

            private GameObject CreateInfoItem()
            {
                var bg = ConsoleWidgetCreator.CreateImage();
                bg.type = Image.Type.Sliced;
                bg.color = ConsoleWidgetCreator.DarkColor;
                StretchHorizontal(bg.transform);

                var button = ConsoleWidgetCreator.CreateButton();
                button.gameObject.name = "Button";
                button.transform.SetParent(bg.transform, false);
                button.GetComponent<Image>().color = ConsoleWidgetCreator.ShowyColor;
                StretchHorizontal(button.transform);
                RectTransform rect = button.transform as RectTransform;
                rect.offsetMin = new Vector2(0, -36);

                var text = button.transform.Find("Text").GetComponent<Text>();
                text.color = ConsoleWidgetCreator.TextColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontStyle = FontStyle.Bold;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.raycastTarget = false;

                text = ConsoleWidgetCreator.CreateText();
                text.gameObject.name = "Text";
                text.color = ConsoleWidgetCreator.TextColor;
                text.alignment = TextAnchor.UpperLeft;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
                text.raycastTarget = false;
                text.rectTransform.SetParent(bg.transform, false);
                text.rectTransform.pivot = Vector2.up;
                text.rectTransform.anchorMin = Vector2.zero;
                text.rectTransform.anchorMax = Vector2.one;
                text.rectTransform.offsetMin = new Vector2(5, 0);
                text.rectTransform.offsetMax = new Vector2(-5, -42);

                CreateCopyButton(bg.transform, "Copy Text");

                return bg.gameObject;
            }

            public void Destroy(GameObject go)
            {
                GameObject.Destroy(go);
            }

            public void Dispose()
            {
            }
        }
    }
}