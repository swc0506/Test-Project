using System;

namespace Core.Timer
{
    internal enum TimerState
    {
        Ticking,
        Pause,
        Cancel,
        Over
    }

    internal abstract class Timer : IUpdateable, IClearable
    {
        /// <summary>
        /// 延迟时间 单位秒
        /// </summary>
        protected float delay { get; private set; }

        /// <summary>
        /// 回调方法
        /// </summary>
        private Action callback;

        private bool isStart;

        /// <summary>
        /// 总流失时间 单位秒
        /// </summary>
        protected float elapseTime { get; private set; }

        public TimerState State { get; protected set; }


        /// <summary>
        /// 设置延迟时间和回调方法
        /// </summary>
        /// <param name="delay">延迟时间 单位秒</param>
        /// <param name="callback">回调方法</param>
        public void SetDelayAction(float delay, Action callback)
        {
            this.delay = delay;
            this.callback = callback;
            State = TimerState.Ticking;
        }

        /// <summary>
        /// 暂停定时任务
        /// </summary>
        public bool Pause()
        {
            if (State == TimerState.Ticking)
            {
                State = TimerState.Pause;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 继续定时任务
        /// </summary>
        public bool Resume()
        {
            if (State == TimerState.Pause)
            {
                State = TimerState.Ticking;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 取消定时任务
        /// </summary>
        public bool Cancel()
        {
            if (State == TimerState.Ticking)
            {
                State = TimerState.Cancel;
                return true;
            }

            return false;
        }
        
        protected void Invoke()
        {
            callback?.Invoke();
        }

        public void Update(float deltaTime)
        {
            if (State == TimerState.Ticking)
            {
                elapseTime += deltaTime;
                if (elapseTime >= delay)
                {
                    if (!isStart)
                    {
                        isStart = true;
                        OnStart();
                    }

                    OnUpdate(deltaTime);
                }
            }
        }

        protected abstract void OnStart();
        
        protected abstract void OnUpdate(float deltaTime);

        public void Clear()
        {
            delay = 0;
            callback = null;
            isStart = false;
            elapseTime = 0;
            State = TimerState.Over;
            OnClear();
        }

        protected abstract void OnClear();

        public bool EqualsCaller(object caller)
        {
            return null != caller && (null != callback && callback.Target == caller);
        }
    }
}