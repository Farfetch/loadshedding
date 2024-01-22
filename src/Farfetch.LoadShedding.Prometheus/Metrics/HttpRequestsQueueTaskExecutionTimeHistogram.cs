using Prometheus;
using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    public class HttpRequestsQueueTaskExecutionTimeHistogram : MetricBase<Histogram>
    {
        private const string Description = "The time each request spent in processing the task";

        internal HttpRequestsQueueTaskExecutionTimeHistogram(
            CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        protected override string DefaultName => "http_requests_task_processing_time_seconds";

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
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 20)
                });
        }
    }
}
