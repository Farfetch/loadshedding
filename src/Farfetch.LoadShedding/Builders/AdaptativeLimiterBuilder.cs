using System;
using Farfetch.LoadShedding.Calculators;
using Farfetch.LoadShedding.Configurations;
using Farfetch.LoadShedding.Limiters;
using Farfetch.LoadShedding.Events;

namespace Farfetch.LoadShedding.Builders
{
    /// <summary>
    /// A Builder to configure an Adaptative Concurrency Limiter.
    /// </summary>
    public class AdaptativeLimiterBuilder
    {
        private readonly ILoadSheddingEvents _events;
        private readonly ILimitCalculator _limitCalculator;
        private IQueueSizeCalculator _queueSizeCalculator;

        private ConcurrencyOptions _options;

        /// <summary>
        /// Constructor that creates ConcurrencyOptions, MetricsManager, LimitCalculator, and QueueSizeCalculator properties by default.
        /// </summary>
        public AdaptativeLimiterBuilder()
        {
            this._options = new ConcurrencyOptions();
            this._events = new LoadSheddingEvents();
            this._limitCalculator = new GradientLimitCalculator(this._options);
        }

        /// <summary>
        /// Sets the Concurrency Options configured by the client.
        /// </summary>
        /// <param name="options">An action to configure the ConcurrencyOptions.</param>
        /// <returns>The AdaptativeLimiterBuilder updated with the options.</returns>
        public AdaptativeLimiterBuilder WithOptions(Action<ConcurrencyOptions> options)
        {
            options?.Invoke(this._options);
            return this;
        }

        /// <summary>
        /// Sets the Concurrency Options configured by the client.
        /// </summary>
        /// <param name="options">An action to configure the ConcurrencyOptions.</param>
        /// <returns>The AdaptativeLimiterBuilder updated with the options.</returns>
        public AdaptativeLimiterBuilder WithOptions(ConcurrencyOptions options)
        {
            this._options = options;
            return this;
        }

        /// <summary>
        /// Allows to subscribe limiter events.
        /// </summary>
        /// <param name="eventsListener">An action to configure what to do when a events is raised.</param>
        /// <returns>The AdaptativeLimiterBuilder updated with a events listener.</returns>
        public AdaptativeLimiterBuilder SubscribeEvents(Action<ILoadSheddingEvents> eventsListener)
        {
            eventsListener?.Invoke(this._events);
            return this;
        }

        /// <summary>
        /// Sets to a custom Queue Size Calculator configured by the client.
        /// </summary>
        /// <param name="queueSizeCalculator">An implementation of a IQueueSizeCalculator</param>
        /// <returns>The AdaptativeLimiterBuilder updated with a custom queue size calculator.</returns>
        public AdaptativeLimiterBuilder WithCustomQueueSizeCalculator(IQueueSizeCalculator queueSizeCalculator)
        {
            this._queueSizeCalculator = queueSizeCalculator;
            return this;
        }

        /// <summary>
        /// Creates an instance of an AdaptativeConcurrencyLimiter with configured properties.
        /// </summary>
        /// <returns>An intance of an AdaptativeConcurrencyLimiter.</returns>
        public IAdaptativeConcurrencyLimiter Build()
        {
            this._options.Validate();

            return new AdaptativeConcurrencyLimiter(
                this._options,
                this._limitCalculator,
                this._queueSizeCalculator ?? new SquareRootQueueCalculator(this._options),
                this._events);
        }
    }
}
