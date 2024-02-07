namespace Farfetch.LoadShedding.Tasks
{
    internal enum TaskResult
    {
        Pending,
        Waiting,
        Processing,
        Timeout,
        Rejected,
        Completed,
    }
}
