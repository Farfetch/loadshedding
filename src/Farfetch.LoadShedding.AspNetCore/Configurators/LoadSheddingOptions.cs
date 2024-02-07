using System;
using System.Collections.Generic;
using Farfetch.LoadShedding.AspNetCore.Options;
using Farfetch.LoadShedding.Calculators;
using Farfetch.LoadShedding.Events;

namespace Farfetch.LoadShedding.AspNetCore.Configurators
{
    /// <summary>
    /// Represents the LoadShedding Configurator.
    /// </summary>
    public class LoadSheddingOptions
    {
        internal LoadSheddingOptions()
        {
            this.AdaptativeLimiter = new AdaptativeLimiterOptions();
            this.EventSubscriptions = new List<Action<IServiceProvider, ILoadSheddingEvents>>();
        }

        /// <summary>
        /// Gets adaptative limiter options.
        /// </summary>
        public AdaptativeLimiterOptions AdaptativeLimiter { get; }

        public IQueueSizeCalculator QueueSizeCalculator { internal get; set; }

        internal IList<Action<IServiceProvider, ILoadSheddingEvents>> EventSubscriptions { get; }

        /// <summary>
        /// Subscribes the global LoadShedding events.
        /// </summary>
        /// <param name="eventsDelegate">The action to configure the event subscribers.</param>
        public void SubscribeEvents(Action<IServiceProvider, ILoadSheddingEvents> eventsDelegate)
        {
            this.EventSubscriptions.Add(eventsDelegate);
        }

        /// <summary>
        /// Subscribes the global LoadShedding events.
        /// </summary>
        /// <param name="eventsDelegate">The action to configure the event subscribers.</param>
        public void SubscribeEvents(Action<ILoadSheddingEvents> eventsDelegate)
            => this.SubscribeEvents((_, events) => eventsDelegate?.Invoke(events));
    }
}
