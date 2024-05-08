namespace Farfetch.LoadShedding.Tasks
{
    internal class ConcurrentCounter
    {
        private readonly object _locker = new object();

        private int _count = 0;

        public int Count => this._count;

        public int Limit { get; set; }

        public int AvailableCount
        {
            get
            {
                lock (this._locker)
                {
                    return this.Limit - this._count;
                }
            }
        }

        public double UsagePercentage
        {
            get
            {
                lock (this._locker)
                {
                    return (this._count * 100.0) / this.Limit;
                }
            }
        }

        public int Increment()
        {
            lock (this._locker)
            {
                this._count++;

                return _count;
            }
        }

        public bool TryIncrement(out int result)
        {
            var increased = false;

            lock (this._locker)
            {
                if (this._count < this.Limit)
                {
                    this._count++;
                    increased = true;
                }

                result = this._count;
            }

            return increased;
        }

        public int Decrement()
        {
            lock (this._locker)
            {
                this._count--;

                return this._count;
            }
        }
    }
}
