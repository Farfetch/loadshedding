using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Farfetch.LoadShedding.Tasks
{
    internal class TaskItem : IDisposable
    {
        private readonly TaskCompletionSource<bool> _taskSource;
        private readonly Stopwatch _lifetime;

        private Stopwatch _waitingTime;

        public Priority Priority { get; private set; }

        public TaskItem(Priority priority)
        {
            this.Priority = priority;
            this._lifetime = Stopwatch.StartNew();
            this._taskSource = new TaskCompletionSource<bool>();
        }

        public TaskResult Status { get; private set; } = TaskResult.Pending;

        public TimeSpan WaitingTime => this._waitingTime?.Elapsed ?? TimeSpan.Zero;

        public TimeSpan ProcessingTime => this._lifetime.Elapsed - this.WaitingTime;

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

        private void Timeout()
        {
            if (this.TryChangeStatus(TaskResult.Timeout))
            {
                this._lifetime.Stop();
            }
        }

        public void Reject()
        {
            if (this.TryChangeStatus(TaskResult.Rejected))
            {
                this._lifetime.Stop();
            }
        }

        public void Complete()
        {
            if (this.TryChangeStatus(TaskResult.Completed))
            {
                this._lifetime.Stop();
                this.OnCompleted?.Invoke();
            }
        }

        public void Process()
        {
            this.Status = TaskResult.Processing;
            this._taskSource.TrySetResult(true);
        }

        public void Dispose()
        {
            this._waitingTime?.Stop();
            this._lifetime.Stop();
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
