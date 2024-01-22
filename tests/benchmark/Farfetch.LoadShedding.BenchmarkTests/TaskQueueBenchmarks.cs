using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.BenchmarkTests
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [Orderer(SummaryOrderPolicy.Declared)]
    [MinColumn, MaxColumn]
    [IterationCount(10)]
    [RankColumn]
    public class TaskQueueBenchmarks
    {
        private readonly TaskQueue _queue = new TaskQueue(int.MaxValue);

        private readonly TaskQueue _emptyQueue = new TaskQueue(int.MaxValue);

        private readonly TaskQueue _limitedQueue = new TaskQueue(1000);

        [IterationSetup]
        public void Initialize()
        {
            this._queue.Clear();
            this._emptyQueue.Clear();
            this._limitedQueue.Clear();

            while (this._queue.Count < 1000)
            {
                this._queue.Enqueue(GetTaskRandomPriority());
            }

            while (this._limitedQueue.Count < 1000)
            {
                this._limitedQueue.Enqueue(GetTaskRandomPriority());
            }
        }

        [Benchmark]
        public void TaskQueueWith1000Items_EnqueueFixedPriority() => this._queue.Enqueue(new TaskItem(0));

        [Benchmark]
        public void TaskQueueEmpty_EnqueueRandomPriority() => this._emptyQueue.Enqueue(GetTaskRandomPriority());

        [Benchmark]
        public void TaskQueueWith1000Items_EnqueueRandomPriority() => this._queue.Enqueue(GetTaskRandomPriority());

        [Benchmark]
        public void TaskQueueWith1000Items_Dequeue() => this._queue.Dequeue();

        [Benchmark]
        public void TaskQueue_EnqueueNewItem_LimitReached() => this._limitedQueue.Enqueue(new TaskItem(0));

        private static TaskItem GetTaskRandomPriority()
        {
            return new TaskItem((Priority)Random.Shared.Next(3));
        }
    }
}
