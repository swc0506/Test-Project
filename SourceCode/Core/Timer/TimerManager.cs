using System;
using System.Collections.Generic;

namespace Core.Timer
{
    /// <summary>
    /// 定时器管理对象
    /// </summary>
    public class TimerManager : Singleton<TimerManager>
    {
        private Scheduler scheduler;

        protected override void OnInitial()
        {
            base.OnInitial();
            scheduler = new Scheduler();
        }

        /// <summary>
        /// 延迟执行1次任务
        /// 如：某个任务需要在10秒后开始执行
        /// </summary>
        /// <param name="delay">延迟时间：单位秒</param>
        /// <param name="callback">回调方法</param>
        /// <returns>定时器唯一id</returns>
        public int Once(float delay, Action callback)
        {
            return scheduler.Once(delay, callback);
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
            return scheduler.Repeat(delay, callback,repeat,interval);
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
            return scheduler.Duration(delay, callback, duration);
        }

        /// <summary>
        /// 取消定时器
        /// </summary>
        /// <param name="timerId">定时器唯一id</param>
        /// <returns></returns>
        public bool Cancel(int timerId)
        {
            return scheduler.Cancel(timerId);
        }

        /// <summary>
        /// 根据回调者取消定时器 不能是匿名函数
        /// </summary>
        /// <param name="caller">回调者</param>
        public void CancelByCaller(object caller)
        {
            scheduler.CancelByCaller(caller);
        }

        /// <summary>
        /// 取消所有定时任务
        /// </summary>
        public void CancelAll()
        {
            scheduler.CancelAll();
        }

        /// <summary>
        /// 暂停定时器
        /// </summary>
        /// <param name="timerId">定时器唯一id</param>
        /// <returns></returns>
        public bool Pause(int timerId)
        {
            return scheduler.Pause(timerId);
        }

        public bool Resume(int timerId)
        {
            return scheduler.Resume(timerId);
        }

        public void Update(float deltaTime)
        {
            scheduler.Update(deltaTime);
        }
       
    }
}