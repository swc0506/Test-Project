using System;
using UnityEngine;

namespace Core
{
    public static class TimeUtils
    {
        public static float GetUnityNow()
        {
            return Time.realtimeSinceStartup;
        }

        public static float GetUnityElapse(float startTime)
        {
            return GetUnityNow() - startTime;
        }

        public static long GetTimestampMillis(DateTime dateTime)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            return dateTimeOffset.UtcDateTime.Ticks / 10000L - GameDate.BEGIN_TIMESTAMP * 1000;
        }

        public static long GetTimestampMillis()
        {
            return GetTimestampMillis(DateTime.Now);
        }


        public static long GetTimestamp(DateTime dateTime)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            return dateTimeOffset.UtcDateTime.Ticks / 10000000L - GameDate.BEGIN_TIMESTAMP;
        }

        public static long GetTimestamp()
        {
            return GetTimestamp(DateTime.Now);
        }
    }
}