using System;

namespace Core.Event
{
    public class EventManager : Singleton<EventManager>
    {
        private IEventDispatcher<int> dispatcher;

        protected override void OnInitial()
        {
            base.OnInitial();
            dispatcher = new EventDispatcher<int>();
        }

        public void AddListener(int id, Action callback)
        {
            dispatcher.AddListener(id, callback);
        }

        public void AddListener<T1>(int id, Action<T1> callback)
        {
            dispatcher.AddListener(id, callback);
        }

        public void AddListener<T1, T2>(int id, Action<T1, T2> callback)
        {
            dispatcher.AddListener(id, callback);
        }

        public void AddListener<T1, T2, T3>(int id, Action<T1, T2, T3> callback)
        {
            dispatcher.AddListener(id, callback);
        }

        public void AddListener<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> callback)
        {
            dispatcher.AddListener(id, callback);
        }

        public void RemoveListener(int id, Action callback)
        {
            dispatcher.RemoveListener(id, callback);
        }

        public void RemoveListener<T1>(int id, Action<T1> callback)
        {
            dispatcher.RemoveListener(id, callback);
        }

        public void RemoveListener<T1, T2>(int id, Action<T1, T2> callback)
        {
            dispatcher.RemoveListener(id, callback);
        }

        public void RemoveListener<T1, T2, T3>(int id, Action<T1, T2, T3> callback)
        {
            dispatcher.RemoveListener(id, callback);
        }

        public void RemoveListener<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> callback)
        {
            dispatcher.RemoveListener(id, callback);
        }

        public void DispatchListener(int id)
        {
            dispatcher.DispatchListener(id);
        }

        public void DispatchListener<T1>(int id, T1 args1)
        {
            dispatcher.DispatchListener(id, args1);
        }

        public void DispatchListener<T1, T2>(int id, T1 args1, T2 args2)
        {
            dispatcher.DispatchListener(id, args1, args2);
        }

        public void DispatchListener<T1, T2, T3>(int id, T1 args1, T2 args2, T3 args3)
        {
            dispatcher.DispatchListener(id, args1, args2, args3);
        }

        public void DispatchListener<T1, T2, T3, T4>(int id, T1 args1, T2 args2, T3 args3, T4 args4)
        {
            dispatcher.DispatchListener(id, args1, args2, args3, args4);
        }

        public void ClearListenerByCaller(object caller)
        {
            dispatcher.ClearListenerByCaller(caller);
        }

        public void ClearListener(int id)
        {
            dispatcher.ClearListener(id);
        }

        public void ClearAll()
        {
            dispatcher.ClearAll();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            ClearAll();
            dispatcher = null;
        }
    }
}