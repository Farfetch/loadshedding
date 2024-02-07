using Prometheus;

namespace Farfetch.LoadShedding.Prometheus
{
    /// <summary>
    /// Represents the options for configuring load shedding metrics.
    /// </summary>
    public class LoadSheddingMetricOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadSheddingMetricOptions"/> class with the specified <see cref="CollectorRegistry"/>.
        /// </summary>
        /// <param name="registry">The Prometheus CollectorRegistry instance to use for metrics.</param>
        internal LoadSheddingMetricOptions(CollectorRegistry registry)
        {
            this.Registry = registry;
            this.ConcurrencyItems = new MetricOptions();
            this.ConcurrencyLimit = new MetricOptions();
            this.QueueItems = new MetricOptions();
            this.QueueLimit = new MetricOptions();
            this.TaskExecutionTime = new MetricOptions();
            this.QueueTime = new MetricOptions();
            this.RequestRejected = new MetricOptions();
        }

        /// <summary>
        /// Gets or sets the Prometheus CollectorRegistry instance to use for metrics.
        /// </summary>
        public CollectorRegistry Registry { get; set; }

        /// <summary>
        /// Gets the options for the concurrency items metric.
        /// </summary>
        public MetricOptions ConcurrencyItems { get; }

        /// <summary>
        /// Gets the options for the concurrency limit metric.
        /// </summary>
        public MetricOptions ConcurrencyLimit { get; }

        /// <summary>
        /// Gets the options for the queue items metric.
        /// </summary>
        public MetricOptions QueueItems { get; }

        /// <summary>
        /// Gets the options for the queue limit metric.
        /// </summary>
        public MetricOptions QueueLimit { get; }

        /// <summary>
        /// Gets the options for the task execution time metric.
        /// </summary>
        public MetricOptions TaskExecutionTime { get; }

        /// <summary>
        /// Gets the options for the queue time metric.
        /// </summary>
        public MetricOptions QueueTime { get; }

        /// <summary>
        /// Gets the options for the request rejected metric.
        /// </summary>
        public MetricOptions RequestRejected { get; }
    }
}
