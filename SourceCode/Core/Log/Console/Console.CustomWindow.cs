using System;
using System.Collections.Generic;

namespace Core.Log
{
    internal partial class ConsoleController
    {
        private class CustomWindow : ItemInfoWindow, IItemInfoDisplayable
        {
            private float userInfoRefreshInterval = 0;
            private const string UserInfoTitle = "User Information";
            private HashSet<Action<Dictionary<string, object>>> userInfoActions;
            private Dictionary<string, object> appendMap;

            protected override void OnStart()
            {
                userInfoActions = new HashSet<Action<Dictionary<string, object>>>();
                appendMap = new Dictionary<string, object>();
                infos.Add(new ConsoleItemInfo(UserInfoTitle, CustomInformation, true, userInfoRefreshInterval));
            }

            private void CustomInformation(Dictionary<string, object> map)
            {
                foreach (var action in userInfoActions)
                {
                    appendMap.Clear();
                    action.Invoke(appendMap);
                    foreach (var item in appendMap)
                    {
                        map[item.Key] = item.Value;
                    }
                }

                if (map.Count == 0)
                {
                    map.Add("Tip:", "You can add user data to display here");
                }
            }

            public void AddUserInfo(Action<Dictionary<string, object>> action)
            {
                userInfoActions.Add(action);
            }

            public void RemoveUserInfo(Action<Dictionary<string, object>> action)
            {
                userInfoActions.Remove(action);
            }

            public void SetUserInfoRefreshInterval(float interval)
            {
                if (userInfoRefreshInterval != interval)
                {
                    userInfoRefreshInterval = interval;
                    SetConsoleItemInfoInterval(UserInfoTitle, interval);
                }
            }

            private bool SetConsoleItemInfoInterval(string title, float interval)
            {
                if (null != infos)
                {
                    for (int i = 0; i < infos.Count; i++)
                    {
                        if (infos[i].title == title)
                        {
                            var info = infos[i];
                            info.refreshInterval = interval;
                            infos[i] = info;
                            return true;
                        }
                    }
                }

                return false;
            }

            public void AddCustomItem(ConsoleItemInfo info)
            {
                if (!infos.Contains(info))
                {
                    infos.Add(info);
                    View.DeviceList.NumItems = infos.Count;
                }
            }
        }
    }
}