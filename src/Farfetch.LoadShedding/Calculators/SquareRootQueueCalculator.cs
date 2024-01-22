using System;
using Farfetch.LoadShedding.Configurations;

namespace Farfetch.LoadShedding.Calculators
{
    internal class SquareRootQueueCalculator : IQueueSizeCalculator
    {
        private readonly ConcurrencyOptions _options;

        public SquareRootQueueCalculator(ConcurrencyOptions options)
        {
            this._options = options;
        }

        public int CalculateQueueSize(IConcurrencyContext context)
        {
            return (int)Math.Max(
                this._options.MinQueueSize,
                Math.Ceiling(Math.Sqrt(context.MaxConcurrencyLimit)));
        }
    }
}
