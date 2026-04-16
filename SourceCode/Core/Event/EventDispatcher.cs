using System;
using System.Collections.Generic;

namespace Core.Event
{
    public class EventDispatcher<K> : EventBaseDispatcher<K, Delegate>, IEventDispatcher<K>
    {
        private bool dynamicDispatch;

        public EventDispatcher(IEqualityComparer<K> comparer, bool dynamicDispatch) : base(comparer)
        {
            this.dynamicDispatch = dynamicDispatch;
        }

        public EventDispatcher(IEqualityComparer<K> comparer) : this(comparer, false)
        {
        }

        public EventDispatcher(bool dynamicDispatch) : this(null, dynamicDispatch)
        {
        }

        public EventDispatcher() : this(false)
        {
        }

        public void AddListener(K id, Action callback)
        {
            CheckAddDelegate(id, callback);
        }

        public void AddListener<T1>(K id, Action<T1> callback)
        {
            CheckAddDelegate(id, callback);
        }

        public void AddListener<T1, T2>(K id, Action<T1, T2> callback)
        {
            CheckAddDelegate(id, callback);
        }

        public void AddListener<T1, T2, T3>(K id, Action<T1, T2, T3> callback)
        {
            CheckAddDelegate(id, callback);
        }

        public void AddListener<T1, T2, T3, T4>(K id, Action<T1, T2, T3, T4> callback)
        {
            CheckAddDelegate(id, callback);
        }

        public void RemoveListener(K id, Action callback)
        {
            CheckRemoveDelegate(id, callback);
        }

        public void RemoveListener<T1>(K id, Action<T1> callback)
        {
            CheckRemoveDelegate(id, callback);
        }

        public void RemoveListener<T1, T2>(K id, Action<T1, T2> callback)
        {
            CheckRemoveDelegate(id, callback);
        }

        public void RemoveListener<T1, T2, T3>(K id, Action<T1, T2, T3> callback)
        {
            CheckRemoveDelegate(id, callback);
        }

        public void RemoveListener<T1, T2, T3, T4>(K id, Action<T1, T2, T3, T4> callback)
        {
            CheckRemoveDelegate(id, callback);
        }

        public void DispatchListener(K id)
        {
            if (delegateMap.TryGetValue(id, out var handler))
            {
                handler.Invoke();
            }
        }

        public void DispatchListener<T1>(K id, T1 args1)
        {
            if (delegateMap.TryGetValue(id, out var handler))
            {
                if (dynamicDispatch)
                {
                    handler.DynamicInvoke(args1);
                }
                else
                {
                    handler.Invoke(args1);
                }
            }
        }

        public void DispatchListener<T1, T2>(K id, T1 args1, T2 args2)
        {
            if (delegateMap.TryGetValue(id, out var handler))
            {
                if (dynamicDispatch)
                {
                    handler.DynamicInvoke(args1, args2);
                }
                else
                {
                    handler.Invoke(args1, args2);
                }
            }
        }

        public void DispatchListener<T1, T2, T3>(K id, T1 args1, T2 args2, T3 args3)
        {
            if (delegateMap.TryGetValue(id, out var handler))
            {
                if (dynamicDispatch)
                {
                    handler.DynamicInvoke(args1, args2, args3);
                }
                else
                {
                    handler.Invoke(args1, args2, args3);
                }
            }
        }

        public void DispatchListener<T1, T2, T3, T4>(K id, T1 args1, T2 args2, T3 args3, T4 args4)
        {
            if (delegateMap.TryGetValue(id, out var handler))
            {
                if (dynamicDispatch)
                {
                    handler.DynamicInvoke(args1, args2, args3, args4);
                }
                else
                {
                    handler.Invoke(args1, args2, args3, args4);
                }
            }
        }
    }
}