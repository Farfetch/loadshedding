using Farfetch.LoadShedding.Measures;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.Configurations
{
    internal class ConcurrencyContext : IConcurrencyContext
    {
        private readonly ITaskManager _taskManager;
        private readonly RTTMeasures _measures;

        public ConcurrencyContext(ITaskManager taskManager, RTTMeasures measures)
        {
            this._taskManager = taskManager;
            this._measures = measures;
        }

        public int MaxConcurrencyLimit => this._taskManager.ConcurrencyLimit;

        public int MaxQueueSize => this._taskManager.QueueLimit;

        public int CurrentQueueCount => this._taskManager.QueueCount;

        public long AvgRTT => this._measures.AvgRTT;

        public long MinRTT => this._measures.MinRTT;

        public long PreviousAvgRTT { get; private set; }

        internal void Snapshot()
        {
            this.PreviousAvgRTT = this.AvgRTT;
        }
    }
}
