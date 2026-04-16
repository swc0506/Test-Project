using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Log;
using UnityEngine;

namespace Core
{
    public class Console
    {
        private struct ConsoleInfo
        {
            public bool startup;
            public int maxLogNum;
            public bool errorAutomaticOpen;
            public string switchTriggerType;
            public float userInfoRefreshInterval;
        }

        private static ConsoleController ctrl;
        private static ConsoleInfo info;

        public static bool IsEnable { get; private set; }

        public static Commander GetCommand()
        {
            if (null != ctrl)
            {
                return ctrl.commander;
            }

            return null;
        }

        /// <summary>
        /// 启动控制台
        /// </summary>
        /// <param name="jsonText">json配置表</param>
        public static void Startup(string jsonText, ISwitchTrigger trigger)
        {
            if (string.IsNullOrEmpty(jsonText) || null == trigger)
            {
                return;
            }

            info = JsonUtils.ToObject<ConsoleInfo>(jsonText);
            IsEnable = Debug.isDebugBuild || info.startup;
            if (IsEnable)
            {
                if (null == ctrl)
                {
                    int maxLogNum = info.maxLogNum;
                    maxLogNum = maxLogNum <= 0 ? 1000 : maxLogNum;
                    ctrl = new ConsoleController(maxLogNum);
                    MonoEventProxy.Instance.LateUpdateEvent += ctrl.Update;
                }

                ctrl.errorAutomaticOpen = info.errorAutomaticOpen;
                ctrl.trigger = trigger;
                ctrl.GetCustomDisplay().SetUserInfoRefreshInterval(info.userInfoRefreshInterval);
                ctrl.Start();
            }
        }

        /// <summary>
        /// 启动log控制台
        /// </summary>
        /// <param name="trigger">控制台开关触发器</param>
        public static void Startup(ISwitchTrigger trigger)
        {
            string text = FileReadUtils.ReadText("Log/Console.json");
            Startup(text, trigger);
        }

        public static void Startup()
        {
            Startup(new DefaultSwitchTrigger());
        }

        /// <summary>
        /// 停止log控制台
        /// </summary>
        public static void Shutdown()
        {
            if (null != ctrl)
            {
                MonoEventProxy.Instance.LateUpdateEvent -= ctrl.Update;
                ctrl.Dispose();
                ctrl = null;
            }
        }

        public static void SetEnable(bool isEnable)
        {
            IsEnable = isEnable;
        }

        public static bool Visible
        {
            get
            {
                if (null != ctrl)
                {
                    return ctrl.Visible;
                }

                return false;
            }
        }

        public static event Action<bool> SwitchEvent
        {
            add
            {
                if (null != ctrl)
                {
                    ctrl.SwitchEvent += value;
                }
            }

            remove
            {
                if (null != ctrl)
                {
                    ctrl.SwitchEvent -= value;
                }
            }
        }

        public static void AddCommand(string command, string annotate, Action callback)
        {
            ctrl?.commander.AddCommand(command, annotate, callback);
        }

        public static void AddCommand<T1>(string command, string annotate, Action<T1> callback)
        {
            ctrl?.commander.AddCommand<T1>(command, annotate, callback);
        }

        public static void AddCommand<T1, T2>(string command, string annotate, Action<T1, T2> callback)
        {
            ctrl?.commander.AddCommand<T1, T2>(command, annotate, callback);
        }

        public static void AddCommand<T1, T2, T3>(string command, string annotate, Action<T1, T2, T3> callback)
        {
            ctrl?.commander.AddCommand<T1, T2, T3>(command, annotate, callback);
        }

        public static void AddCommand<T1, T2, T3, T4>(string command, string annotate,
            Action<T1, T2, T3, T4> callback)
        {
            ctrl?.commander.AddCommand<T1, T2, T3, T4>(command, annotate, callback);
        }

        public static void AddCommand(string command, string annotate, Delegate callback)
        {
            ctrl?.commander.AddCommand(command, annotate, callback);
        }

        public static void AddInstanceCommand(string command, string annotate, string methodName, object instance)
        {
            ctrl?.commander.AddInstanceCommand(command, annotate, methodName, instance);
        }

        public static void AddInstanceCommands(object instance)
        {
            ctrl?.commander.AddInstanceCommands(instance);
        }

        public static void AddStaticCommand(string command, string annotate, string methodName, Type type)
        {
            ctrl?.commander.AddStaticCommand(command, annotate, methodName, type);
        }

        public static void AddStaticCommands(Type type)
        {
            ctrl?.commander.AddStaticCommands(type);
        }

        public static void AddStaticCommands(Assembly assembly)
        {
            ctrl?.commander.AddStaticCommands(assembly);
        }

        public static void AddStaticCommands()
        {
            ctrl?.commander.AddStaticCommands(Assembly.GetCallingAssembly());
        }


        public static void RemoveCommand(string command, Action callback)
        {
            ctrl?.commander.RemoveCommand(command, callback);
        }

        public static void RemoveCommand<T1>(string command, Action<T1> callback)
        {
            ctrl?.commander.RemoveCommand<T1>(command, callback);
        }

        public static void RemoveCommand<T1, T2>(string command, Action<T1, T2> callback)
        {
            ctrl?.commander.RemoveCommand<T1, T2>(command, callback);
        }

        public static void RemoveCommand<T1, T2, T3>(string command, Action<T1, T2, T3> callback)
        {
            ctrl?.commander.RemoveCommand<T1, T2, T3>(command, callback);
        }

        public static void RemoveCommand<T1, T2, T3, T4>(string command, Action<T1, T2, T3, T4> callback)
        {
            ctrl?.commander.RemoveCommand<T1, T2, T3, T4>(command, callback);
        }

        public static void RemoveCommand(string command, Delegate callback)
        {
            ctrl?.commander.RemoveCommand(command, callback);
        }

        public static void RemoveInstanceCommand(string command, string methodName, object instance)
        {
            ctrl?.commander.RemoveInstanceCommand(command, methodName, instance);
        }

        public static void RemoveStaticCommand(string command, string methodName, Type type)
        {
            ctrl?.commander.RemoveStaticCommand(command, methodName, type);
        }

        public static void RemoveInstanceCommands(object instance)
        {
            ctrl?.commander.RemoveInstanceCommands(instance);
        }

        public static void RemoveStaticCommands(Type type)
        {
            ctrl?.commander.RemoveStaticCommands(type);
        }

        public static void RemoveStaticCommands(Assembly assembly)
        {
            ctrl?.commander.RemoveStaticCommands(assembly);
        }

        public static void RemoveStaticCommands()
        {
            ctrl?.commander.RemoveStaticCommands(Assembly.GetCallingAssembly());
        }

        public static void AddUserInfo(Action<Dictionary<string, object>> action)
        {
            ctrl?.GetCustomDisplay().AddUserInfo(action);
        }

        public static void RemoveUserInfo(Action<Dictionary<string, object>> action)
        {
            ctrl?.GetCustomDisplay().RemoveUserInfo(action);
        }

        public static void AddCustomItem(ConsoleItemInfo info)
        {
            ctrl?.GetCustomDisplay().AddCustomItem(info);
        }
    }
}