using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Farfetch.LoadShedding.Tasks
{
    internal class TaskItem : ITaskItem, IDisposable
    {
        private readonly TaskCompletionSource<bool> _taskSource;
        private readonly Stopwatch _lifetime;

        private Stopwatch _waitingTime;

        public TaskItem(Priority priority)
        {
            this.Priority = priority;
            this._lifetime = Stopwatch.StartNew();
            this._taskSource = new TaskCompletionSource<bool>();
        }

        public Priority Priority { get; }

        public TaskResult Status { get; private set; } = TaskResult.Pending;

        public TimeSpan WaitingTime => this._waitingTime?.Elapsed ?? TimeSpan.Zero;

        public TimeSpan ProcessingTime => this._lifetime.Elapsed - this.WaitingTime;

        public Dictionary<string, string> Labels { get; } = new Dictionary<string, string>();

        public Action OnCompleted { get; set; }

        public bool IsWaiting => this.Status == TaskResult.Pending || this.Status == TaskResult.Waiting;

        public async Task WaitAsync(int timeout, CancellationToken token)
        {
            if (this._taskSource.Task.IsCompleted)
            {
                return;
            }

            this.Status = TaskResult.Waiting;

            this._waitingTime = Stopwatch.StartNew();

            var timeoutTask = Task.Delay(timeout, token);

            var returnedTask = await Task
                .WhenAny(this._taskSource.Task, timeoutTask)
                .ConfigureAwait(false);

            this._waitingTime.Stop();

            if (returnedTask == timeoutTask)
            {
                this.Timeout();
            }
        }

        /// <summary>
        /// Rejects the item.
        /// </summary>
        public void Reject()
        {
            if (this.TryChangeStatus(TaskResult.Rejected))
            {
                this._lifetime.Stop();
            }
        }

        /// <summary>
        /// Completes the task.
        /// </summary>
        public void Complete()
        {
            if (this.TryChangeStatus(TaskResult.Completed))
            {
                this._lifetime.Stop();
                this.OnCompleted?.Invoke();
            }
        }

        /// <summary>
        /// Sets the task as processing.
        /// </summary>
        public void Process()
        {
            this.Status = TaskResult.Processing;
            this._taskSource.TrySetResult(true);
        }

        /// <summary>
        /// Dispose the task.
        /// </summary>
        public void Dispose()
        {
            this._waitingTime?.Stop();
            this._lifetime.Stop();
        }

        private void Timeout()
        {
            if (this.TryChangeStatus(TaskResult.Timeout))
            {
                this._lifetime.Stop();
            }
        }

        private bool TryChangeStatus(TaskResult status)
        {
            if (this.Status == status)
            {
                return false;
            }

            this.Status = status;
            this._taskSource.TrySetResult(true);

            return true;
        }
    }
}
