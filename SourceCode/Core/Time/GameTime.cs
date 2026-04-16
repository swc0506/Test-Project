using System;
using UnityEngine;

namespace Core
{
    public class GameTime : Singleton<GameTime>
    {
        private bool isStart;
        private long startTimestamp;
        private float startRealTime;
        private float prevRealTime;

        /// <summary>
        /// 本地时区的日期
        /// </summary>
        public GameDate Local { get; private set; }

        /// <summary>
        /// 服务端时区的日期
        /// </summary>
        public GameDate Server { get; private set; }

        /// <summary>
        /// 本地时区与服务器时区的差
        /// </summary>
        public TimeSpan UtcOffset { get; private set; }

        /// <summary>
        /// 时间戳 秒
        /// </summary>
        public long Timestamp
        {
            get { return startTimestamp + (int)Math.Floor(GetElapseTime()); }
        }

        /// <summary>
        /// 时间戳 毫秒
        /// </summary>
        public long TimestampMillis
        {
            get { return (long)((startTimestamp + Math.Floor(GetElapseTime())) * 1000); }
        }

        private float GetElapseTime()
        {
            return TimeUtils.GetUnityElapse(startRealTime);
        }

        public void Start(long startTimestamp, TimeSpan serverUtc)
        {
            if (startTimestamp <= 0)
            {
                startTimestamp = ToUnixTimeSeconds(DateTimeOffset.Now);
            }

            this.startTimestamp = startTimestamp;

            startRealTime = TimeUtils.GetUnityNow();
            prevRealTime = startRealTime;
            if (null == Local)
            {
                Local = new GameDate(this, TimeZoneInfo.Local);
            }

            if (null == Server)
            {
                TimeZoneInfo timeZone = TimeZoneInfo.CreateCustomTimeZone("UTC", serverUtc, "Server", "Server");
                Server = new GameDate(this, timeZone);
            }

            UtcOffset = Local.UtcOffset - Server.UtcOffset;
            isStart = true;
        }

        public void Start(long startTimestamp, int serverUtcOffsetHours, int serverUtcOffsetMinutes)
        {
            Start(startTimestamp, new TimeSpan(0, serverUtcOffsetHours, serverUtcOffsetMinutes, 0));
        }

        public void Start(int serverUtcOffsetHours, int serverUtcOffsetMinutes)
        {
            Start(0, serverUtcOffsetHours, serverUtcOffsetMinutes);
        }

        public void Start(long startTimestamp)
        {
            Start(startTimestamp, TimeZoneInfo.Local.BaseUtcOffset);
        }

        public void Start()
        {
            Start(ToUnixTimeSeconds(DateTimeOffset.Now));
        }

        private long ToUnixTimeSeconds(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.UtcDateTime.Ticks / 10000000L - GameDate.BEGIN_TIMESTAMP;
        }

        /// <summary>
        /// 矫正时间戳
        /// </summary>
        /// <param name="timestamp"></param>
        public void Correct(long timestamp)
        {
            startTimestamp = timestamp;
            startRealTime = TimeUtils.GetUnityNow();
            prevRealTime = startRealTime;
        }

        public void Update(float deltaTime)
        {
            if (isStart)
            {
                float deltaRealTime = TimeUtils.GetUnityElapse(prevRealTime);
                prevRealTime = TimeUtils.GetUnityNow();
                Local.Update(deltaRealTime);
                Server.Update(deltaRealTime);
            }
        }

        public void Stop()
        {
            startTimestamp = 0;
            startRealTime = 0;
            Local?.ClearAllFixedRate();
            Server?.ClearAllFixedRate();
            UtcOffset = TimeSpan.Zero;
            isStart = false;
        }
    }
}