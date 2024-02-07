namespace Farfetch.LoadShedding.Prometheus
{
    /// <summary>
    /// Represents the options for configuring metrics.
    /// </summary>
    public class MetricOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the metric is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the name of the metric.
        /// </summary>
        public string Name { get; set; }
    }
}
