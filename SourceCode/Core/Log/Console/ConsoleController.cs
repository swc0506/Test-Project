using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Log
{
    internal sealed partial class ConsoleController
    {
        private enum WindowType
        {
            Log,
            Device,
            Custom
        }

        public ISwitchTrigger trigger;
        public bool errorAutomaticOpen;
        private ConsoleView view;
        private bool visible;
        private bool hasError;
        public Commander commander;
        public event Action<bool> SwitchEvent;

        private StringBuilder strBuilder;
        private List<CommandInfo> suggestInfos;
        private string prevCommandText;
        private CircularBuffer<string> historyCommands;
        private int historyIndex;
        private int caretCommandCount;
        private const int MaxDisplayLength = 20;
        private const string PrefsHistory = "ConsoleController.PrefsHistory";

        private Dictionary<int, BaseWindow> windowMap;
        private WindowType focusWindow;
        private int totalFrame;

        private const float SAMPLE_TIME = 1f;
        private int elapseFrame;
        private float elapseTime;

        public bool Visible
        {
            get { return visible; }
        }

        private Dictionary<int, string> fpsMap = new Dictionary<int, string>();

        internal ConsoleController(int maxLogNum)
        {
            var logWindow = new LogWindow(maxLogNum);
            windowMap = new Dictionary<int, BaseWindow>
            {
                { (int)WindowType.Log, logWindow },
                { (int)WindowType.Device, new DeviceWindow() },
                { (int)WindowType.Custom, new CustomWindow() }
            };
            commander = new Commander(logWindow.SystemPrint);
            commander.AddCommand("help", "debug list of commands", LogCommands);
            Application.logMessageReceivedThreaded += OnThreadReceivedLog;

            for (int i = 10; i <= 60; i++)
            {
                fpsMap.Add(i, CreateFPSText(i));
            }
        }


        public void Start()
        {
            if (view != null)
            {
                return;
            }

            visible = true;
            trigger = null == trigger ? new DefaultSwitchTrigger() : trigger;
            CreateView(null);
            foreach (var item in windowMap)
            {
                item.Value.Start(view);
                item.Value.OnDisable();
            }

            SetFocusView(WindowType.Log);
            SetVisible(false);
        }

        public void Update()
        {
            totalFrame++;
            if (null != trigger && trigger.Trigger())
            {
                SetVisible(!visible);
            }

            if (hasError)
            {
                SetVisible(true);
                hasError = false;
            }

            if (visible)
            {
                view.Update();
                windowMap[(int)focusWindow].OnUpdate(Time.deltaTime);
                if (view.CommandInput.isFocused)
                {
                    CheckCommandInputKey();
                }
            }
            else
            {
                if (totalFrame == 1)
                {
                    view.Update();
                }
            }

            RefreshFPS();
        }

        private void RefreshFPS()
        {
            elapseFrame += 1;
            elapseTime += Time.unscaledDeltaTime;
            if (elapseTime >= SAMPLE_TIME)
            {
                int fps = Mathf.RoundToInt(elapseFrame / elapseTime);
                view.FPSText.text = GetFPSText(fps);
                elapseTime = 0;
                elapseFrame = 0;
            }
        }

        private string CreateFPSText(int fps)
        {
            return "FPS:" + fps;
        }

        private string CreateMSText(int fps)
        {
            return "MS:" + fps;
        }

        private string GetFPSText(int fps)
        {
            string text = string.Empty;
            if (!fpsMap.TryGetValue(fps, out text))
            {
                text = CreateFPSText(fps);
                fpsMap.Add(fps, text);
            }

            return text;
        }

        private void OnThreadReceivedLog(string condition, string stackTrace, LogType type)
        {
            if (errorAutomaticOpen && (type == LogType.Error || type == LogType.Exception))
            {
                hasError = true;
            }

            ((LogWindow)windowMap[(int)WindowType.Log]).ThreadPushLog(condition, stackTrace, type);
        }

        private void SetVisible(bool visible)
        {
            if (this.visible == visible)
            {
                return;
            }

            this.visible = visible;
            if (!this.visible)
            {
                view.CommandInput.text = string.Empty;
                view.CommandInput.DeactivateInputField();
            }

            view.Canvas.blocksRaycasts = this.visible;
            view.InfoPanel.SetActive(this.visible);

            SwitchEvent?.Invoke(this.visible);
        }

        private void CreateView(Transform viewParent)
        {
            view = new ConsoleView();
            view.Initial();
            if (null != viewParent)
            {
                view.Canvas.transform.SetParent(viewParent);
            }

            var entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Drag, callback = new EventTrigger.TriggerEvent()
            };
            entry.callback.AddListener(OnDragResizeView);
            var eventTrigger = view.ResizeButton.gameObject.AddComponent<EventTrigger>();
            eventTrigger.triggers.Add(entry);
            view.CloseButton.onClick.AddListener(OnClickCloseConsole);
            view.DeviceButton.onClick.AddListener(OnToggleDeviceWindow);
            view.UserButton.onClick.AddListener(OnToggleUserWindow);

            strBuilder = new StringBuilder();
            suggestInfos = new List<CommandInfo>();
            historyCommands = new CircularBuffer<string>(10);
            historyIndex = -1;
            view.SuggestList.SetProvider(new WidgetProvider());
            view.SuggestList.itemPath = "SuggestItem";
            view.SuggestList.SizeHandler = OnSuggestItemSize;
            view.SuggestList.RenderHandler = OnSuggestItemRender;
            view.SuggestList.gameObject.SetActive(false);
            view.CommandInput.onValidateInput += OnValidateCommand;
            view.CommandInput.onValueChanged.AddListener(OnInputSuggestCommand);
            view.CommandInput.onEndEdit.AddListener(OnEndCommand);

            string historyText = PlayerPrefs.GetString(PrefsHistory);
            if (!string.IsNullOrEmpty(historyText))
            {
                string[] cmds = Regex.Split(historyText, "Cmd=");
                foreach (var item in cmds)
                {
                    historyCommands.Push(item);
                }
            }
        }

        private void OnDragResizeView(BaseEventData eventData)
        {
            Vector2 maxSize = ((RectTransform)view.Canvas.transform).rect.size;
            Vector2 currSize = view.PanelRect.rect.size;
            float delta = -((PointerEventData)eventData).delta.y;
            if (delta > 0)
            {
                float maxDelta = maxSize.y - currSize.y;
                if (maxDelta < 20)
                {
                    delta = maxDelta;
                }

                delta = delta > maxDelta ? maxDelta : delta;
            }

            else if (delta < 0)
            {
                float minDelta = maxSize.y * view.MinRate - currSize.y;
                if (minDelta > -20)
                {
                    delta = minDelta;
                }

                delta = delta < minDelta ? minDelta : delta;
            }

            view.PanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currSize.y + delta);
        }

        private void OnClickCloseConsole()
        {
            SetVisible(false);
        }

        private void SetFocusView(WindowType focus)
        {
            if (windowMap.TryGetValue((int)focusWindow, out var window))
            {
                window?.OnDisable();
            }

            focusWindow = focus;
            windowMap[(int)focusWindow].OnEnable();
            view.DeviceButton.targetGraphic.color = focusWindow == WindowType.Device
                ? ConsoleWidgetCreator.ToggleColor
                : ConsoleWidgetCreator.DarkColor;
            view.UserButton.targetGraphic.color = focusWindow == WindowType.Custom
                ? ConsoleWidgetCreator.ToggleColor
                : ConsoleWidgetCreator.DarkColor;
        }

        private void OnToggleDeviceWindow()
        {
            if (focusWindow == WindowType.Device)
            {
                SetFocusView(WindowType.Log);
            }
            else
            {
                SetFocusView(WindowType.Device);
            }
        }

        private void OnToggleUserWindow()
        {
            if (focusWindow == WindowType.Custom)
            {
                SetFocusView(WindowType.Log);
            }
            else
            {
                SetFocusView(WindowType.Custom);
            }
        }

        public IItemInfoDisplayable GetCustomDisplay()
        {
            CustomWindow window = windowMap[(int)WindowType.Custom] as CustomWindow;
            return window;
        }

        #region Command

        private void LogCommands()
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in commander.Infos)
            {
                strBuilder.AppendFormat("{0}   {1}", item.command, item.description);
                strBuilder.AppendLine();
            }

            ((LogWindow)windowMap[(int)WindowType.Log]).SystemPrint(strBuilder.ToString());
        }

        private void CheckCommandInputKey()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (--historyIndex < 0)
                {
                    historyIndex = historyCommands.Count - 1;
                }

                SetHistoryCommand();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (++historyIndex >= historyCommands.Count)
                {
                    historyIndex = 0;
                }

                SetHistoryCommand();
            }
        }

        private void SetHistoryCommand()
        {
            if (historyIndex >= 0 && historyIndex < historyCommands.Count)
            {
                SetCommandText(historyCommands[historyIndex]);
            }
        }


        private char OnValidateCommand(string text, int charIndex, char addedChar)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (addedChar == '\t')
                {
                    if (suggestInfos.Count > 0)
                    {
                        SetCommandText(suggestInfos[0].command);
                    }

                    return '\0';
                }
                else if (addedChar == '\n')
                {
                    ExecuteCommand(text);
                    return '\0';
                }
            }

            return addedChar;
        }

        private void SetCommandText(string text)
        {
            view.CommandInput.text = text;
            if (!string.IsNullOrEmpty(text))
            {
                view.CommandInput.caretPosition = text.Length;
            }
        }

        private void OnInputSuggestCommand(string text)
        {
            if (prevCommandText == text)
            {
                return;
            }

            prevCommandText = text;
            suggestInfos.Clear();
            if (string.IsNullOrEmpty(text))
            {
                UpdateSuggestCommandView();
                return;
            }

            caretCommandCount = 1;
            if (!commander.Splitter.TrySplitParameters(text, out var args))
            {
                return;
            }

            caretCommandCount = args.Count;
            if (commander.Splitter.IsDelimiter(text[text.Length - 1]))
            {
                caretCommandCount++;
            }

            foreach (var item in commander.Infos)
            {
                if (MatchInputText(item.command, text))
                {
                    suggestInfos.Add(item);
                    continue;
                }

                int parameters = args.Count - 1;
                if (item.command == args[0] && item.parameterTypes.Length >= parameters)
                {
                    bool matchArg = true;
                    for (int i = 0; i < parameters; i++)
                    {
                        if (!commander.Parser.TryParse(args[i + 1], item.parameterTypes[i], out var obj))
                        {
                            matchArg = false;
                            break;
                        }
                    }

                    if (matchArg)
                    {
                        suggestInfos.Add(item);
                    }
                }
            }

            UpdateSuggestCommandView();
        }

        private bool MatchInputText(string text, string inputText)
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                int index = 0;
                while (true)
                {
                    if (text.Length > index && inputText.Length > index)
                    {
                        if (!text[index].ToString()
                                .Equals(inputText[index].ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                    else if (index > text.Length && index > inputText.Length)
                    {
                        break;
                    }

                    index++;
                }

                if (inputText.Length > text.Length && !commander.Splitter.IsDelimiter(inputText[text.Length]))
                {
                    return false;
                }
            }

            return true;
        }

        private void OnEndCommand(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                suggestInfos.Clear();
                UpdateSuggestCommandView();
            }
        }

        private void UpdateSuggestCommandView()
        {
            bool active = suggestInfos.Count > 0;
            if (view.SuggestList.gameObject.activeSelf != active)
            {
                view.SuggestList.gameObject.SetActive(active);
                if (active)
                {
                    OnInputSuggestCommand(view.CommandInput.text);
                }
                else
                {
                    prevCommandText = null;
                }
            }

            view.SuggestList.NumItems = suggestInfos.Count;
        }

        private void ExecuteCommand(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            try
            {
                if (commander.ExecuteCommand(text))
                {
                    string info = string.Format("Execute command success:{0}", text);
                    ((LogWindow)windowMap[(int)WindowType.Log]).SystemPrint(info);

                    int length = historyCommands.Count;
                    if (length == 0 || (length >= 0 && historyCommands[length - 1] != text))
                    {
                        historyCommands.Push(text);
                        SaveHistoryCommands();
                    }

                    view.CommandInput.text = string.Empty;
                    OnEndCommand(string.Empty);
                }
            }
            catch (Exception e)
            {
                string error = string.Format("Execute command error:{0}", e.Message.ToString());
                ((LogWindow)windowMap[(int)WindowType.Log]).SystemPrint(error);
            }
        }

        private void SaveHistoryCommands()
        {
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < historyCommands.Count; i++)
            {
                if (i != 0)
                {
                    strBuilder.Append("Cmd=");
                }

                strBuilder.Append(historyCommands[i]);
            }

            PlayerPrefs.SetString(PrefsHistory, strBuilder.ToString());
        }

        private Vector2 OnSuggestItemSize(int index, GameObject go)
        {
            float width = view.SuggestList.scrollRect.viewport.rect.size.x;
            RectTransform rect = go.transform as RectTransform;
            var text = go.transform.Find("Text").GetComponent<Text>();
            text.rectTransform.sizeDelta = new Vector2(width, text.rectTransform.sizeDelta.y);
            text.text = GetRendererSuggestString(suggestInfos[index]);
            Vector2 size = new Vector2(text.preferredWidth + 20, text.preferredHeight + 20);
            rect.sizeDelta = size;
            return size;
        }

        private string GetRendererSuggestString(CommandInfo info)
        {
            strBuilder.Clear();
            strBuilder.AppendFormat(GetFocusCaretString(1), info.command);
            for (int i = 0; i < info.parameterNames.Length; i++)
            {
                strBuilder.Append(i == 0 ? " " : ",");
                string argsName = string.Format("{0} {1}", commander.Parser.GetParseTypeName(info.parameterTypes[i]),
                    info.parameterNames[i]);
                strBuilder.AppendFormat(GetFocusCaretString(i + 2), argsName);
            }

            string des = info.description;
            if (!string.IsNullOrEmpty(des))
            {
                if (des.Length > MaxDisplayLength)
                {
                    des = des.Substring(0, MaxDisplayLength) + "...";
                }

                strBuilder.AppendLine();
                strBuilder.Append(des);
            }

            return strBuilder.ToString();
        }

        private string GetFocusCaretString(int count)
        {
            return caretCommandCount == count ? "<color=#FFCA00>{0}</color>" : "{0}";
        }

        private void OnSuggestItemRender(int index, GameObject go)
        {
            var info = suggestInfos[index];
            var button = go.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { OnClickSetCommandText(index); });
            var text = button.transform.Find("Text").GetComponent<Text>();
            text.text = GetRendererSuggestString(info);
        }

        private void OnClickSetCommandText(int index)
        {
            if (index < 0 || index >= suggestInfos.Count)
            {
                return;
            }

            var info = suggestInfos[index];
            view.CommandInput.ActivateInputField();
            SetCommandText(info.command);
        }

        #endregion

        public void Dispose()
        {
            Application.logMessageReceivedThreaded -= OnThreadReceivedLog;
            view.Dispose();
            view = null;
        }
    }
}