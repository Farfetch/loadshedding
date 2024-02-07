using System;
using System.Threading;
using System.Threading.Tasks;
using Farfetch.LoadShedding.Calculators;
using Farfetch.LoadShedding.Configurations;
using Farfetch.LoadShedding.Events;
using Farfetch.LoadShedding.Measures;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Limiters
{
    internal class AdaptativeConcurrencyLimiter : IAdaptativeConcurrencyLimiter, IDisposable
    {
        private const int MaxPercentageForRecovering = 10;
        private const int MinPercentageForUpdatingLimits = 50;

        private static readonly TimeSpan s_updateCheckInterval = TimeSpan.FromMilliseconds(500);

        private readonly Timer _updater;
        private readonly ITaskManager _taskManager;
        private readonly ConcurrencyContext _context;
        private readonly ILimitCalculator _limitCalculator;
        private readonly IQueueSizeCalculator _queueCalculator;
        private readonly RTTMeasures _measures;

        public AdaptativeConcurrencyLimiter(
            ConcurrencyOptions concurrencyOptions,
            ILimitCalculator limitCalculator,
            IQueueSizeCalculator queueCalculator,
            ILoadSheddingEvents events)
        {
            this._limitCalculator = limitCalculator;
            this._queueCalculator = queueCalculator;

            this._measures = new RTTMeasures(concurrencyOptions.Tolerance);

            this._taskManager = new TaskManager(
                concurrencyOptions.InitialConcurrencyLimit,
                concurrencyOptions.InitialQueueSize,
                concurrencyOptions.QueueTimeoutInMs,
                events);

            this._context = new ConcurrencyContext(this._taskManager, this._measures);
            this._updater = new Timer(_ => this.UpdateLimits(), null, TimeSpan.Zero, s_updateCheckInterval);
        }

        /// <summary>
        /// It is responsible for managing the adaptative semaphore for the current request.
        /// As soon as the request is executed, the data is collected to determine the performance degradation.
        /// </summary>
        public Task ExecuteAsync(Func<Task> function, CancellationToken cancellationToken = default)
        {
            return this.ExecuteAsync(0, function, cancellationToken);
        }

        /// <summary>
        /// It is responsible for managing the adaptative semaphore for the current request.
        /// As soon as the request is executed, the data is collected to determine the performance degradation.
        /// </summary>
        public async Task ExecuteAsync(Priority priority, Func<Task> function, CancellationToken cancellationToken = default)
        {
            using (var item = await _taskManager.AcquireAsync(priority, cancellationToken))
            {
                try
                {
                    var delayTask = Task.Delay(Timeout.Infinite, cancellationToken);

                    var resultTask = await Task.WhenAny(
                         function.Invoke(),
                         delayTask);

                    if (delayTask == resultTask)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                finally
                {
                    item.Complete();

                    this._measures.AddSample(item.ProcessingTime.TotalMilliseconds);
                }
            }
        }

        public void Dispose()
        {
            this._updater.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Responsible for adapt the concurrency limit and the max queue size according with the service performance in runtime based.
        /// </summary>
        private void UpdateLimits()
        {
            try
            {
                var currentPercentage = this._taskManager.UsagePercentage;

                if (currentPercentage <= MaxPercentageForRecovering)
                {
                    this._measures.RecoverFromLoad();
                    return;
                }

                if (currentPercentage >= MinPercentageForUpdatingLimits)
                {
                    this._taskManager.ConcurrencyLimit = this._limitCalculator.CalculateLimit(this._context);
                    this._taskManager.QueueLimit = this._queueCalculator.CalculateQueueSize(this._context);
                }
            }
            finally
            {
                this._context.Snapshot();
            }
        }
    }
}
