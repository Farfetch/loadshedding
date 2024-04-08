using System.Threading;
using System.Threading.Tasks;

namespace Farfetch.LoadShedding.Tasks
{
    internal interface ITaskManager
    {
        int ConcurrencyLimit { get; set; }

        int ConcurrencyCount { get; }

        int QueueLimit { get; set; }

        int QueueCount { get; }

        double UsagePercentage { get; }

        Task<TaskItem> AcquireAsync(Priority priority, string method = null, CancellationToken cancellationToken = default);
    }
}
