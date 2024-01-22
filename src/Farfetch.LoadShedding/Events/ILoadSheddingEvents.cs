using Farfetch.LoadShedding.Events.Args;

namespace Farfetch.LoadShedding.Events
{
    public interface ILoadSheddingEvents
    {
        /// <summary>
        /// Event triggered when the concurrency limit is changed.
        /// </summary>
        IEvent<LimitChangedEventArgs> ConcurrencyLimitChanged { get; }

        /// <summary>
        /// Event triggered when the queue limit is changed.
        /// </summary>
        IEvent<LimitChangedEventArgs> QueueLimitChanged { get; }

        /// <summary>
        /// Event triggered when some task has been started to process.
        /// </summary>
        IEvent<ItemProcessingEventArgs> ItemProcessing { get; }

        /// <summary>
        /// Event triggered when some task has been finished.
        /// </summary>
        IEvent<ItemProcessedEventArgs> ItemProcessed { get; }

        /// <summary>
        /// Event triggered when some task has been enqueued.
        /// </summary>
        IEvent<ItemEnqueuedEventArgs> ItemEnqueued { get; }

        /// <summary>
        /// Event triggered when some task has been dequeued.
        /// </summary>
        IEvent<ItemDequeuedEventArgs> ItemDequeued { get; }

        /// <summary>
        /// Event triggered when some task has been rejected.
        /// </summary>
        IEvent<ItemRejectedEventArgs> Rejected { get; }
    }
}
