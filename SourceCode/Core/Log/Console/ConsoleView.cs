using Core.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Log
{
    internal class ConsoleView
    {
        private static Vector2 Resolution
        {
            get
            {
                if (Screen.width >= Screen.height)
                {
                    return new Vector2(1334, 750);
                }
                else
                {
                    return new Vector2(450, 800);
                }
            }
        }

        private const float ICON_SIZE = 32;
        private const float LOG_MIN_WIDTH = 60;
        private const float MIN_SEARCH_WIDTH = 130;

        public readonly float MinRate = 0.2f;
        public readonly float ToolbarHeight = 36;

        private GameObject eventSys;
        private RectTransform scrollRect;
        private RectTransform toolbarRect1;
        private RectTransform toolbarRect2;
        private ScrollRect logLevelScroll;
        private LayoutElement searchElement;
        private Vector2 panelSize;
        private Vector2 logLevelViewportSize;
        private Vector2 tipContentSize;
        private ScreenOrientation prevScreenOrientation;
        private bool initialNotch;

        public CanvasGroup Canvas { get; private set; }
        public RectTransform RootRect { get; private set; }

        public Text FPSText { get; private set; }
        public GameObject InfoPanel { get; private set; }
        public RectTransform PanelRect { get; private set; }
        public Button ClearButton { get; private set; }
        public Button CollapseButton { get; private set; }
        public Button CloseButton { get; private set; }

        public Button DebugButton { get; private set; }
        public Button InfoButton { get; private set; }
        public Button WarnButton { get; private set; }
        public Button ErrorButton { get; private set; }
        public Button FatalButton { get; private set; }

        public InputField SearchInput { get; private set; }
        public Button SaveButton { get; private set; }
        public Button DeviceButton { get; private set; }
        public Button UserButton { get; private set; }
        public UIScrollList LogList { get; private set; }
        public UIScrollList SuggestList { get; private set; }
        public UIScrollList DeviceList { get; private set; }
        public InputField CommandInput { get; private set; }
        public Button ResizeButton { get; private set; }

        public void Initial()
        {
            CreateCanvas();
            TryCreateEventSystem();
            CreatePanel();
            CreateFPSText();
        }

        public void Update()
        {
            //自动调整对应布局
            if (panelSize != PanelRect.rect.size)
            {
                LimitPanelSize();
                if (panelSize.x != PanelRect.rect.size.x)
                {
                    RevertToolbarRect1Elements();
                }

                panelSize = PanelRect.rect.size;
                tipContentSize = Vector2.zero;
            }

            if (((RectTransform)SearchInput.transform).rect.size.x < MIN_SEARCH_WIDTH &&
                SearchInput.transform.parent == toolbarRect1)
            {
                AdjustToolbarRect2Elements();
            }

            if (logLevelViewportSize != logLevelScroll.viewport.rect.size)
            {
                logLevelViewportSize = logLevelScroll.viewport.rect.size;
                float minWidth = LOG_MIN_WIDTH * logLevelScroll.content.childCount;
                float contentWidth = logLevelViewportSize.x < minWidth ? minWidth : logLevelViewportSize.x;
                logLevelScroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);
                logLevelScroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, logLevelViewportSize.y);
            }

            if (tipContentSize != SuggestList.scrollRect.content.rect.size)
            {
                tipContentSize = SuggestList.scrollRect.content.rect.size;
                float delta = scrollRect.rect.size.y - tipContentSize.y;
                float offset = delta > 0 ? -delta : 0;
                SuggestList.rectTransform.offsetMax = new Vector2(0, offset);
            }

            MatchNotchScreen();
        }

        private void MatchNotchScreen()
        {
            if (!initialNotch || Screen.orientation != prevScreenOrientation)
            {
                initialNotch = true;
                prevScreenOrientation = Screen.orientation;
                Rect safeArea = Screen.safeArea;

                Vector2 uiSize = ((RectTransform)Canvas.transform).rect.size;
                if (Screen.width >= Screen.height)
                {
                    float navX1 = safeArea.x / Screen.width * uiSize.x * 0.5f;
                    float navX2 = (Screen.width - (safeArea.width + safeArea.x)) / Screen.width * uiSize.x * 0.5f;
                    RootRect.offsetMin = new Vector2(navX1, 0);
                    RootRect.offsetMax = new Vector2(-navX2, 0);
                }
                else
                {
                    float navX1 = safeArea.y / Screen.height * uiSize.y * 0.5f;
                    float navX2 = (Screen.height - (safeArea.height + safeArea.y)) / Screen.height * uiSize.y * 0.5f;
                    RootRect.offsetMin = new Vector2(0, navX1);
                    RootRect.offsetMax = new Vector2(0, -navX2);
                }
            }
        }

        private void LimitPanelSize()
        {
            Vector2 maxSize = ((RectTransform)Canvas.transform).rect.size;
            Vector2 currSize = PanelRect.rect.size;
            if (currSize.y > maxSize.y)
            {
                PanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize.y);
            }
            else if (currSize.y < maxSize.y * MinRate)
            {
                PanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize.y * MinRate);
            }
        }

        private void RevertToolbarRect1Elements()
        {
            searchElement.flexibleWidth = 5;
            SearchInput.transform.SetParent(toolbarRect1, false);
            SaveButton.transform.SetParent(toolbarRect1, false);
            DeviceButton.transform.SetParent(toolbarRect1, false);
            UserButton.transform.SetParent(toolbarRect1, false);
            scrollRect.offsetMax = new Vector2(0, -ToolbarHeight);
            CloseButton.transform.SetAsLastSibling();
            toolbarRect2.gameObject.SetActive(false);
        }

        private void AdjustToolbarRect2Elements()
        {
            toolbarRect2.gameObject.SetActive(true);
            searchElement.flexibleWidth = 6;
            SearchInput.transform.SetParent(toolbarRect2, false);
            SaveButton.transform.SetParent(toolbarRect2, false);
            DeviceButton.transform.SetParent(toolbarRect2, false);
            UserButton.transform.SetParent(toolbarRect2, false);
            scrollRect.offsetMax = new Vector2(0, -ToolbarHeight * 2);
        }

        #region CreatePanel

        private void SetColor(GameObject go, Color color)
        {
            Graphic ui = go.GetComponent<Graphic>();
            ui.color = color;
        }

        private void SetSprite(Image img, string name)
        {
            img.sprite = ConsoleWidgetCreator.LoadSprite(name);
            img.raycastTarget = false;
            img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ICON_SIZE);
            img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ICON_SIZE);
        }

        private RectTransform StretchRectBoth(Transform transform)
        {
            RectTransform rectTrans = transform as RectTransform;
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.sizeDelta = Vector2.zero;
            rectTrans.anchoredPosition = Vector2.zero;
            return rectTrans;
        }

        private GameObject CreateUINode(string name)
        {
            GameObject go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");
            return go;
        }

        private void CreateCanvas()
        {
            var gameObject = CreateUINode("Canvas");
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            GameObject.DontDestroyOnLoad(gameObject);

            Canvas = gameObject.AddComponent<CanvasGroup>();

            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = byte.MaxValue - 1;

            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = Resolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gameObject.AddComponent<GraphicRaycaster>();

            RootRect = CreateRectTransform(gameObject.transform);
        }

        private void TryCreateEventSystem()
        {
            if (null == GameObject.FindObjectOfType<EventSystem>())
            {
                eventSys = CreateUINode(typeof(EventSystem).Name);
                eventSys.AddComponent<EventSystem>();
                eventSys.AddComponent<StandaloneInputModule>();
            }
        }

        private RectTransform CreateRectTransform(Transform parent)
        {
            RectTransform rectTrans = new GameObject().AddComponent<RectTransform>();
            rectTrans.SetParent(parent);
            StretchRectBoth(rectTrans);
            return rectTrans;
        }

        private void CreateFPSText()
        {
            var text = CreateText(RootRect.transform);
            RectTransform rectTrans = text.rectTransform;
            rectTrans.sizeDelta = new Vector2(100, 42);
            text.fontSize = 24;
            text.color = Color.white;
            FPSText = text;
        }

        private void CreatePanel()
        {
            var image = ConsoleWidgetCreator.CreateImage();
            image.transform.SetParent(RootRect, false);
            InfoPanel = image.gameObject;
            PanelRect = image.rectTransform;
            image.type = Image.Type.Sliced;
            SetColor(InfoPanel, new Color32(56, 56, 56, 200));
            StretchRectBoth(PanelRect);
            image.rectTransform.pivot = Vector2.up;
            PanelRect.anchorMin = new Vector2(0, 0.5f);
            PanelRect.sizeDelta = new Vector2(PanelRect.sizeDelta.x, 0);

            CreateTopbar();

            scrollRect = CreateRectTransform(PanelRect);
            StretchRectBoth(scrollRect.transform);
            scrollRect.offsetMin = new Vector2(0, ToolbarHeight);
            scrollRect.offsetMax = new Vector2(0, -ToolbarHeight);

            var scroll = CreateScrollView(scrollRect, true, true);
            StretchRectBoth(scroll.transform);
            LogList = scroll.gameObject.AddComponent<UIScrollList>();
            LogList.Initial();

            scroll = CreateScrollView(scrollRect, true, true);
            StretchRectBoth(scroll.transform);
            DeviceList = scroll.gameObject.AddComponent<UIScrollList>();
            DeviceList.Initial();

            scroll = CreateScrollView(scrollRect, true, true);
            StretchRectBoth(scroll.transform);
            SuggestList = scroll.gameObject.AddComponent<UIScrollList>();
            SuggestList.miniPadding = new Vector2(5, 5);
            SuggestList.Initial();
            var color = ConsoleWidgetCreator.SuggestColor;
            color.a = 150 / 255f;
            SetColor(scroll.gameObject, color);

            CreateBottomBar();
        }

        private Text CreateText(Transform parent)
        {
            var text = ConsoleWidgetCreator.CreateText();
            text.rectTransform.SetParent(parent, false);
            text.rectTransform.anchorMin = Vector2.up;
            text.rectTransform.anchorMax = Vector2.up;
            text.rectTransform.pivot = Vector2.up;
            text.rectTransform.sizeDelta = Vector2.zero;
            text.rectTransform.anchoredPosition = Vector2.zero;
            text.color = ConsoleWidgetCreator.TextColor;
            return text;
        }

        private Button CreateButton(RectTransform parent, string sprite, bool hasLabel)
        {
            var button = ConsoleWidgetCreator.CreateButton();
            button.transform.SetParent(parent, false);
            SetColor(button.gameObject, ConsoleWidgetCreator.DarkColor);

            var image = ConsoleWidgetCreator.CreateImage();
            image.transform.SetParent(button.transform, false);
            SetSprite(image, sprite);

            if (!hasLabel)
            {
                GameObject.Destroy(button.transform.Find("Text").gameObject);
            }

            return button;
        }

        private Button CreateLogButton(RectTransform parent, string sprite)
        {
            var toggle = CreateButton(parent, sprite, true);
            var ele = toggle.gameObject.AddComponent<LayoutElement>();
            ele.minWidth = LOG_MIN_WIDTH;
            var image = toggle.transform.Find("Image").GetComponent<Image>();
            image.rectTransform.anchorMin = image.rectTransform.anchorMax = new Vector2(0.3f, 0.5f);
            var text = toggle.transform.Find("Text").GetComponent<Text>();
            StretchRectBoth(text.rectTransform);
            text.rectTransform.anchorMin = new Vector2(0.6f, 0f);
            text.rectTransform.SetAsLastSibling();
            text.alignment = TextAnchor.MiddleLeft;
            text.color = ConsoleWidgetCreator.TextColor;
            text.raycastTarget = false;
            text.text = "0";
            return toggle;
        }

        private InputField CreateInputField(Transform parent, string placeholder)
        {
            var input = ConsoleWidgetCreator.CreateInputField();
            SetColor(input.gameObject, ConsoleWidgetCreator.DarkColor);
            input.transform.SetParent(parent, false);
            StretchRectBoth(input.transform);

            var placeText = input.placeholder as Text;
            placeText.text = placeholder;
            placeText.font = input.textComponent.font;
            placeText.alignment = input.textComponent.alignment = TextAnchor.MiddleLeft;
            placeText.horizontalOverflow = input.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            input.placeholder.color = new Color(150 / 255f, 150 / 255f, 150 / 255f, 255 / 255f);
            input.textComponent.color = ConsoleWidgetCreator.TextColor;
            return input;
        }

        private ScrollRect CreateScrollView(Transform parent, bool isVertical, bool needScrollbar)
        {
            var scroll = ConsoleWidgetCreator.CreateScrollView();
            scroll.vertical = isVertical;
            scroll.horizontal = !isVertical;
            //replace RectMask2D
            var mask = scroll.viewport.GetComponent<Mask>();
            if (null != mask)
            {
                GameObject.Destroy(mask.graphic);
                GameObject.Destroy(mask.GetComponent<CanvasRenderer>());
                GameObject.Destroy(mask);
                scroll.viewport.gameObject.AddComponent<RectMask2D>();
            }

            if (isVertical)
            {
                GameObject.Destroy(scroll.horizontalScrollbar.gameObject);
                scroll.horizontalScrollbar = null;
                if (!needScrollbar)
                {
                    GameObject.Destroy(scroll.verticalScrollbar.gameObject);
                    scroll.verticalScrollbar = null;
                }
                else
                {
                    SetColor(scroll.verticalScrollbar.gameObject, ConsoleWidgetCreator.DarkColor);
                    SetColor(scroll.verticalScrollbar.handleRect.gameObject, ConsoleWidgetCreator.ShowyColor);
                }
            }
            else
            {
                GameObject.Destroy(scroll.verticalScrollbar.gameObject);
                scroll.verticalScrollbar = null;
                if (!needScrollbar)
                {
                    GameObject.Destroy(scroll.horizontalScrollbar.gameObject);
                    scroll.horizontalScrollbar = null;
                }
            }

            var scrollRect = scroll.transform as RectTransform;
            scrollRect.SetParent(parent, false);
            SetColor(scrollRect.gameObject, ConsoleWidgetCreator.DarkColor);

            return scroll;
        }

        private RectTransform CreateToolbarRect(Vector2 min, Vector2 max)
        {
            var barRect = CreateRectTransform(PanelRect);
            barRect.pivot = Vector2.up;
            barRect.anchorMin = min;
            barRect.anchorMax = max;
            barRect.sizeDelta = new Vector2(barRect.sizeDelta.x, ToolbarHeight);
            return barRect;
        }

        private void CreateTopbar()
        {
            toolbarRect1 = CreateToolbarRect(Vector2.up, Vector2.one);
            toolbarRect1.gameObject.AddComponent<HorizontalLayoutGroup>();

            toolbarRect2 = CreateToolbarRect(Vector2.up, Vector2.one);
            toolbarRect2.gameObject.AddComponent<HorizontalLayoutGroup>();
            Vector2 pos = toolbarRect2.anchoredPosition;
            pos.y = -ToolbarHeight;
            toolbarRect2.anchoredPosition = pos;

            ClearButton = CreateButton(toolbarRect1, "IconClear", false);
            CollapseButton = CreateButton(toolbarRect1, "IconCollapse", false);
            CloseButton = CreateButton(toolbarRect1, "IconHide", false);
            CreateLogToggles(toolbarRect1);

            CreateSearchInput();
            SaveButton = CreateButton(toolbarRect1, "IconSave", false);
            DeviceButton = CreateButton(toolbarRect1, "IconDevice", false);
            UserButton = CreateButton(toolbarRect1, "IconUser", false);
        }

        private void CreateSearchInput()
        {
            SearchInput = CreateInputField(toolbarRect1, "Search...");
            SearchInput.placeholder.rectTransform.offsetMin =
                SearchInput.textComponent.rectTransform.offsetMin = new Vector2(36, 0);
            SearchInput.placeholder.rectTransform.offsetMax =
                SearchInput.textComponent.rectTransform.offsetMax = new Vector2(0, 0);

            searchElement = SearchInput.gameObject.AddComponent<LayoutElement>();
            searchElement.flexibleWidth = 5;

            var searchImg = ConsoleWidgetCreator.CreateImage();
            searchImg.transform.SetParent(SearchInput.transform, false);
            searchImg.rectTransform.pivot = searchImg.rectTransform.anchorMin =
                searchImg.rectTransform.anchorMax = new Vector2(0, 0.5f);
            SetSprite(searchImg, "IconSearch");
        }

        private void CreateLogToggles(Transform parent)
        {
            logLevelScroll = CreateScrollView(parent, false, false);
            var ele = logLevelScroll.gameObject.AddComponent<LayoutElement>();
            ele.flexibleWidth = 5;

            var layout = logLevelScroll.content.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = layout.childForceExpandHeight = true;
            logLevelScroll.content.anchorMin = logLevelScroll.content.anchorMax = Vector2.up;
            logLevelScroll.content.anchoredPosition = Vector2.zero;

            DebugButton = CreateLogButton(logLevelScroll.content, "IconDebug");
            InfoButton = CreateLogButton(logLevelScroll.content, "IconInfo");
            WarnButton = CreateLogButton(logLevelScroll.content, "IconWarn");
            ErrorButton = CreateLogButton(logLevelScroll.content, "IconError");
            FatalButton = CreateLogButton(logLevelScroll.content, "IconFatal");
        }

        private void CreateBottomBar()
        {
            var barRect = CreateToolbarRect(Vector2.zero, Vector2.right);
            Vector3 pos = barRect.anchoredPosition;
            pos.y = ToolbarHeight;
            barRect.anchoredPosition = pos;

            float resizeWidth = 64;
            CommandInput = CreateInputField(barRect, "help log list of commands");
            CommandInput.lineType = InputField.LineType.MultiLineNewline;
            var rect = CommandInput.transform as RectTransform;
            rect.offsetMax = new Vector2(-resizeWidth, 0);

            ResizeButton = CreateButton(barRect, "IconResize", false);
            rect = ResizeButton.transform as RectTransform;
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchorMin = Vector2.right;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = new Vector2(resizeWidth, 0);
        }

        #endregion

        public void Dispose()
        {
            if (null != eventSys)
            {
                GameObject.Destroy(eventSys);
                eventSys = null;
            }

            if (null != Canvas)
            {
                GameObject.Destroy(Canvas.gameObject);
                Canvas = null;
            }
        }
    }
}