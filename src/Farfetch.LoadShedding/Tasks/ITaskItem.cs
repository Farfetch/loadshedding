using System;

namespace Farfetch.LoadShedding.Tasks
{
    public interface ITaskItem
    {
        string Method { get; }

        Priority Priority { get; }

        TimeSpan WaitingTime { get; }

        TimeSpan ProcessingTime { get; }
    }
}
