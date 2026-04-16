using System;

namespace Core.Event
{
    public class EventArgsManager : Singleton<EventArgsManager>
    {
        private IEventArgsDispatcher<int> dispatcher;

        protected override void OnInitial()
        {
            base.OnInitial();
            dispatcher = new EventArgsDispatcher<int>();
        }

        public void AddListener(int id, EventHandler<EventArgs> callback)
        {
            dispatcher.AddListener(id, callback);
        }

        public void RemoveListener(int id, EventHandler<EventArgs> callback)
        {
            dispatcher.RemoveListener(id, callback);
        }

        public void DispatchListener(int id, object sender, EventArgs args)
        {
            dispatcher.DispatchListener(id, sender, args);
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
    }
}