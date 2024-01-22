using System;
using System.Collections.Generic;
using System.Text;

namespace Farfetch.LoadShedding.Prometheus
{
    public class MetricOptions
    {
        public bool Enabled { get; set; } = true;

        public string Name { get; set; }
    }
}
