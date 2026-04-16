using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CoreEditor
{
    public class EditorInterfaceUtils
    {
        private static readonly ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();


        [InitializeOnLoadMethod]
        private static void Initial()
        {
            EditorApplication.update += Update;
        }

        public static void ClearConsole()
        {
            var logEntries = typeof(EditorApplication).Assembly.GetType("UnityEditor.LogEntries");
            if (null != logEntries)
            {
                var clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
                if (null != clearMethod)
                {
                    clearMethod.Invoke(null, null);
                }
            }
        }

        public static void AddFocusEvent(Action<bool> changeFocus)
        {
            EventInfo eventInfo = typeof(EditorApplication).GetEvent("focusChanged",
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (null != eventInfo)
            {
                Action<bool> handler = new Action<bool>(changeFocus);
                eventInfo.GetAddMethod(true).Invoke(null, new object[] { handler });
            }
        }


        private static void Update()
        {
            if (tasks.Count <= 0)
            {
                return;
            }

            while (tasks.TryDequeue(out var action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogWarningFormat("MainThread Call Exception:{0}", e);
                }
            }
        }

        public static void MainThreadCall(Action callback)
        {
            tasks.Enqueue(callback);
        }
    }
}