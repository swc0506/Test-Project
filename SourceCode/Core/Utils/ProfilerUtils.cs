using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core
{
    public static class ProfilerUtils
    {
        struct StopwatchInfo : IDisposable
        {
            private string name;
            private Stopwatch stopwatch;

            public StopwatchInfo(string name)
            {
                this.name = name;
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }

            public void Dispose()
            {
                double elapse = stopwatch.ElapsedMilliseconds;
                stopwatch.Stop();
                Logger.InfoFormat("{0} Cost {1}:ms", name, elapse);
            }
        }

        private static int serialId;
        private static Dictionary<int, StopwatchInfo> stopwatchMap = new Dictionary<int, StopwatchInfo>();

        public static int Begin(string flagName)
        {
            int id = serialId++;
            StopwatchInfo swInfo = new StopwatchInfo(flagName);
            stopwatchMap.Add(id, swInfo);
            return id;
        }

        public static void End(int id)
        {
            if (stopwatchMap.TryGetValue(id, out StopwatchInfo swInfo))
            {
                swInfo.Dispose();
                stopwatchMap.Remove(id);
            }
        }
    }
}