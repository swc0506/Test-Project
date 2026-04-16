using System;

namespace Core
{
    public abstract class AbstractListenerValue<T>
    {
        private T currValue;
        private T prevValue;

        public T Value
        {
            get { return currValue; }
            set
            {
                if (!currValue.Equals(value))
                {
                    prevValue = currValue;
                    currValue = value;
                    OnChangeValue();
                }
            }
        }

        public T PrevValue
        {
            get { return prevValue; }
        }
        
        protected abstract void OnChangeValue();

        public void Clear()
        {
            currValue = prevValue = default(T);
        }
    }

    public class ListenerValue<T> : AbstractListenerValue<T>
    {
        private Action<ListenerValue<T>> changedEvent;

        /// <summary>
        /// 当值发生变化时候的事件
        /// </summary>
        public event Action<ListenerValue<T>> ChangedEvent
        {
            add { changedEvent += value; }
            remove { changedEvent -= value; }
        }

        protected override void OnChangeValue()
        {
            changedEvent?.Invoke(this);
        }
    }

    public class ListenerValue<T, U> : AbstractListenerValue<T>
    {
        public U UserData { get; private set; }
        
        private Action<ListenerValue<T,U>> changedEvent;

        
        public void SetUserData(U userData)
        {
            this.UserData = userData;
        }

        
        /// <summary>
        /// 当值发生变化时候的事件  参数1:当前值 参数2:上次值
        /// </summary>
        public event Action<ListenerValue<T,U>> ChangedEvent
        {
            add { changedEvent += value; }
            remove { changedEvent -= value; }
        }

        protected override void OnChangeValue()
        {
            changedEvent?.Invoke(this);
        }
    }
}