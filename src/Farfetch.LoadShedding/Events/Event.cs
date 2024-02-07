using System;
using System.Collections.Generic;
using Farfetch.LoadShedding.Events;

namespace Farfetch.LoadShedding
{
    internal class Event<TArg> : IEvent<TArg>
    {
        private readonly List<Action<TArg>> _handlers = new List<Action<TArg>>();

        public IEventSubscription Subscribe(Action<TArg> handler)
        {
            this._handlers.Add(handler);
            return new EventSubscription(() => this._handlers.Remove(handler));
        }

        public void Raise(TArg arg)
        {
            foreach (var handler in this._handlers)
            {
                try
                {
                    if (handler is null)
                    {
                        continue;
                    }

                    handler.Invoke(arg);
                }
                catch (Exception)
                {
                    // Exceptions on the subscribers should be ignored.
                }
            }
        }
    }
}
