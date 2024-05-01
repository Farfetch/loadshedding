using System;
using System.Threading;
using System.Threading.Tasks;

namespace Farfetch.LoadShedding.Tasks
{
    internal static class TaskExtensions
    {
        private static readonly Action<Task> IgnoreTaskContinuation = t => { _ = t.Exception; };

        /// <summary>
        /// Observes and ignores a potential exception on a given Task.
        /// If a Task fails and throws an exception which is never observed, it will be caught by the .NET finalizer thread.
        /// This function awaits the given task and if the exception is thrown, it observes this exception and simply ignores it.
        /// This will prevent the escalation of this exception to the .NET finalizer thread.
        /// </summary>
        /// <param name="task">The task to be ignored.</param>
        public static void IgnoreExceptions(this Task task)
        {
            if (task.IsCompleted)
            {
                _ = task.Exception;
            }
            else
            {
                task.ContinueWith(
                    IgnoreTaskContinuation,
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }
        }

        public static async Task IgnoreWhenCancelled(this Task task)
        {
            if (!task.IsCanceled)
            {
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
