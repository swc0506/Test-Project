using System;
using System.Diagnostics;
using Core;
using Debug = UnityEngine.Debug;

namespace CoreEditor
{
    public class StatisticsTime : IDisposable
    {
        private Stopwatch stopwatch;

        public void Start(string tip)
        {
            if (null == stopwatch)
            {
                stopwatch = new Stopwatch();
            }
            else
            {
                stopwatch.Reset();
            }

            if (!string.IsNullOrEmpty(tip))
            {
                Debug.Log(tip);
            }

            stopwatch.Start();
        }

        public void Start()
        {
            Start(null);
        }

        public void Stop(string tip)
        {
            if (!string.IsNullOrEmpty(tip))
            {
                string content = string.Format("{0},cost time:{1}", tip,
                    FormatUtils.GetMSTimeText(stopwatch.ElapsedMilliseconds));
                Debug.Log(content);
            }

            stopwatch.Stop();
        }

        public void Stop()
        {
            Stop(null);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}