using Farfetch.LoadShedding.Events.Args;

namespace Farfetch.LoadShedding.Events
{
    public interface ILoadSheddingEvents
    {
        /// <summary>
        /// Gets event triggered when the concurrency limit is changed.
        /// </summary>
        IEvent<LimitChangedEventArgs> ConcurrencyLimitChanged { get; }

        /// <summary>
        /// Gets event triggered when the queue limit is changed.
        /// </summary>
        IEvent<LimitChangedEventArgs> QueueLimitChanged { get; }

        /// <summary>
        /// Gets event triggered when some task has been started to process.
        /// </summary>
        IEvent<ItemProcessingEventArgs> ItemProcessing { get; }

        /// <summary>
        /// Gets event triggered when some task has been finished.
        /// </summary>
        IEvent<ItemProcessedEventArgs> ItemProcessed { get; }

        /// <summary>
        /// Gets event triggered when some task has been enqueued.
        /// </summary>
        IEvent<ItemEnqueuedEventArgs> ItemEnqueued { get; }

        /// <summary>
        /// Gets event triggered when some task has been dequeued.
        /// </summary>
        IEvent<ItemDequeuedEventArgs> ItemDequeued { get; }

        /// <summary>
        /// Gets event triggered when some task has been rejected.
        /// </summary>
        IEvent<ItemRejectedEventArgs> Rejected { get; }
    }
}
