using Farfetch.LoadShedding.Events.Args;

namespace Farfetch.LoadShedding.Events
{
    /// <summary>
    /// Responsible for declare the contract of the events listener.
    /// </summary>
    internal class LoadSheddingEvents : ILoadSheddingEvents
    {
        public LoadSheddingEvents()
        {
            this.ConcurrencyLimitChanged = new Event<LimitChangedEventArgs>();
            this.QueueLimitChanged = new Event<LimitChangedEventArgs>();
            this.ItemProcessing = new Event<ItemProcessingEventArgs>();
            this.ItemProcessed = new Event<ItemProcessedEventArgs>();
            this.ItemEnqueued = new Event<ItemEnqueuedEventArgs>();
            this.ItemDequeued = new Event<ItemDequeuedEventArgs>();
            this.Rejected = new Event<ItemRejectedEventArgs>();
        }

        /// <summary>
        /// Event triggered when the concurrency limit is changed.
        /// </summary>
        public IEvent<LimitChangedEventArgs> ConcurrencyLimitChanged { get; set; }

        /// <summary>
        /// Event triggered when the queue limit is changed.
        /// </summary>
        public IEvent<LimitChangedEventArgs> QueueLimitChanged { get; set; }

        /// <summary>
        /// Event triggered when some task has been started to process.
        /// </summary>
        public IEvent<ItemProcessingEventArgs> ItemProcessing { get; set; }

        /// <summary>
        /// Event triggered when some task has been finished.
        /// </summary>
        public IEvent<ItemProcessedEventArgs> ItemProcessed { get; set; }

        /// <summary>
        /// Event triggered when some task has been enqueued.
        /// </summary>
        public IEvent<ItemEnqueuedEventArgs> ItemEnqueued { get; set; }

        /// <summary>
        /// Event triggered when some task has been dequeued.
        /// </summary>
        public IEvent<ItemDequeuedEventArgs> ItemDequeued { get; set; }

        /// <summary>
        /// Event triggered when some task has been rejected.
        /// </summary>
        public IEvent<ItemRejectedEventArgs> Rejected { get; set; }
    }
}
