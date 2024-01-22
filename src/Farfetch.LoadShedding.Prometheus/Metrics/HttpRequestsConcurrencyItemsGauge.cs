using Prometheus;
using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    public class HttpRequestsConcurrencyItemsGauge : MetricBase<Gauge>
    {
        private const string Description = "The current number of executions concurrently";

        internal HttpRequestsConcurrencyItemsGauge(
            CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        protected override string DefaultName => "http_requests_concurrency_items_total";

        public void Set(string method, string priority, double value)
        {
            this.Metric?
                .WithLabels(method, priority)
                .Set(value);
        }

        protected override Gauge Create(CollectorRegistry registry, MetricOptions options)
        {
            return PrometheusBase
               .Metrics
               .WithCustomRegistry(registry)
               .CreateGauge(options.Name, Description, new PrometheusBase.GaugeConfiguration
               {
                   LabelNames = new[] { MetricsConstants.MethodLabel, MetricsConstants.PriorityLabel }
               });
        }
    }
}
