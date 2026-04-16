using System;

namespace Core
{
    public class Singleton<T> : IDisposable where T : Singleton<T>
    {
        private static T instance;
        private static readonly object sysLock = new object();

        public static T Instance
        {
            get { return GetInstance(); }
        }

        public static T GetInstance()
        {
            if (null == instance)
            {
                lock (sysLock)
                {
                    disposed = false;
                    instance = Activator.CreateInstance<T>();
                    SingletonRecorder.Add(instance);
                    instance.OnInitial();
                }
            }

            return instance;
        }

        private static bool disposed;

        public static bool Disposed
        {
            get { return disposed; }
        }

        public void Dispose()
        {
            OnDispose();
            disposed = true;
            instance = null;
        }

        /// <summary>
        /// 单例被初始创建时调用
        /// </summary>
        protected virtual void OnInitial()
        {
        }

        /// <summary>
        /// 单例被销毁时调用
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}