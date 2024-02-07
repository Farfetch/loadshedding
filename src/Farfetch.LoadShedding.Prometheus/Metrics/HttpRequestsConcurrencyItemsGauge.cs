using Prometheus;

using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    /// <summary>
    /// Represents a gauge metric for measuring the Concurrency Items.
    /// </summary>
    public class HttpRequestsConcurrencyItemsGauge : MetricBase<Gauge>
    {
        private const string Description = "The current number of executions concurrently";

        internal HttpRequestsConcurrencyItemsGauge(
            CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        /// <inheritdoc/>
        protected override string DefaultName => "http_requests_concurrency_items_total";

        /// <summary>
        /// Sets the value of the gauge.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="value">The value.</param>
        public void Set(string method, string priority, double value)
        {
            this.Metric?
                .WithLabels(method, priority)
                .Set(value);
        }

        /// <inheritdoc/>
        protected override Gauge Create(CollectorRegistry registry, MetricOptions options)
        {
            return PrometheusBase
               .Metrics
               .WithCustomRegistry(registry)
               .CreateGauge(options.Name, Description, new PrometheusBase.GaugeConfiguration
               {
                   LabelNames = new[] { MetricsConstants.MethodLabel, MetricsConstants.PriorityLabel },
               });
        }
    }
}
