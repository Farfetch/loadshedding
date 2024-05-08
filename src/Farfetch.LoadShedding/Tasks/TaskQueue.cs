using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Farfetch.LoadShedding.BenchmarkTests")]

namespace Farfetch.LoadShedding.Tasks
{
    internal class TaskQueue
    {
        private readonly ConcurrentCounter _counter = new();

        private readonly TaskItemList[] _queues = new TaskItemList[3] { new(), new(), new() };

        public TaskQueue(int limit)
        {
            this._counter.Limit = limit;
        }

        public int Count => this._counter.Count;

        public int Limit
        {
            get => this._counter.Limit;
            set => this._counter.Limit = value;
        }

        public void Enqueue(TaskItem item)
        {
            int count = this.EnqueueItem(item);

            if (count > this.Limit)
            {
                this.RejectLastItem();
            }
        }

        public TaskItem Dequeue()
        {
            var nextQueueItem = this._queues
                .FirstOrDefault(x => x.HasItems)
                ?.Dequeue();

            if (nextQueueItem != null)
            {
                this.DecrementCounter();
            }

            return nextQueueItem;
        }

        public void Remove(TaskItem item)
        {
            if (this._queues[(int)item.Priority].Remove(item))
            {
                this.DecrementCounter();
            }
        }

        internal void Clear()
        {
            foreach (var queue in this._queues)
            {
                queue.Clear();
            }
        }

        private int EnqueueItem(TaskItem item)
        {
            this._queues[(int)item.Priority].Add(item);

            return this.IncrementCounter();
        }

        private void RejectLastItem()
        {
            var lastItem = this._queues
                .LastOrDefault(x => x.HasItems)
                ?.DequeueLast();

            if (lastItem == null)
            {
                return;
            }

            this.DecrementCounter();

            lastItem.Reject();
        }

        private int IncrementCounter()
        {
            return this._counter.Increment();
        }

        private int DecrementCounter()
        {
            return this._counter.Decrement();
        }
    }
}
