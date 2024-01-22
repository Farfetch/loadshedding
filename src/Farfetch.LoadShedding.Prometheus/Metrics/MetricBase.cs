using Prometheus;

namespace Farfetch.LoadShedding.Prometheus.Metrics
{
    public abstract class MetricBase<T>
    {
        protected abstract string DefaultName { get; }

        protected MetricBase(CollectorRegistry registry, MetricOptions options)
        {
            this.IsEnabled = options.Enabled;

            if (options.Enabled)
            {
                options.Name = options.Name ?? this.DefaultName;

                this.Metric = this.Create(registry, options);
            }
        }

        public bool IsEnabled { get; }

        protected T Metric { get; }

        protected abstract T Create(CollectorRegistry registry, MetricOptions options);
    }
}
