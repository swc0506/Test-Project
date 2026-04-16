using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Log
{
    internal static class ConsoleWidgetCreator
    {
        private const float kWidth = 160f;
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        private static readonly Vector2 ThickElementSize = new Vector2(kWidth, kThickHeight);
        private static readonly Vector2 ThinElementSize = new Vector2(kWidth, kThinHeight);
        private static readonly Vector2 ImageElementSize = new Vector2(100f, 100f);
        private static readonly Color DefaultTextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
        private static readonly Color DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static readonly Color PanelColor = new Color(1f, 1f, 1f, 0.392f);

        public static readonly Color DarkColor = new Color(45 / 255f, 45 / 255f, 45 / 255f, 255 / 255f);
        public static readonly Color ShowyColor = new Color(75 / 255f, 75 / 255f, 75 / 255f, 255 / 255f);
        public static readonly Color ExpandedColor = new Color(75 / 255f, 105 / 255f, 135 / 255f, 255 / 255f);
        public static readonly Color TextColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
        public static readonly Color ToggleColor = new Color(80 / 255f, 80 / 255f, 80 / 255f, 255 / 255f);
        public static readonly Color SuggestColor = new Color(0 / 255f, 190 / 255f, 20 / 255f, 200 / 255f);


        private static Dictionary<string, Sprite> iconMap;

        private static Sprite resStandard
        {
            get { return LoadSprite("BackgroundSliced"); }
        }

        private static Sprite resBackground
        {
            get { return LoadSprite("BackgroundSliced"); }
        }

        private static Sprite resInputField
        {
            get { return LoadSprite("BackgroundSliced"); }
        }


        private static Font defFont;

        public static Font DefFont
        {
            get
            {
                if (null == defFont)
                {
                    defFont = GetBuiltinFont("Arial.ttf");
                    if (null == defFont)
                    {
                        defFont = GetBuiltinFont("LegacyRuntime.ttf");
                    }
                }

                return defFont;
            }
        }

        private static Font GetBuiltinFont(string name)
        {
            try
            {
                var font = Resources.GetBuiltinResource<Font>(name);
                return font;
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public static Sprite LoadSprite(string name)
        {
            if (null == iconMap)
            {
                iconMap = new Dictionary<string, Sprite>();
                var icons = Resources.LoadAll<Sprite>("Log/Icons");
                foreach (var item in icons)
                {
                    iconMap.Add(item.name, item);
                }
            }

            if (iconMap.TryGetValue(name, out var sprite))
            {
                return sprite;
            }

            return null;
//            return Resources.Load<Sprite>(string.Format("Log/Images/{0}", name));
        }

        private static GameObject CreateUIElementRoot(string name, Vector2 size, params Type[] components)
        {
            GameObject child = new GameObject(name, components);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        private static GameObject CreateUIObject(string name, GameObject parent, params Type[] components)
        {
            GameObject go = new GameObject(name, components);
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;
            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        public static Text CreateText()
        {
            GameObject go = CreateUIElementRoot("Text", ThickElementSize, typeof(Text));
            Text text = go.GetComponent<Text>();
            text.color = DefaultTextColor;
            text.font = DefFont;

            text.raycastTarget = false;
            text.text = string.Empty;
            return text;
        }

        public static Image CreateImage()
        {
            GameObject go = CreateUIElementRoot("Image", ImageElementSize, typeof(Image));
            var image = go.GetComponent<Image>();
            image.sprite = resBackground;
            image.raycastTarget = false;
            return image;
        }

        public static Button CreateButton()
        {
            GameObject go = CreateUIElementRoot("Button", ThickElementSize, typeof(Image), typeof(Button));

            GameObject childText = CreateUIObject("Text", go, typeof(Text));

            Image image = go.GetComponent<Image>();
            image.sprite = resStandard;
            image.type = Image.Type.Sliced;
            image.color = DefaultSelectableColor;

            Button bt = go.GetComponent<Button>();
            SetDefaultColorTransitionValues(bt);

            Text text = childText.GetComponent<Text>();
            text.text = "Button";
            text.alignment = TextAnchor.MiddleCenter;
            text.color = DefaultTextColor;
            text.font = DefFont;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            return bt;
        }

        public static InputField CreateInputField()
        {
            GameObject root = CreateUIElementRoot("InputField", ThickElementSize, typeof(Image), typeof(InputField));

            GameObject childPlaceholder = CreateUIObject("Placeholder", root, typeof(Text));
            GameObject childText = CreateUIObject("Text", root, typeof(Text));

            Image image = root.GetComponent<Image>();
            image.sprite = resInputField;
            image.type = Image.Type.Sliced;
            image.color = DefaultSelectableColor;

            InputField inputField = root.GetComponent<InputField>();
            SetDefaultColorTransitionValues(inputField);

            Text text = childText.GetComponent<Text>();
            text.text = "";
            text.supportRichText = false;
            text.color = DefaultTextColor;
            text.font = DefFont;

            Text placeholder = childPlaceholder.GetComponent<Text>();
            placeholder.text = "Enter text...";
            placeholder.fontStyle = FontStyle.Italic;
            placeholder.font = DefFont;
            // Make placeholder color half as opaque as normal text color.
            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = new Vector2(10, 6);
            textRectTransform.offsetMax = new Vector2(-10, -7);

            RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;
            placeholderRectTransform.offsetMin = new Vector2(10, 6);
            placeholderRectTransform.offsetMax = new Vector2(-10, -7);

            inputField.textComponent = text;
            inputField.placeholder = placeholder;

            return inputField;
        }

        private static GameObject CreateScrollbar()
        {
            // Create GOs Hierarchy
            GameObject scrollbarRoot =
                CreateUIElementRoot("Scrollbar", ThinElementSize, typeof(Image), typeof(Scrollbar));

            GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", sliderArea, typeof(Image));

            Image bgImage = scrollbarRoot.GetComponent<Image>();
            bgImage.sprite = resBackground;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = DefaultSelectableColor;

            Image handleImage = handle.GetComponent<Image>();
            handleImage.sprite = resStandard;
            handleImage.type = Image.Type.Sliced;
            handleImage.color = DefaultSelectableColor;

            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            Scrollbar scrollbar = scrollbarRoot.GetComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            SetDefaultColorTransitionValues(scrollbar);

            return scrollbarRoot;
        }

        public static ScrollRect CreateScrollView()
        {
            GameObject root =
                CreateUIElementRoot("Scroll View", new Vector2(200, 200), typeof(Image), typeof(ScrollRect));

            GameObject viewport = CreateUIObject("Viewport", root, typeof(Image), typeof(Mask));
            GameObject content = CreateUIObject("Content", viewport, typeof(RectTransform));

            // Sub controls.

            GameObject hScrollbar = CreateScrollbar();
            hScrollbar.name = "Scrollbar Horizontal";
            SetParentAndAlign(hScrollbar, root);
            RectTransform hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.anchorMin = Vector2.zero;
            hScrollbarRT.anchorMax = Vector2.right;
            hScrollbarRT.pivot = Vector2.zero;
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            GameObject vScrollbar = CreateScrollbar();
            vScrollbar.name = "Scrollbar Vertical";
            SetParentAndAlign(vScrollbar, root);
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup RectTransforms.

            // Make viewport fill entire scroll view.
            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;

            // Make context match viewpoprt width and be somewhat taller.
            // This will show the vertical scrollbar and not the horizontal one.
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;

            // Setup UI components.

            ScrollRect scrollRect = root.GetComponent<ScrollRect>();
            scrollRect.content = contentRT;
            scrollRect.viewport = viewportRT;
            scrollRect.horizontalScrollbar = hScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;
            scrollRect.verticalScrollbarSpacing = -3;

            Image rootImage = root.GetComponent<Image>();
            rootImage.sprite = resBackground;
            rootImage.type = Image.Type.Sliced;
            rootImage.color = PanelColor;

            Mask viewportMask = viewport.GetComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            Image viewportImage = viewport.GetComponent<Image>();
            viewportImage.sprite = null;
            viewportImage.type = Image.Type.Sliced;

            return scrollRect;
        }
    }
}