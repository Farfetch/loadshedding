using Prometheus;
using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
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
