using System;

namespace Core.Timer
{

    /// <summary>
    ///  延迟执行1次的定时任务
    /// </summary>
    internal class OnceTimer : Timer
    {
        protected override void OnStart()
        {
            Invoke();
            State = TimerState.Over;
        }
        
        protected override void OnUpdate(float deltaTime)
        {
        }

        protected override void OnClear()
        {
        }
    }
}