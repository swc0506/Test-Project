using log4net;
using UnityEngine;

namespace Core.Log
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog logger;

        public Log4NetLogger(ILog logger)
        {
            this.logger = logger;
            Application.logMessageReceivedThreaded += OnThreadReceivedLog;
        }

        public void Debug(object message)
        {
            logger?.Debug(message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            logger?.DebugFormat(format, args);
        }

        public void Info(object message)
        {
            logger?.Info(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            logger?.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            logger?.Warn(message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            logger?.WarnFormat(format, args);
        }

        public void Error(object message)
        {
            logger?.Error(message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            logger?.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            logger?.Fatal(message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            logger?.FatalFormat(format, args);
        }

        private void OnThreadReceivedLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    FatalFormat("{0}\n{1}", condition, stackTrace);
                }
                else
                {
                    Fatal(condition);
                }
            }
        }
    }
}