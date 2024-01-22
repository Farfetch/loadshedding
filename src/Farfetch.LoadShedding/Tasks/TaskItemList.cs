using System.Collections.Generic;
using System.Linq;

namespace Farfetch.LoadShedding.Tasks
{
    internal class TaskItemList
    {
        private readonly object _locker = new object();

        private readonly IList<TaskItem> _items = new List<TaskItem>();

        public bool HasItems => this._items.Count > 0;

        public void Add(TaskItem item)
        {
            lock (this._locker)
            {
                this._items.Add(item);
            }
        }

        public TaskItem Dequeue()
        {
            lock (this._locker)
            {
                var item = this._items.FirstOrDefault(x => x.IsWaiting);

                this._items.Remove(item);

                return item;
            }
        }

        public TaskItem DequeueLast()
        {
            lock (this._locker)
            {
                var item = this._items.LastOrDefault(x => x.IsWaiting);

                if (item != null)
                {
                    this._items.Remove(item);
                }

                return item;
            }
        }

        public bool Remove(TaskItem item)
        {
            lock (this._locker)
            {
                return this._items.Remove(item);
            }
        }

        public void Clear()
        {
            lock (this._locker)
            {
                this._items.Clear();
            }
        }
    }
}
