using System;
using System.Threading.Tasks;

namespace Farfetch.LoadShedding.Tasks
{
    internal static class TaskExtensions
    {
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
