using System;

namespace Core.Event
{
    internal class EventException : Exception
    {
        public EventException(string message)
            : base(message)
        {
        }
    }
}