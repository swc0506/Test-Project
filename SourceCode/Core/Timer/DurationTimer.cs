namespace Core.Timer
{
    /// <summary>
    /// 延迟持续几秒每帧都执行定时任务
    /// </summary>
    internal class DurationTimer : Timer
    {
        private float duration;
        
        private float finishedTime;

        /// <summary>
        /// 设置持续时间 
        /// </summary>
        /// <param name="duration">持续时间 -1代表无限时长</param>
        public void SetDuration(float duration)
        {
            this.duration = duration;
        }

        protected override void OnStart()
        {
            finishedTime = duration >= 0 ? delay + duration : -1;
        }
        
        protected override void OnUpdate(float deltaTime)
        {
            Invoke();
            if (finishedTime >= 0 && elapseTime >= finishedTime)
            {
                State = TimerState.Over;
            }
        }

        protected override void OnClear()
        {
            finishedTime = 0;
        }
    }
}