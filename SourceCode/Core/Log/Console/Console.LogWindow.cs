using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Log
{
    internal partial class ConsoleController
    {
        private class LogWindow : BaseWindow
        {
            internal enum LogLevel
            {
                DEBUG,
                INFO,
                WARN,
                ERROR,
                FATAL,
                System
            }

            internal struct LogInfo : IEquatable<LogInfo>
            {
                public string dateTime;
                public string text;
                public string stackTrace;
                public LogLevel level;

                public LogInfo(string dateTime, string text, string stackTrace, LogLevel level)
                {
                    this.dateTime = dateTime;
                    this.text = text;
                    this.stackTrace = stackTrace;
                    this.level = level;
                }

                public bool Equals(LogInfo other)
                {
                    return text == other.text && stackTrace == other.stackTrace && level == other.level;
                }

                public override bool Equals(object obj)
                {
                    return obj is LogInfo other && Equals(other);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        var hashCode = (text != null ? text.GetHashCode() : 0);
                        hashCode = (hashCode * 397) ^ (stackTrace != null ? stackTrace.GetHashCode() : 0);
                        hashCode = (hashCode * 397) ^ (int) level;
                        return hashCode;
                    }
                }

                public override string ToString()
                {
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.AppendFormat("{0} {1}", dateTime, level.ToString());
                    strBuilder.AppendLine();
                    strBuilder.Append(text);
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        strBuilder.AppendLine();
                        strBuilder.Append(stackTrace);
                    }

                    return strBuilder.ToString();
                }
            }

            private class PrefsToggle
            {
                private readonly string key;
                private readonly Button button;
                public Text text;
                private readonly Action callback;

                private bool isOn;

                public bool IsOn
                {
                    get { return isOn; }
                }

                internal PrefsToggle(string key, Button button, bool defValue, Action callback)
                {
                    this.key = key;
                    this.button = button;
                    this.text = button.transform.Find("Text").GetComponent<Text>();
                    this.callback = callback;
                    int defInt = defValue ? 1 : 0;
                    isOn = PlayerPrefs.GetInt(key, defInt) == 1;
                    RefreshTargetColor();
                    button.onClick.AddListener(OnToggleChanged);
                }

                private void OnToggleChanged()
                {
                    isOn = !isOn;
                    PlayerPrefs.SetInt(key, isOn ? 1 : 0);
                    RefreshTargetColor();
                    callback?.Invoke();
                }

                private void RefreshTargetColor()
                {
                    button.targetGraphic.color =
                        IsOn ? ConsoleWidgetCreator.ToggleColor : ConsoleWidgetCreator.DarkColor;
                }
            }

            private readonly int maxLogNum;
            private string dateTimePattern;
            private string log4NetPattern;
            private ConcurrentQueue<LogInfo> threadLogs;
            private CircularBuffer<LogInfo> allLogs;
            private Dictionary<int, int> logCountMap;
            private CircularBuffer<LogInfo> viewLogs;
            private Dictionary<int, int> collapseMap;

            private Dictionary<int, string> logIconMap;
            private Dictionary<int, PrefsToggle> logToggleMap;
            private int focusExpandedIndex;
            private PrefsToggle collapse;
            private string filterText;
            private StringBuilder strBuilder;

            public LogWindow(int maxLogNum)
            {
                this.maxLogNum = maxLogNum;
            }

            public override void Start(ConsoleView view)
            {
                base.Start(view);

                //pattern log4net format
                dateTimePattern = @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{1,4}";
                log4NetPattern = string.Format("({0}) ({1}) -?\\s?", dateTimePattern,
                    string.Join("|", Enum.GetNames(typeof(LogLevel))));
                threadLogs = new ConcurrentQueue<LogInfo>();
                allLogs = new CircularBuffer<LogInfo>(maxLogNum);
                logCountMap = new Dictionary<int, int>();
                viewLogs = new CircularBuffer<LogInfo>(maxLogNum);
                collapseMap = new Dictionary<int, int>();

                logIconMap = new Dictionary<int, string>
                {
                    {(int) LogLevel.DEBUG, "IconDebug"},
                    {(int) LogLevel.INFO, "IconInfo"},
                    {(int) LogLevel.WARN, "IconWarn"},
                    {(int) LogLevel.ERROR, "IconError"},
                    {(int) LogLevel.FATAL, "IconFatal"},
                    {(int) LogLevel.System, "IconSystem"}
                };

                logToggleMap = new Dictionary<int, PrefsToggle>();
                AddLogPrefsToggle(LogLevel.DEBUG, view.DebugButton);
                AddLogPrefsToggle(LogLevel.INFO, view.InfoButton);
                AddLogPrefsToggle(LogLevel.WARN, view.WarnButton);
                AddLogPrefsToggle(LogLevel.ERROR, view.ErrorButton);
                AddLogPrefsToggle(LogLevel.FATAL, view.FatalButton);

                focusExpandedIndex = -1;
                collapse = new PrefsToggle("Console.CollapseToggle", view.CollapseButton, false, OnFilterLogs);
                strBuilder = new StringBuilder();

                view.ClearButton.onClick.AddListener(OnClickClearLogs);
                view.SaveButton.onClick.AddListener(OnClickSaveLogs);
                view.SearchInput.onValueChanged.AddListener(OnInputSearchLog);
                view.LogList.SetProvider(new WidgetProvider());
                view.LogList.stretchItem = true;
                view.LogList.itemPath = "LogItem";
                view.LogList.SizeHandler = OnLogItemSize;
                view.LogList.RenderHandler = OnLogItemRender;
            }

            public override void OnEnable()
            {
                base.OnEnable();
                View.LogList.gameObject.SetActive(true);
            }

            public override void OnDisable()
            {
                base.OnDisable();
                View.LogList.gameObject.SetActive(false);
            }

            public override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                while (threadLogs.TryDequeue(out var log))
                {
                    AppendLogInfo(log);
                }
            }

            private void AddLogPrefsToggle(LogLevel logLevel, Button toggle)
            {
                logToggleMap.Add((int) logLevel, new PrefsToggle("Console." + logLevel, toggle, true, OnFilterLogs));
            }

            public void SystemPrint(string content)
            {
                string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff");
                string str = string.Format("<color=#1afa29>{0}</color>", content);
                LogInfo info = new LogInfo(dateTime, str, null, LogLevel.System);
                bool isFull = viewLogs.IsFull;
                viewLogs.Push(info);
                RefreshLogList(isFull);
                ScrollToLastLog();
            }

            public void ThreadPushLog(string condition, string stackTrace, LogType type)
            {
                if (null == condition || null == log4NetPattern)
                {
                    return;
                }

                //regex log4Net log 
                Match match = Regex.Match(condition, log4NetPattern);
                string dateTime;
                LogLevel logLevel;
                if (match.Success)
                {
                    dateTime = match.Groups[1].Value;
                    Enum.TryParse(match.Groups[2].Value, out logLevel);
                    condition = Regex.Replace(condition, match.Groups[0].Value, string.Empty);
                }
                else
                {
                    dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff");
                    logLevel = ConvertLogLevel(type);
                }

                threadLogs.Enqueue(new LogInfo(dateTime, condition, stackTrace, logLevel));
            }

            private LogLevel ConvertLogLevel(LogType logType)
            {
                LogLevel logLevel;
                switch (logType)
                {
                    case LogType.Exception:
                        logLevel = LogLevel.FATAL;
                        break;
                    case LogType.Error:
                        logLevel = LogLevel.ERROR;
                        break;
                    case LogType.Warning:
                        logLevel = LogLevel.WARN;
                        break;
                    default:
                        logLevel = LogLevel.DEBUG;
                        break;
                }

                return logLevel;
            }

            private void AppendLogInfo(LogInfo log)
            {
                allLogs.Push(log);
                if (!logCountMap.ContainsKey((int) log.level))
                {
                    logCountMap.Add((int) log.level, 1);
                }
                else
                {
                    logCountMap[(int) log.level] += 1;
                }

                logToggleMap[(int) log.level].text.text = logCountMap[(int) log.level].ToString();

                bool isFull = viewLogs.IsFull;
                if (!TryFilterLog(log))
                {
                    RefreshLogList(isFull);
                    if (log.level == LogLevel.ERROR || log.level == LogLevel.FATAL)
                    {
                        focusExpandedIndex = -1;
                        int lastIndex = viewLogs.Count - 1;
                        OnClickExpandedItem(lastIndex);
                        View.LogList.ScrollToItem(lastIndex);
                    }
                }
            }

            private void RefreshLogList(bool isFull)
            {
                if (isFull)
                {
                    if (focusExpandedIndex == -1)
                    {
                        View.LogList.RefreshAllRender();
                    }
                    else
                    {
                        View.LogList.NumItems = viewLogs.Count;
                    }
                }
                else
                {
                    View.LogList.Append();
                }
            }

            private void ScrollToLastLog()
            {
                View.LogList.ScrollToItem(viewLogs.Count - 1);
            }

            private void OnClickSaveLogs()
            {
                StringBuilder strBuilder = new StringBuilder();
                foreach (var item in allLogs)
                {
                    strBuilder.AppendLine(item.ToString());
                }

                string path = Application.isEditor ? Directory.GetCurrentDirectory() : Application.temporaryCachePath;
                path = Path.Combine(path, "all.log");
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(strBuilder.ToString());
                        sw.Dispose();
                    }

                    SystemPrint(string.Format("Save Logs Success:{0}", path));
                }
                catch (Exception e)
                {
                    SystemPrint(string.Format("Save Logs Exception:{0}", e.Message));
                }
            }

            private void OnClickClearLogs()
            {
                focusExpandedIndex = -1;
                allLogs.Clear();
                logCountMap.Clear();
                viewLogs.Clear();
                if (collapse.IsOn)
                {
                    collapseMap.Clear();
                }

                View.LogList.NumItems = 0;
            }

            private void OnInputSearchLog(string text)
            {
                if (filterText != text)
                {
                    filterText = text;
                    OnFilterLogs();
                }
            }

            private Vector2 OnLogItemSize(int index, GameObject go)
            {
                RectTransform rect = go.transform as RectTransform;
                float height = View.ToolbarHeight;
                if (focusExpandedIndex == index)
                {
                    var text = go.transform.Find("Text").GetComponent<Text>();
                    text.horizontalOverflow = HorizontalWrapMode.Wrap;
                    text.text = GetRendererLogString(viewLogs[index], true, text.fontSize);
                    height = height + text.preferredHeight + 20;
                }

                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                return rect.rect.size;
            }

            private void OnLogItemRender(int index, GameObject go)
            {
                LogInfo info = viewLogs[index];
                bool expanded = focusExpandedIndex == index;
                var button = go.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => { OnClickExpandedItem(index); });
                var image = button.GetComponent<Image>();
                image.color = expanded ? ConsoleWidgetCreator.ExpandedColor :
                    index % 2 == 0 ? ConsoleWidgetCreator.DarkColor : ConsoleWidgetCreator.ShowyColor;
                image = button.transform.Find("IconImage").GetComponent<Image>();
                image.sprite = ConsoleWidgetCreator.LoadSprite(logIconMap[(int) info.level]);
                var copyButton = button.transform.Find("CopyButton").GetComponent<Button>();
                copyButton.gameObject.SetActive(expanded);
                copyButton.onClick.RemoveAllListeners();
                copyButton.onClick.AddListener(() => { OnClickCopyItemLog(index); });
                var text = button.transform.Find("Text").GetComponent<Text>();
                text.horizontalOverflow = expanded ? HorizontalWrapMode.Wrap : HorizontalWrapMode.Overflow;
                text.text = GetRendererLogString(info, expanded, text.fontSize);
                var countImage = button.transform.Find("CountImage");
                countImage.gameObject.SetActive(collapse.IsOn);
                if (collapse.IsOn)
                {
                    collapseMap.TryGetValue(info.GetHashCode(), out var count);
                    countImage.Find("Text").GetComponent<Text>().text = count.ToString();
                }
            }

            private string GetRendererLogString(LogInfo info, bool expanded, int fontSize)
            {
                string content;
                strBuilder.Clear();
                //有颜色开头,把时间插入到颜色段内
                Match match = Regex.Match(info.text, "^<color=#([a-z]|[A-Z]|[0-9]){6,8}>");
                if (match.Success)
                {
                    content = info.text.Insert(match.Value.Length, string.Format("{0} ", info.dateTime));
                }
                else
                {
                    content = string.Format("{0} {1}", info.dateTime, info.text);
                }

                if (!expanded)
                {
                    float width = View.LogList.scrollRect.viewport.rect.size.x;
                    int count = fontSize == 0 ? MAX_RENDER_TEXT_LENGTH : (int) (width * 3 / fontSize);
                    if (content.Length > count)
                    {
                        string colorStr = Regex.IsMatch(info.text, "</color>$") ? "</color>" : string.Empty;
                        content = string.Format("{0}{1}{2}", content.Substring(0, count), "...", colorStr);
                    }
                }

                strBuilder.Append(content);
                if (expanded && !string.IsNullOrEmpty(info.stackTrace))
                {
                    strBuilder.AppendLine();
                    strBuilder.Append(info.stackTrace);
                }

                return GetRenderString(strBuilder);
            }

            private void OnClickExpandedItem(int index)
            {
                if (focusExpandedIndex == index)
                {
                    focusExpandedIndex = -1;
                    View.LogList.RefreshItemSize(index);
                    View.LogList.RefreshItemRender(index);
                }
                else
                {
                    int prevExpandedIndex = focusExpandedIndex;
                    focusExpandedIndex = index;
                    View.LogList.RefreshItemSize(index);
                    View.LogList.RefreshItemRender(index);
                    if (prevExpandedIndex >= 0)
                    {
                        View.LogList.RefreshItemSize(prevExpandedIndex);
                        View.LogList.RefreshItemRender(prevExpandedIndex);
                    }
                }
            }

            private void OnClickCopyItemLog(int index)
            {
                GUIUtility.systemCopyBuffer = viewLogs[index].ToString();
            }

            private void OnFilterLogs()
            {
                viewLogs.Clear();
                if (collapse.IsOn)
                {
                    collapseMap.Clear();
                }

                foreach (var item in allLogs)
                {
                    TryFilterLog(item);
                }

                View.LogList.NumItems = viewLogs.Count;
            }

            private bool TryFilterLog(LogInfo info)
            {
                if (!string.IsNullOrEmpty(filterText))
                {
                    try
                    {
                        if (!Regex.IsMatch(info.text, filterText, RegexOptions.IgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        SystemPrint("FilterLog Exception:" + e);
                        return false;
                    }
                }

                if (logToggleMap[(int) info.level].IsOn)
                {
                    if (collapse.IsOn)
                    {
                        int key = info.GetHashCode();
                        if (collapseMap.ContainsKey(key))
                        {
                            collapseMap[key] += 1;
                        }
                        else
                        {
                            collapseMap.Add(key, 1);
                            viewLogs.Push(info);
                            return false;
                        }
                    }
                    else
                    {
                        viewLogs.Push(info);
                        return false;
                    }
                }

                return true;
            }
        }
    }
}