using System;
using Farfetch.LoadShedding.Events;

namespace Farfetch.LoadShedding
{
    /// <summary>
    /// Represents an Event to be subscribed.
    /// </summary>
    /// <typeparam name="TArg">The argument expected by the event.</typeparam>
    public interface IEvent<TArg>
    {
        /// <summary>
        /// Subscribes to the event.
        /// </summary>
        /// <param name="handler">The handler to be called when the event is fired.</param>
        IEventSubscription Subscribe(Action<TArg> handler);

        /// <summary>
        /// Raises the event and notify all the subscribers.
        /// </summary>
        /// <param name="arg">The event arguments.</param>
        void Raise(TArg arg);
    }
}
