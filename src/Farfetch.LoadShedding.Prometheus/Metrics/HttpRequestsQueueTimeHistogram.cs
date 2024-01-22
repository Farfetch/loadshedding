using Prometheus;
using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    public class HttpRequestsQueueTimeHistogram : MetricBase<Histogram>
    {
        private const string Description = "The time each request spent in the queue until its executed";

        internal HttpRequestsQueueTimeHistogram(
            PrometheusBase.CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        protected override string DefaultName => "http_requests_queue_time_seconds";

        public void Observe(string method, string priority, double value)
        {
            this.Metric?
                .WithLabels(method, priority)
                .Observe(value);
        }

        protected override Histogram Create(CollectorRegistry registry, MetricOptions options)
        {
            return PrometheusBase
                .Metrics
                .WithCustomRegistry(registry)
                .CreateHistogram(options.Name, Description, new PrometheusBase.HistogramConfiguration
                {
                    LabelNames = new[] { MetricsConstants.MethodLabel, MetricsConstants.PriorityLabel },
                    Buckets = Histogram.ExponentialBuckets(0.0005, 2, 20)
                });
        }
    }
}
