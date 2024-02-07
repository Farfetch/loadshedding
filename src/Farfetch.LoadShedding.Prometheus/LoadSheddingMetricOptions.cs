using Prometheus;

namespace Farfetch.LoadShedding.Prometheus
{
    public class LoadSheddingMetricOptions
    {
        internal LoadSheddingMetricOptions(CollectorRegistry registry)
        {
            this.Registry = registry;
            this.ConcurrencyItems = new MetricOptions();
            this.ConcurrencyLimit = new MetricOptions();
            this.QueueItems = new MetricOptions();
            this.QueueLimit = new MetricOptions();
            this.TaskExecutionTime = new MetricOptions();
            this.QueueTime = new MetricOptions();
            this.RequestRejected = new MetricOptions();
        }

        public CollectorRegistry Registry { get; set; }

        public MetricOptions ConcurrencyItems { get; }

        public MetricOptions ConcurrencyLimit { get; }

        public MetricOptions QueueItems { get; }

        public MetricOptions QueueLimit { get; }

        public MetricOptions TaskExecutionTime { get; }

        public MetricOptions QueueTime { get; }

        public MetricOptions RequestRejected { get; }
    }
}
