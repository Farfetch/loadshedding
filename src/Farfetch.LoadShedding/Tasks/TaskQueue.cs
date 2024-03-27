using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Farfetch.LoadShedding.BenchmarkTests")]

namespace Farfetch.LoadShedding.Tasks
{
    internal class TaskQueue
    {
        private readonly ConcurrentCounter _counter = new();

        private readonly IDictionary<Priority, TaskItemList> _queues = new SortedDictionary<Priority, TaskItemList>()
        {
            [Priority.Critical] = new TaskItemList(),
            [Priority.Normal] = new TaskItemList(),
            [Priority.NonCritical] = new TaskItemList(),
        };

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

        public Action<int, TaskItem> OnItemEnqueued { get; set; }

        public Action<int, TaskItem> OnItemDequeued { get; set; }

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
                .FirstOrDefault(x => x.Value.HasItems)
                .Value?
                .Dequeue();

            if (nextQueueItem != null)
            {
                this.DecrementCounter(nextQueueItem);
            }

            return nextQueueItem;
        }

        public void Remove(TaskItem item)
        {
            if (this._queues[item.Priority].Remove(item))
            {
                this.DecrementCounter(item);
            }
        }

        internal void Clear()
        {
            foreach (var queue in this._queues)
            {
                queue.Value.Clear();
            }
        }

        private int EnqueueItem(TaskItem item)
        {
            this._queues[item.Priority].Add(item);

            return IncrementCounter(item);
        }

        private void RejectLastItem()
        {
            var lastItem = this._queues
                .LastOrDefault(x => x.Value.HasItems)
                .Value?
                .DequeueLast();

            if (lastItem == null)
            {
                return;
            }

            this.DecrementCounter(lastItem);

            lastItem.Reject();
        }

        private int IncrementCounter(TaskItem item)
        {
            var count = this._counter.Increment();
            this.OnItemEnqueued?.Invoke(count, item);

            return count;
        }

        private int DecrementCounter(TaskItem nextQueueItem)
        {
            var count = this._counter.Decrement();
            this.OnItemDequeued?.Invoke(count, nextQueueItem);

            return count;
        }
    }
}
