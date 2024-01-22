using System;

namespace Farfetch.LoadShedding.Events
{
    internal class EventSubscription : IEventSubscription
    {
        private readonly Action _cancelHandler;

        public EventSubscription(Action cancelHandler)
        {
            this._cancelHandler = cancelHandler;
        }

        public void Cancel()
        {
            this._cancelHandler.Invoke();
        }
    }
}
