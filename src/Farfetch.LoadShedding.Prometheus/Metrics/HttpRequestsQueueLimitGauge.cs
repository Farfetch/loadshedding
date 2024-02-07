using Prometheus;
using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    public class HttpRequestsQueueLimitGauge : MetricBase<Gauge>
    {
        private const string Description = "The current queue limit size";

        internal HttpRequestsQueueLimitGauge(
            CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        /// <inheritdoc/>
        protected override string DefaultName => "http_requests_queue_limit_total";

        public void Set(double value)
        {
            this.Metric?.Set(value);
        }

        /// <inheritdoc/>
        protected override Gauge Create(CollectorRegistry registry, MetricOptions options)
        {
            return PrometheusBase
                .Metrics
                .WithCustomRegistry(registry)
                .CreateGauge(options.Name, Description);
        }
    }
}
