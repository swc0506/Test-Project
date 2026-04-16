using System;
using Core.Timer;

namespace Core
{
    public class GameDate
    {
        public const long BEGIN_TIMESTAMP = 62135596800L;
        public const int ONE_MINUTE_SECONDS = 60;
        public const int ONE_HOUR_SECONDS = 60 * 60;
        public const int ONE_DAY_SECONDS = 24 * 60 * 60;
        public const int ONE_WEEK_SECONDS = 7 * 24 * 60 * 60;

        private readonly GameTime gameTime;
        private readonly TimeZoneInfo timeZoneInfo;
        private readonly bool isLocalOffset;

        public TimeSpan UtcOffset
        {
            get { return timeZoneInfo.BaseUtcOffset; }
        }

        private Scheduler scheduler;

        public GameDate(GameTime gameTime, TimeZoneInfo timeZoneInfo)
        {
            this.gameTime = gameTime;
            this.timeZoneInfo = timeZoneInfo;
            isLocalOffset = timeZoneInfo.BaseUtcOffset == TimeZoneInfo.Local.BaseUtcOffset;

            scheduler = new Scheduler();
        }

        public long Timestamp
        {
            get { return gameTime.Timestamp; }
        }

        public long TimestampMillis
        {
            get { return gameTime.TimestampMillis; }
        }

        /// <summary>
        /// 当前的日期
        /// </summary>
        public DateTime Now
        {
            get { return GetDateTime(Timestamp); }
        }

        /// <summary>
        /// 根据时间戳转换成日期
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public DateTime GetDateTime(long timestamp)
        {
            DateTimeOffset dateTimeOffset = FromUnixTimeSeconds(timestamp);
            if (isLocalOffset)
            {
                return dateTimeOffset.LocalDateTime;
            }
            else
            {
                return TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.UtcDateTime, timeZoneInfo);
            }
        }

        private DateTimeOffset FromUnixTimeSeconds(long timestamp)
        {
            return new DateTimeOffset((timestamp + GameDate.BEGIN_TIMESTAMP) * 10000000L, TimeSpan.Zero);
        }

        #region 获取时间戳

