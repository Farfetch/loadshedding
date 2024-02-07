using Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    /// <summary>
    /// Represents the base for a metric class.
    /// </summary>
    /// <typeparam name="T">Type of the metric.</typeparam>
    public abstract class MetricBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricBase{T}"/> class.
        /// </summary>
        /// <param name="registry">The collector registry.</param>
        /// <param name="options">The options.</param>
        protected MetricBase(CollectorRegistry registry, MetricOptions options)
        {
            this.IsEnabled = options.Enabled;

            if (options.Enabled)
            {
                options.Name = options.Name ?? this.DefaultName;

                this.Metric = this.Create(registry, options);
            }
        }

        /// <summary>
        /// Gets a value indicating whether gets the value of the metric enabled.
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// Gets the default metric name.
        /// </summary>
        protected abstract string DefaultName { get; }

        /// <summary>
        /// Gets the metric instance.
        /// </summary>
        protected T Metric { get; }

        /// <summary>
        /// Creates a metric.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        protected abstract T Create(CollectorRegistry registry, MetricOptions options);
    }
}
