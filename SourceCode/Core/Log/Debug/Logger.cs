using System;
using System.IO;
using Core.Log;
using UnityEngine;
#if LOG4NET
using log4net;
using log4net.Config;

#endif

namespace Core
{
    public interface ILogger
    {
        void Debug(object message);
        void DebugFormat(string format, params object[] args);
        void Info(object message);
        void InfoFormat(string format, params object[] args);
        void Warn(object message);
        void WarnFormat(string format, params object[] args);
        void Error(object message);
        void ErrorFormat(string format, params object[] args);
        void Fatal(object message);
        void FatalFormat(string format, params object[] args);
    }

    public static class Logger
    {
        internal static string logPath;
        private static ILogger logger;

        public static void Startup()
        {
            logPath = Application.isEditor ? Directory.GetCurrentDirectory() : Application.temporaryCachePath;

            string text = FileReadUtils.ReadText("Log/Log4Net.xml");
            SetConfig(text);
            if (null == logger)
            {
                logger = new UnityLogger();
            }
        }

        private static Stream ConvertStream(string str)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(str);
            sw.Flush();
            ms.Position = 0;
            return ms;
        }

        public static void SetConfig(string xmlText)
        {
            try
            {
                Stream stream = ConvertStream(xmlText);
#if LOG4NET
                XmlConfigurator.Configure(stream);
#endif
                stream.Dispose();

                //MatchBestLogger
                string logType = null;
                if (UnityEngine.Debug.isDebugBuild)
                {
                    logType = "DEBUG";
                }
                else
                {
                    logType = "RELEASE";
                }

                logger = GetLogger(logType);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning(e.Message);
            }
        }

        private static ILogger GetLogger(string name)
        {
#if LOG4NET
            ILog log = LogManager.GetLogger(name);
            return new Log4NetLogger(log);
#else
            return null;
#endif
        }

        public static ILogger GetLogger(Type type)
        {
#if LOG4NET
            ILog log = LogManager.GetLogger(type);
            return new Log4NetLogger(log);
#else
            return null;
#endif
        }

        public static void Debug(object message)
        {
            logger?.Debug(message);
        }

        public static void Info(object message)
        {
            logger?.Info(message);
        }

        public static void Warn(object message)
        {
            logger?.Warn(message);
        }

        public static void Error(object message)
        {
            logger?.Error(message);
        }

        public static void Fatal(object message)
        {
            logger?.Fatal(message);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            logger?.DebugFormat(format, args);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            logger?.InfoFormat(format, args);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            logger?.WarnFormat(format, args);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            logger?.ErrorFormat(format, args);
        }

        public static void FatalFormat(string format, params object[] args)
        {
            logger?.FatalFormat(format, args);
        }

        public static void Shutdown()
        {
#if LOG4NET
            LogManager.Shutdown();
#endif
            logger = null;
        }
    }
}