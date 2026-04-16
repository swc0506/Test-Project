namespace Core.Timer
{
    /// <summary>
    /// 延迟每间隔几秒执行1次的定时任务
    /// </summary>
    internal class RepeatTimer : Timer
    {
        private int repeat;
        private float interval;

        private float nextTime;
        private int curCount;

        /// <summary>
        /// 设置重复次数和间隔时间
        /// </summary>
        /// <param name="repeat">重复次数 -1代表无限次</param>
        /// <param name="interval">间隔时间单位秒</param>
        public void SetRepeat(int repeat, float interval)
        {
            this.repeat = repeat;
            this.interval = interval;
            //防止参数错误导致死循环
            if (this.repeat < 0 && this.interval <= 0)
            {
                this.repeat = 0;
            }
        }

        protected override void OnStart()
        {
            nextTime = delay;
        }
        
        protected override void OnUpdate(float deltaTime)
        {
            //即使卡帧，也会严格根据流失时间和间隔时间，执行对应次数
            while (nextTime >= 0 && elapseTime >= nextTime)
            {
                Invoke();
                curCount++;
                if (repeat >= 0 && curCount > repeat)
                {
                    State = TimerState.Over;
                    nextTime = -1;
                }
                else
                {
                    nextTime = delay + interval * curCount;
                }
            }
        }

        protected override void OnClear()
        {
            interval = 0;
            repeat = 0;
            nextTime = -1;
            curCount = 0;
        }
    }
}