        /// <summary>
        /// 根据日期获取时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public long GetTimestamp(DateTime dateTime)
        {
            if (!isLocalOffset)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
                dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo);
            }

            return ToUnixTimeSeconds(new DateTimeOffset(dateTime));
        }

        public long GetTimestamp(int year, int month, int day, int hour, int minute, int second)
        {
            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
            return GetTimestamp(dateTime);
        }

        public long GetTimestamp(int year, int month, int day, int hour, int minute)
        {
            return GetTimestamp(year, month, day, hour, minute, 0);
        }

        public long GetTimestamp(int year, int month, int day, int hour)
        {
            return GetTimestamp(year, month, day, hour, 0);
        }

        public long GetTimestamp(int year, int month, int day)
        {
            return GetTimestamp(year, month, day, 0);
        }

        private long ToUnixTimeSeconds(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.UtcDateTime.Ticks / 10000000L - GameDate.BEGIN_TIMESTAMP;
        }

        #endregion

        #region 获取偏移的时间戳

        /// <summary>
        /// 获取相对当前时间偏移多少的时间戳
        /// </summary>
        /// <param name="years"></param>
        /// <param name="months"></param>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public long GetOffsetTimestamp(int years, int months, int days, int hours, int minutes, int seconds)
        {
            DateTime dateTime = Now;
            dateTime = dateTime.AddYears(years);
            dateTime = dateTime.AddMonths(months);
            dateTime = dateTime.AddDays(days);
            dateTime = dateTime.AddHours(hours);
            dateTime = dateTime.AddMonths(minutes);
            dateTime = dateTime.AddSeconds(seconds);
            return GetTimestamp(dateTime);
        }

        public long GetOffsetTimestamp(int months, int days, int hours, int minutes, int seconds)
        {
            return GetOffsetTimestamp(0, months, days, hours, minutes, seconds);
        }
        
        public long GetOffsetTimestamp(int days, int hours, int minutes, int seconds)
        {
            return GetOffsetTimestamp(0, days, hours, minutes, seconds);
        }

        public long GetOffsetTimestamp(int hours, int minutes, int seconds)
        {
            return GetOffsetTimestamp(0, hours, minutes, seconds);
        }

        public long GetOffsetTimestamp(int minutes, int seconds)
        {
            return GetOffsetTimestamp(0, minutes, seconds);
        }

        public long GetOffsetTimestamp(int seconds)
        {
            return GetOffsetTimestamp(0, seconds);
        }

        public long GetOffsetTimestamp(TimeSpan timeSpan)
        {
            DateTime dateTime = Now;
            dateTime = dateTime.Add(timeSpan);
            return GetTimestamp(dateTime);
        }
        
        #endregion

        #region 获取偏移的日期
        
        /// <summary>
        /// 获取相对当前时间偏移多少的日期
        /// </summary>
        /// <param name="years"></param>
        /// <param name="months"></param>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public DateTime GetOffsetDateTime(int years, int months, int days, int hours, int minutes, int seconds)
        {
            return GetDateTime(GetOffsetTimestamp(years, months, days, hours, minutes, seconds));
        }

        public DateTime GetOffsetDateTime(int months, int days, int hours, int minutes, int seconds)
        {
            return GetDateTime(GetOffsetTimestamp(months, days, hours, minutes, seconds));
        }

        public DateTime GetOffsetDateTime(int days, int hours, int minutes, int seconds)
        {
            return GetDateTime(GetOffsetTimestamp(days, hours, minutes, seconds));
        }

        public DateTime GetOffsetDateTime(int hours, int minutes, int seconds)
        {
            return GetDateTime(GetOffsetTimestamp(hours, minutes, seconds));
        }

        public DateTime GetOffsetDateTime(int minutes, int seconds)
        {
            return GetDateTime(GetOffsetTimestamp(minutes, seconds));
        }

        public DateTime GetOffsetDateTime(int seconds)
        {
            return GetDateTime(GetOffsetTimestamp(seconds));
        }
        
        public DateTime GetOffsetDateTime(TimeSpan timeSpan)
        {
            return GetDateTime(GetOffsetTimestamp(timeSpan));
        }

        #endregion

        #region 获取该时间戳几天后的某个点的间戳

        /// <summary>
        /// 获取该时间戳几天后的几点几分几秒时间戳
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="afterDay"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public long GetAfterDayTimestamp(long timestamp, int afterDay, int hour, int minute, int second)
        {
            DateTime nowTime = GetDateTime(timestamp);
            DateTime dateTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, hour, minute, second);
            dateTime = dateTime.AddDays(afterDay);
            return GetTimestamp(dateTime);
        }
        
        /// <summary>
        /// 获取该时间戳几天后的0点0分0秒时间戳
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="afterDay"></param>
        /// <returns></returns>
        public long GetAfterDayTimestamp(long timestamp, int afterDay)
        {
            return GetAfterDayTimestamp(timestamp, afterDay, 0, 0, 0);
        }

        
        /// <summary>
        /// 获取当前时间戳几天后的几点几分几秒时间戳
        /// </summary>
        /// <param name="afterDay"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public long GetNowAfterDayTimestamp(int afterDay, int hour, int minute, int second)
        {
            return GetAfterDayTimestamp(Timestamp, afterDay, hour, minute, second);
        }

        /// <summary>
        /// 获取当前时间戳几天后的0点0分0秒时间戳
        /// </summary>
        /// <param name="afterDay"></param>
        /// <returns></returns>
        public long GetNowAfterDayTimestamp(int afterDay)
        {
            return GetAfterDayTimestamp(Timestamp, afterDay);
        }


        /// <summary>
        /// 获取该时间戳这天的几点几分几秒时间戳
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public long GetTodayTimestamp(long timestamp, int hour, int minute, int second)
        {
            return GetAfterDayTimestamp(timestamp, 0, hour, minute, second);
        }

        /// <summary>
        /// 获取该时间戳这天的0点0分0秒时间戳
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public long GetTodayTimestamp(long timestamp)
        {
            return GetTodayTimestamp(timestamp, 0, 0, 0);
        }
        
        /// <summary>
        /// 获取当前时间戳这天的几点几分几秒时间戳
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public long GetNowTodayTimestamp(int hour, int minute, int second)
        {
            return GetTodayTimestamp(Timestamp, hour, minute, second);
        }

        /// <summary>
        /// 获取当前时间戳这天的0点0分0秒时间戳
        /// </summary>
        /// <returns></returns>
        public long GetNowTodayTimestamp()
        {
            return GetTodayTimestamp(Timestamp);
        }

        #endregion

        #region 获取该时间戳几周后星期几的某个点的间戳

        /// <summary>
        /// 获取该时间戳几周后星期几的几点几分几秒时间戳 从周1开始
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="afterWeek"></param>
        /// <param name="dayOfWeek"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public long GetAfterDayOfWeekTimestamp(long timestamp, int afterWeek, DayOfWeek dayOfWeek, int hour, int minute,
            int second)
        {
            DateTime nowTime = GetDateTime(timestamp);
            DateTime dateTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, hour, minute, second);
            dateTime = dateTime.AddDays(afterWeek * 7);
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
            int startDaysOffset = (dateTime.DayOfWeek - firstDayOfWeek + 7) % 7;
            dateTime = dateTime.AddDays(-startDaysOffset);
            if (dateTime.DayOfWeek != dayOfWeek)
            {
                int daysOffset = (dayOfWeek - firstDayOfWeek + 7) % 7;
                dateTime = dateTime.AddDays(daysOffset);
            }

            return GetTimestamp(dateTime);
        }

        /// <summary>
        /// 获取该时间戳几周后星期几的0点0分0秒时间戳 从周1开始
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="afterWeek"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public long GetAfterDayOfWeekTimestamp(long timestamp, int afterWeek, DayOfWeek dayOfWeek)
        {
            return GetAfterDayOfWeekTimestamp(timestamp, afterWeek, dayOfWeek, 0, 0, 0);
        }
        
        /// <summary>
        /// 获取当前间戳几周后星期几的某个时间点的时间戳 从周1开始
        /// </summary>
        /// <param name="afterWeek"></param>
        /// <param name="dayOfWeek"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public long GetNowAfterDayOfWeekTimestamp(int afterWeek, DayOfWeek dayOfWeek, int hour, int minute, int second)
        {
            return GetAfterDayOfWeekTimestamp(Timestamp, afterWeek, dayOfWeek, hour, minute, second);
        }

        /// <summary>
        /// 获取当前时间戳几周后星期几的0点0分0秒时间戳 从周1开始
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="afterWeek"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public long GetNowAfterDayOfWeekTimestamp(int afterWeek, DayOfWeek dayOfWeek)
        {
            return GetAfterDayOfWeekTimestamp(Timestamp, afterWeek, dayOfWeek, 0, 0, 0);
        }

        /// <summary>
        /// 获取该时间戳本周几的几点几分几秒时间戳 从周1开始
        /// </summary>
        /// <returns></returns>
        public long GetDayOfWeekTimestamp(long timestamp, DayOfWeek dayOfWeek, int hour, int minute, int second)
        {
            return GetAfterDayOfWeekTimestamp(timestamp, 0, dayOfWeek, hour, minute, second);
        }

        /// <summary>
        /// 获取该时间戳本周几的0点0分0秒时间戳
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public long GetDayOfWeekTimestamp(long timestamp, DayOfWeek dayOfWeek)
        {
            return GetDayOfWeekTimestamp(timestamp, dayOfWeek, 0, 0, 0);
        }
        
        /// <summary>
        /// 获取当前时间戳本周几的几点几分几秒时间戳 从周1开始
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public long GetNowDayOfWeekTimestamp(DayOfWeek dayOfWeek, int hours, int minutes, int seconds)
        {
            return GetDayOfWeekTimestamp(Timestamp, dayOfWeek, hours, minutes, seconds);
        }

        /// <summary>
        /// 获取当前时间戳本周几的0点0分0秒时间戳
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public long GetNowDayOfWeekTimestamp(DayOfWeek dayOfWeek)
        {
            return GetDayOfWeekTimestamp(Timestamp, dayOfWeek);
        }

        #endregion

        #region 事件

        internal void Update(float deltaTime)
        {
            scheduler.Update(deltaTime);
        }

        private float GetDelaySeconds(int dayOfWeek, int hour, int minute, int second, float interval)
        {
            DateTime now = Now;
            float delay = 0;
            int delta = 0;
            if (dayOfWeek > 0)
            {
                int nowDayOfWeek = now.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)now.DayOfWeek;
                delta = dayOfWeek - nowDayOfWeek;
                delay += delta * ONE_DAY_SECONDS;
            }

            if (hour >= 0)
            {
                delta = hour - now.Hour;
                delay += delta * ONE_HOUR_SECONDS;
            }

            if (minute >= 0)
            {
                delta = minute - now.Minute;
                delay += delta * ONE_MINUTE_SECONDS;
            }

            if (second >= 0)
            {
                delta = second - now.Second;
                delay += delta;
            }

            if (delay < 0)
            {
                delay += interval;
            }

            return delay;
        }

        /// <summary>
        /// 添加固定在每分钟的第几秒执行事件
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public int AddFixedMinutelyRate(int second, Action callback)
        {
            if (second >= 0 && second <= 59)
            {
                float delay = GetDelaySeconds(-1, -1, -1, second, ONE_MINUTE_SECONDS);
                return scheduler.Repeat(delay, callback, -1, ONE_MINUTE_SECONDS);
            }

            return 0;
        }

        /// <summary>
        /// 添加固定在每小时的第几分几秒执行事件
        /// </summary>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public int AddFixedHourlyRate(int minute, int second, Action callback)
        {
            if (minute >= 0 && minute <= 59)
            {
                if (second >= 0 && second <= 59)
                {
                    float delay = GetDelaySeconds(-1, -1, minute, second, ONE_HOUR_SECONDS);
                    return scheduler.Repeat(delay, callback, -1, ONE_HOUR_SECONDS);
                }
            }

            return 0;
        }

        /// <summary>
        /// 添加固定在每日的第几小时几分几秒执行事件
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public int AddFixedDailyRate(int hour, int minute, int second, Action callback)
        {
            if (hour >= 0 && hour <= 23)
            {
                if (minute >= 0 && minute <= 59)
                {
                    if (second >= 0 && second <= 59)
                    {
                        float delay = GetDelaySeconds(-1, hour, minute, second, ONE_DAY_SECONDS);
                        return scheduler.Repeat(delay, callback, -1, ONE_DAY_SECONDS);
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 添加固定在每周的星期几几小时几分几秒执行事件
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public int AddFixedWeeklyRate(int dayOfWeek, int hour, int minute, int second, Action callback)
        {
            if (dayOfWeek >= 1 && dayOfWeek <= 7)
            {
                if (hour >= 0 && hour <= 23)
                {
                    if (minute >= 0 && minute <= 59)
                    {
                        if (second >= 0 && second <= 59)
                        {
                            float delay = GetDelaySeconds(dayOfWeek, hour, minute, second, ONE_WEEK_SECONDS);
                            return scheduler.Repeat(delay, callback, -1, ONE_WEEK_SECONDS);
                        }
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 移除固定周期事件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveFixedRate(int id)
        {
            return scheduler.Cancel(id);
        }

        /// <summary>
        /// 根据回调者移除固定周期事件
        /// </summary>
        /// <param name="caller"></param>
        public void CancelFixedRateByCaller(object caller)
        {
            scheduler.CancelByCaller(caller);
        }

        /// <summary>
        /// 清除所有固定周期事件
        /// </summary>
        public void ClearAllFixedRate()
        {
            scheduler.CancelAll();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Date:{0},Timestamp:{1}", Now.ToString("yyyy-MM-dd HH:mm:ss"), Timestamp);
        }
    }
}