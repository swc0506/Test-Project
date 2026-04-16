using System.Collections.Generic;
using UnityEngine;

namespace CoreEditor.HybridCLR
{
    public struct LogInfo
    {
        public string text;
        public LogType type;

        public LogInfo(string text, LogType type)
        {
            this.text = text;
            this.type = type;
        }
    }

    public class CompileInfos
    {
        private static Queue<LogInfo> logs = new Queue<LogInfo>();

        public static Queue<LogInfo> Logs
        {
            get { return logs; }
        }

        internal static void Push(string text, LogType type)
        {
            logs.Enqueue(new LogInfo(text, type));
        }
    }
}