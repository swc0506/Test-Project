using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Log
{
    public class ConsoleItemInfo : IEquatable<ConsoleItemInfo>
    {
        private Dictionary<string, object> infoMap;
        private StringBuilder strBuilder;

        public string title;
        private Action<Dictionary<string, object>> actionInfo;
        public bool expanded;
        public float refreshInterval;
        private float elapseTime;
        private string text;

        public ConsoleItemInfo(string title, Action<Dictionary<string, object>> actionInfo, bool expanded = false,
            float refreshInterval = 0)
        {
            infoMap = new Dictionary<string, object>();
            strBuilder = new StringBuilder();

            this.title = title;
            this.actionInfo = actionInfo;
            this.expanded = expanded;
            this.refreshInterval = refreshInterval;
        }

        private string FormatText(bool addColor)
        {
            infoMap.Clear();
            strBuilder.Clear();
            actionInfo?.Invoke(infoMap);
            foreach (var item in infoMap)
            {
                string format = addColor ? "<color=#8BDE00>{0}:</color>{1}" : "{0}:{1}";
                strBuilder.AppendFormat(format, item.Key, item.Value);
                strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }

        public string Text
        {
            get
            {
                if (null == text)
                {
                    text = FormatText(true);
                }

                return text;
            }
        }

        public bool OnRefresh(float deltaTime)
        {
            if (refreshInterval > 0 && expanded)
            {
                elapseTime += deltaTime;
                if (elapseTime > refreshInterval)
                {
                    elapseTime = 0;
                    text = FormatText(true);
                    return true;
                }
            }

            return false;
        }

        public bool Equals(ConsoleItemInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return title == other.title;
        }

        public override string ToString()
        {
            return FormatText(false);
        }
    }

    internal interface IItemInfoDisplayable
    {
        /// <summary>
        /// 增加用户显示数据
        /// </summary>
        /// <param name="action"></param>
        void AddUserInfo(Action<Dictionary<string, object>> action);

        /// <summary>
        /// 移除用户显示数据
        /// </summary>
        /// <param name="action"></param>
        void RemoveUserInfo(Action<Dictionary<string, object>> action);


        /// <summary>
        /// 设置用户显示数据 刷新间隔时间
        /// </summary>
        /// <param name="interval">秒</param>
        void SetUserInfoRefreshInterval(float interval);

        /// <summary>
        /// 增加自定义栏显示数据
        /// </summary>
        /// <param name="info"></param>
        void AddCustomItem(ConsoleItemInfo info);
    }
}