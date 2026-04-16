using System;
using System.Collections.Generic;

namespace Core.Timer
{
    public class Scheduler
    {
        private AtomicInt atomic = new AtomicInt();
        private readonly Dictionary<int, Timer> addMap = new Dictionary<int, Timer>();
        private readonly Dictionary<int, Timer> cancelMap = new Dictionary<int, Timer>();
        private readonly List<int> temp = new List<int>();

        private readonly Dictionary<int, Timer> activeMap = new Dictionary<int, Timer>();

        /// <summary>
        /// 延迟执行1次任务
        /// 如：某个任务需要在10秒后开始执行
        /// </summary>
        /// <param name="delay">延迟时间：单位秒</param>
        /// <param name="callback">回调方法</param>
        /// <returns>定时器唯一id</returns>
        public int Once(float delay, Action callback)
        {
            OnceTimer timer = new OnceTimer();
            timer.SetDelayAction(delay, callback);
            int timerId = atomic.IncrementAndGet();
            addMap.Add(timerId, timer);
            return timerId;
        }

        /// <summary>
        /// 延迟执行,每间隔几秒重复执行几次
        /// 如：某个任务需要在10秒后开始执行，每间隔1秒执行一次，重复执行3次。
        /// </summary>
        /// <param name="delay">延迟时间：单位秒</param>
        /// <param name="callback">回调方法</param>
        /// <param name="repeat">执行的次数 -1代表无限次</param>
        /// <param name="interval">间隔时间：单位秒</param>
        /// <returns>定时器唯一id</returns>
        public int Repeat(float delay, Action callback, int repeat, float interval)
        {
            RepeatTimer timer = new RepeatTimer();
            timer.SetDelayAction(delay, callback);
            timer.SetRepeat(repeat, interval);
            int timerId = atomic.IncrementAndGet();
            addMap.Add(timerId, timer);
            return timerId;
        }

        /// <summary>
        /// 延迟执行，持续几秒每帧都执行
        /// 如：某个任务需要在10秒后开始执行，持续5秒,每帧都会执行。
        /// </summary>
        /// <param name="delay">延迟时间：单位秒</param>
        /// <param name="callback">回调方法</param>
        /// <param name="duration">持续时间：单位秒 -1代表无限时长</param>
        /// <returns>定时器唯一id</returns>
        public int Duration(float delay, Action callback, float duration)
        {
            DurationTimer timer = new DurationTimer();
            timer.SetDelayAction(delay, callback);
            timer.SetDuration(duration);
            int timerId = atomic.IncrementAndGet();
            addMap.Add(timerId, timer);
            return timerId;
        }

        /// <summary>
        /// 取消定时器
        /// </summary>
        /// <param name="timerId">定时器唯一id</param>
        /// <returns></returns>
        public bool Cancel(int timerId)
        {
            Timer timer = null;
            if (activeMap.TryGetValue(timerId, out timer))
            {
                cancelMap[timerId] = timer;
                return true;
            }

            if (addMap.TryGetValue(timerId, out timer))
            {
                addMap.Remove(timerId);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 根据回调者取消定时器 不能是匿名函数
        /// </summary>
        /// <param name="caller">回调者</param>
        public void CancelByCaller(object caller)
        {
            foreach (var item in addMap)
            {
                if (item.Value.EqualsCaller(caller))
                {
                    temp.Add(item.Key);
                }
            }

            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    addMap.Remove(item);
                }

                temp.Clear();
            }

            foreach (var item in activeMap)
            {
                if (item.Value.EqualsCaller(caller))
                {
                    cancelMap[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// 取消所有定时任务
        /// </summary>
        public void CancelAll()
        {
            addMap.Clear();
            cancelMap.Clear();
            activeMap.Clear();
        }

        private Timer GetActiveTimer(int timerId)
        {
            Timer timer = null;
            if (addMap.TryGetValue(timerId, out timer))
            {
                return timer;
            }

            if (activeMap.TryGetValue(timerId, out timer))
            {
                return timer;
            }

            return null;
        }

        /// <summary>
        /// 暂停定时器
        /// </summary>
        /// <param name="timerId">定时器唯一id</param>
        /// <returns></returns>
        public bool Pause(int timerId)
        {
            Timer timer = GetActiveTimer(timerId);
            return null != timer && timer.Pause();
        }

        public bool Resume(int timerId)
        {
            Timer timer = GetActiveTimer(timerId);
            return null != timer && timer.Resume();
        }

        public void Update(float deltaTime)
        {
            CheckAddTimers();
            CheckCancelTimers();
            TickTimers(deltaTime);
        }

        private void CheckAddTimers()
        {
            if (addMap.Count > 0)
            {
                foreach (var item in addMap)
                {
                    activeMap.Add(item.Key, item.Value);
                }

                addMap.Clear();
            }
        }

        private void CheckCancelTimers()
        {
            if (cancelMap.Count > 0)
            {
                foreach (var item in cancelMap)
                {
                    activeMap.Remove(item.Key);
                }

                cancelMap.Clear();
            }
        }

        private void TickTimers(float deltaTime)
        {
            foreach (var item in activeMap)
            {
                if (item.Value.State == TimerState.Over)
                {
                    cancelMap.Add(item.Key, item.Value);
                }
                else
                {
                    item.Value.Update(deltaTime);
                }
            }
        }
    }
}