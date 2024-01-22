using Prometheus;
using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    public class HttpRequestsConcurrencyLimitGauge : MetricBase<Gauge>
    {
        private const string Description = "The current concurrency limit";

        internal HttpRequestsConcurrencyLimitGauge(
            PrometheusBase.CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        protected override string DefaultName => "http_requests_concurrency_limit_total";

        public void Set(double value)
        {
            this.Metric?.Set(value);
        }

        protected override Gauge Create(CollectorRegistry registry, MetricOptions options)
        {
            return PrometheusBase
                .Metrics
                .WithCustomRegistry(registry)
                .CreateGauge(options.Name, Description);
        }
    }
}
