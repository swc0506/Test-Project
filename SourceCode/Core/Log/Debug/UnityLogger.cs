using UnityEngine;

namespace Core.Log
{
    public class UnityLogger : ILogger
    {
        private UnityEngine.ILogger logger;

        public UnityLogger()
        {
            logger = UnityEngine.Debug.unityLogger;
        }

        public void Debug(object message)
        {
            logger?.Log(LogType.Log, message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            logger?.LogFormat(LogType.Log, format, args);
        }

        public void Info(object message)
        {
            logger?.Log(LogType.Log, message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            logger?.LogFormat(LogType.Log, format, args);
        }

        public void Warn(object message)
        {
            logger?.Log(LogType.Warning, message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            logger?.LogFormat(LogType.Warning, format, args);
        }

        public void Error(object message)
        {
            logger?.Log(LogType.Error, message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            logger?.LogFormat(LogType.Error, format, args);
        }

        public void Fatal(object message)
        {
            logger?.Log(LogType.Exception, message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            logger?.LogFormat(LogType.Exception, format, args);
        }
    }
}