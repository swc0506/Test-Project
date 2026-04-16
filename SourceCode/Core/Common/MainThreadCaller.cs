using System;
using System.Collections.Concurrent;

namespace Core
{
    /// <summary>
    /// 在主线程运行方法
    /// </summary>
    public static class MainThreadCaller
    {
        private interface IInvokeable
        {
            void Invoke();
        }

        private struct ActionTask<T> : IInvokeable, IEquatable<ActionTask<T>>
        {
            private readonly Action<T> action;
            private readonly T param;

            public ActionTask(Action<T> action, T param)
            {
                this.action = action;
                this.param = param;
            }

            public void Invoke()
            {
                action.Invoke(param);
            }

            public bool Equals(ActionTask<T> other)
            {
                return Equals(action, other.action) && Equals(param, other.param);
            }

            public override bool Equals(object obj)
            {
                return obj is ActionTask<T> other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((action != null ? action.GetHashCode() : 0) * 397) ^
                           (param != null ? param.GetHashCode() : 0);
                }
            }
        }

        private struct ActionTask : IInvokeable, IEquatable<ActionTask>
        {
            private readonly Action action;

            public ActionTask(Action action)
            {
                this.action = action;
            }

            public void Invoke()
            {
                action.Invoke();
            }

            public bool Equals(ActionTask other)
            {
                return Equals(action, other.action);
            }

            public override bool Equals(object obj)
            {
                return obj is ActionTask other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (action != null ? action.GetHashCode() : 0) * 397;
                }
            }
        }

        private static readonly ConcurrentQueue<IInvokeable> tasks = new ConcurrentQueue<IInvokeable>();
        private static bool isStartup;

        public static void Startup()
        {
            if (!isStartup)
            {
                isStartup = true;
                MonoEventProxy.Instance.LateUpdateEvent += Update;
            }
        }

        /// <summary>
        /// 把该方推进主线程执行方法队列
        /// </summary>
        /// <param name="action"></param>
        public static void Call(Action action)
        {
            tasks.Enqueue(new ActionTask(action));
        }

        /// <summary>
        /// 把该方推进主线程执行方法队列
        /// </summary>
        /// <param name="action"></param>
        /// <param name="param"></param>
        public static void Call<T>(Action<T> action, T param)
        {
            tasks.Enqueue(new ActionTask<T>(action, param));
        }

        /// <summary>
        /// 方法驱动 
        /// </summary>
        private static void Update()
        {
            if (tasks.Count <= 0)
            {
                return;
            }

            while (tasks.TryDequeue(out var action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("MainThread Call Exception:{0}", e);
                }
            }
        }

        public static void Shutdown()
        {
            if (isStartup)
            {
                isStartup = false;
                MonoEventProxy.Instance.LateUpdateEvent -= Update;
            }
        }
    }
}