using System;
using System.Collections.Generic;

namespace Farfetch.LoadShedding.Tasks
{
    public interface ITaskItem
    {
        Priority Priority { get; }

        TimeSpan WaitingTime { get; }

        TimeSpan ProcessingTime { get; }

        Dictionary<string, string> Labels { get; }
    }
}
