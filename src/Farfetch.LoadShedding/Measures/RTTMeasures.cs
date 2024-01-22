using System;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("Farfetch.LoadShedding.Tests")]

namespace Farfetch.LoadShedding.Measures
{
    /// <summary>
    /// Contains the round trip time measures.
    /// </summary>
    internal class RTTMeasures
    {
        /// <summary>
        /// After a service overload, response times are likely to be higher than usual. 
        /// Therefore, it becomes necessary to restore the response time metrics to their normal values. 
        /// This can be achieved by gradually decreasing the avg RTT that acts as a 
        /// mechanism to bring the response time back to its normal level.
        /// In this case it will decrease the avg RTT by 10% each iteration (normally 1 second) until it is stabilized.
        /// </summary>
        private const double StabilizationFactor = 0.9;

        private readonly double _tolerance;

        private long _numberOfExecutions;
        private double _totalTime;
        private long _avgRTT;

        /// <summary>
        /// Initializes an instance of the class defining the tolerance parameter.
        /// </summary>
        /// <param name="tolerance">A tolerance to control the trade-off between the minimum and the average RTT.</param>
        public RTTMeasures(double tolerance)
        {
            this._tolerance = tolerance;
            this.Reset();
        }

        /// <summary>
        /// Gets the average round trip time.
        /// </summary>
        public long AvgRTT => this._avgRTT;

        /// <summary>
        /// Gets the total number of executions.
        /// </summary>
        public long TotalCount => this._numberOfExecutions;

        /// <summary>
        /// Gets the minimum round trip time.
        /// </summary>
        public long MinRTT { get; private set; } = int.MaxValue;

        /// <summary>
        /// Responsible for tracking specific data to determine performance degradation.
        /// In each request: the round trip time is accumulated; the number of requests is increased; the average round trip time is calculated;
        /// and the minimum round trip time is maintained.
        /// </summary>
        /// <param name="durationInMs">Receives the duration of a request.</param>
        public void AddSample(double durationInMs)
        {
            this._totalTime += durationInMs;
            this._numberOfExecutions++;

            Interlocked.Exchange(ref this._avgRTT, (int)Math.Ceiling(this._totalTime / this._numberOfExecutions));

            if (this.AvgRTT < this.MinRTT)
            {
                this.MinRTT = this.AvgRTT;
            }
        }

        internal void RecoverFromLoad()
        {
            if (this._isRecovered)
            {
                return;
            }

            Interlocked.Exchange(ref this._totalTime, (int)Math.Floor(this._totalTime * StabilizationFactor));
            Interlocked.Exchange(ref this._avgRTT, (int)Math.Floor(this._totalTime / this._numberOfExecutions));

            if (this._isRecovered)
            {
                this.Reset();
            }
        }

        private bool _isRecovered => (this.MinRTT * this._tolerance) > this.AvgRTT;

        private void Reset()
        {
            this._numberOfExecutions = 0;
            this._totalTime = 0;
            this._avgRTT = 0;
            this.MinRTT = int.MaxValue;
        }
    }
}
