using Prometheus;
using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    public class HttpRequestsRejectedCounter : MetricBase<Counter>
    {
        private const string Description = "The number of requests rejected because the queue limit is reached";

        internal HttpRequestsRejectedCounter(
            CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        /// <inheritdoc/>
        protected override string DefaultName => "http_requests_rejected_total";

        public void Increment(string method, string priority, string reason)
        {
            this.Metric?.WithLabels(method, priority, reason).Inc();
        }

        /// <inheritdoc/>
        protected override Counter Create(CollectorRegistry registry, MetricOptions options)
        {
            return PrometheusBase.Metrics
                .WithCustomRegistry(registry)
                .CreateCounter(options.Name ?? DefaultName, Description, new CounterConfiguration
                {
                    LabelNames = new[]
                    {
                        MetricsConstants.MethodLabel,
                        MetricsConstants.PriorityLabel,
                        MetricsConstants.ReasonLabel,
                    },
                });
        }
    }
}
