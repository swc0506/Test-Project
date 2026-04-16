using System;
using System.Collections.Generic;

namespace Core.Event
{
    public abstract class EventBaseDispatcher<K, T> where T : Delegate
    {
        protected Dictionary<K, GroupDelegate<T>> delegateMap;

        public EventBaseDispatcher(IEqualityComparer<K> comparer)
        {
            delegateMap = new Dictionary<K, GroupDelegate<T>>(comparer);
        }

        public EventBaseDispatcher() : this(null)
        {
        }

        protected bool CheckAddDelegate(K id, T callback)
        {
            if (null == callback)
            {
                return false;
            }

            if (!delegateMap.TryGetValue(id, out var handler))
            {
                handler = new GroupDelegate<T>();
                handler.Add(callback);
                delegateMap.Add(id, handler);
            }
            else
            {
                if (UnityEngine.Application.isEditor)
                {
                    if (HasDelegate(handler, callback))
                    {
                        throw new EventException(string.Format(
                            "This Method Already Exist,Id:{0},Name:{1},Target:{2}", id, callback.Method.Name,
                            callback.Target));
                    }
                }

                handler.Add(callback);
            }

            return true;
        }

        private bool HasDelegate(GroupDelegate<T> handler, T func)
        {
            return handler.Contains(func);
        }

        protected void CheckRemoveDelegate(K id, T func)
        {
            if (delegateMap.TryGetValue(id, out var handler))
            {
                handler.Remove(func);
            }
        }

        public void ClearListenerByCaller(object caller)
        {
            foreach (var item in delegateMap)
            {
                if (null != item.Value)
                {
                    item.Value.ClearByCaller(caller);
                }
            }
        }

        public void ClearListener(K id)
        {
            delegateMap.Remove(id);
        }

        public void ClearAll()
        {
            delegateMap.Clear();
        }
    }
}