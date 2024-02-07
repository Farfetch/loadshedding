using Prometheus;

using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    /// <summary>
    /// Represents a histogram metric for measuring the Http Requests Queue Time.
    /// </summary>
    public class HttpRequestsQueueTimeHistogram : MetricBase<Histogram>
    {
        private const string Description = "The time each request spent in the queue until its executed";

        internal HttpRequestsQueueTimeHistogram(
            PrometheusBase.CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        /// <inheritdoc/>
        protected override string DefaultName => "http_requests_queue_time_seconds";

        /// <summary>
        ///  Sets the value of the histogram.
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
                    Buckets = Histogram.ExponentialBuckets(0.0005, 2, 20),
                });
        }
    }
}
