using System;
using System.Collections.Generic;

namespace Core.Event
{
    public class EventArgsDispatcher<K> : EventBaseDispatcher<K, EventHandler<EventArgs>>,
        IEventArgsDispatcher<K>
    {
        public EventArgsDispatcher(IEqualityComparer<K> comparer) : base(comparer)
        {
        }

        public EventArgsDispatcher() : base()
        {
        }

        public void AddListener(K id, EventHandler<EventArgs> callback)
        {
            CheckAddDelegate(id, callback);
        }

        public void RemoveListener(K id, EventHandler<EventArgs> callback)
        {
            CheckRemoveDelegate(id, callback);
        }

        public void DispatchListener(K id, object sender, EventArgs args)
        {
            if (delegateMap.TryGetValue(id, out var handler))
            {
                handler.Invoke(sender,args);
            }
        }
    }
}