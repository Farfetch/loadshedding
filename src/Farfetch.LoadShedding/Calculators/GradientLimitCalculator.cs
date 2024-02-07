using System;
using Farfetch.LoadShedding.Configurations;

namespace Farfetch.LoadShedding.Calculators
{
    internal class GradientLimitCalculator : ILimitCalculator
    {
        private const double SmoothingFactor = 0.2;

        private readonly ConcurrencyOptions _options;

        public GradientLimitCalculator(ConcurrencyOptions options)
        {
            this._options = options;
        }

        /// <summary>
        /// Calculate the new concurrency limit dinamically.
        /// </summary>
        public int CalculateLimit(IConcurrencyContext context)
        {
            var currentLimit = context.MaxConcurrencyLimit;

            if (context.AvgRTT == 0)
            {
                return currentLimit;
            }

            var gradient = this.CalculateConcurrencyLimitGradient(context);

            var newLimit = (currentLimit * gradient) + context.CurrentQueueCount;

            if (newLimit < currentLimit)
            {
                newLimit = SmoothNewLimit(currentLimit, newLimit);
            }

            newLimit = Math.Min(this._options.MaxConcurrencyLimit, newLimit);

            if (KeepCurrentLimit(currentLimit, newLimit, context))
            {
                return currentLimit;
            }

            return (int)Math.Max(this._options.MinConcurrencyLimit, newLimit);
        }

        /// <summary>
        /// It calculates the new concurrency limit gradient by multiplying the minimum round trip
        /// value with the tolerance, and dividing by the round trip average value.
        /// The result should never be minor than 0.5 so that half of the requests are not lost.
        /// </summary>
        private double CalculateConcurrencyLimitGradient(IConcurrencyContext context) =>
             Math.Max(0.5, Math.Min(1, context.MinRTT * this._options.Tolerance / context.AvgRTT));

        /// <summary>
        /// If the current limit is greater than the new limit, is needed to smooth
        /// the new limit value in order to avoid abrupt changes.
        /// A reasonable value of 0.2 (20%) was chosen to do the smooth operation.
        /// The result should always be greater or equals to the minimum concurrency limit.
        /// </summary>
        private double SmoothNewLimit(double currentLimit, double newLimit) =>
            (currentLimit * (1 - SmoothingFactor)) + (SmoothingFactor * newLimit);

        /// <summary>
        /// The current limit should be kept if some criteria are accomplished.
        /// This prevents from changing to a more unstable limit.
        /// </summary>
        private static bool KeepCurrentLimit(double currentLimit, double newLimit, IConcurrencyContext context) =>
            newLimit < currentLimit && context.PreviousAvgRTT >= context.AvgRTT;
    }
}
