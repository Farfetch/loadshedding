using System.Threading.Tasks;
using System.Threading;

namespace Farfetch.LoadShedding.Tasks
{
    internal interface ITaskManager
    {
        int ConcurrencyLimit { get; set; }

        int ConcurrencyCount { get; }

        int QueueLimit { get; set; }

        int QueueCount { get; }

        double UsagePercentage { get; }

        Task<TaskItem> AcquireAsync(Priority priority, CancellationToken cancellationToken = default);
    }
}
