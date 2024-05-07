using Prometheus;
using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    /// <summary>
    /// Represents a gauge metric for measuring the Http Requests Queue Items.
    /// </summary>
    public class HttpRequestsQueueItemsGauge : MetricBase<Gauge>
    {
        private const string Description = "The current number of items waiting to be processed in the queue";

        internal HttpRequestsQueueItemsGauge(
            CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        /// <inheritdoc/>
        protected override string DefaultName => "http_requests_queue_items_total";

        ///// <summary>
        ///// Sets the value of the gauge.
        ///// </summary>
        ///// <param name="method">The method.</param>
        ///// <param name="priority">The priority.</param>
        ///// <param name="value">The value.</param>
        //public void Set(string method, string priority, double value)
        //{
        //    this.Metric?
        //        .WithLabels(method, priority)
        //        .Set(value);
        //}

        /// <summary>
        /// Increments the value of the gauge.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="priority">The priority.</param>
        public void Increment(string method, string priority)
        {
            this.Metric?
                .WithLabels(method, priority)
                .Inc();
        }

        /// <summary>
        /// Decrements the value of the gauge.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="priority">The priority.</param>
        public void Decrement(string method, string priority)
        {
            this.Metric?
                .WithLabels(method, priority)
                .Dec();
        }

        /// <inheritdoc/>
        protected override Gauge Create(CollectorRegistry registry, MetricOptions options)
        {
            return PrometheusBase
                .Metrics
                .WithCustomRegistry(registry)
                .CreateGauge(options.Name, Description, new GaugeConfiguration
                {
                    LabelNames = new[]
                    {
                        MetricsConstants.MethodLabel,
                        MetricsConstants.PriorityLabel,
                    },
                });
        }
    }
}
