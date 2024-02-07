using Prometheus;

using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    /// <summary>
    /// Represents a histogram metric for measuring the Http Requests Queue Task Execution Time.
    /// </summary>
    public class HttpRequestsQueueTaskExecutionTimeHistogram : MetricBase<Histogram>
    {
        private const string Description = "The time each request spent in processing the task";

        internal HttpRequestsQueueTaskExecutionTimeHistogram(
            CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        /// <inheritdoc/>
        protected override string DefaultName => "http_requests_task_processing_time_seconds";

        /// <summary>
        /// Sets the value of the histogram.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="value">The value.</param>
        public void Observe(string method, string priority, double value)
        {
            this.Metric?
                .WithLabels(method, priority)
                .Observe(value);
        }

        /// <inheritdoc/>
        protected override Histogram Create(CollectorRegistry registry, MetricOptions options)
        {
            return PrometheusBase
                .Metrics
                .WithCustomRegistry(registry)
                .CreateHistogram(options.Name, Description, new PrometheusBase.HistogramConfiguration
                {
                    LabelNames = new[] { MetricsConstants.MethodLabel, MetricsConstants.PriorityLabel },
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 20),
                });
        }
    }
}
