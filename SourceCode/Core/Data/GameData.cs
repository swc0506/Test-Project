using System;

namespace Core.Data
{
    public abstract class GameData<T> : IDataable where T : GameData<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    DataManager.Instance.RegisterData(typeof(T));
                }

                return instance;
            }
        }

        void IDataable.Initial(IDataable data)
        {
            instance = (T)data;
            OnInitial();
        }

        void IDataable.Clear()
        {
            OnClear();
        }

        void IDataable.Dispose()
        {
            OnDispose();
            instance = null;
        }

        /// <summary>
        /// 单利被初始创建时调用
        /// </summary>
        protected virtual void OnInitial()
        {
        }

        /// <summary>
        /// 单利被销毁时调用
        /// </summary>
        protected abstract void OnClear();

        protected virtual void OnDispose()
        {
        }
    }
}