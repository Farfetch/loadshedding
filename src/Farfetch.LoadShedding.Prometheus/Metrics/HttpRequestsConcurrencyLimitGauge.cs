using Prometheus;

using PrometheusBase = Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    /// <summary>
    /// Represents a gauge metric for measuring the current concurrency limit of HTTP requests.
    /// </summary>
    public class HttpRequestsConcurrencyLimitGauge : MetricBase<Gauge>
    {
        private const string Description = "The current concurrency limit";

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestsConcurrencyLimitGauge"/> class.
        /// </summary>
        /// <param name="collectorRegistry">The Prometheus collector registry.</param>
        /// <param name="options">The metric options.</param>
        internal HttpRequestsConcurrencyLimitGauge(
            PrometheusBase.CollectorRegistry collectorRegistry,
            MetricOptions options)
            : base(collectorRegistry, options)
        {
        }

        /// <inheritdoc/>
        protected override string DefaultName => "http_requests_concurrency_limit_total";

        /// <summary>
        /// Sets the value of the gauge.
        /// </summary>
        /// <param name="value">The value to set.</param>
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
