using System;
using Farfetch.LoadShedding.AspNetCore.Configurators;
using Farfetch.LoadShedding.Prometheus;
using Farfetch.LoadShedding.Prometheus.Metrics;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace Farfetch.LoadShedding
{
    /// <summary>
    /// Extension methods of IAdaptativeLimiterOptions
    /// </summary>
    public static class IAdaptativeLimiterOptionsExtensions
    {
        /// <summary>
        /// Extension method to include prometheus metrics.
        /// </summary>
        /// <param name="options">The LoadSheddingOptions instance.</param>
        /// <param name="optionsDelegate">The delegate for options</param>
        /// <returns>LoadSheddingOptions</returns>
        public static LoadSheddingOptions AddMetrics(
            this LoadSheddingOptions options,
            Action<LoadSheddingMetricOptions> optionsDelegate = null)
        {
            options.SubscribeEvents((provider, events) =>
            {
                var metricsOptions = new LoadSheddingMetricOptions(Metrics.DefaultRegistry);

                optionsDelegate?.Invoke(metricsOptions);

                var concurrencyItemsGauge = new HttpRequestsConcurrencyItemsGauge(metricsOptions.Registry, metricsOptions.ConcurrencyItems);
                var concurrencyLimitGauge = new HttpRequestsConcurrencyLimitGauge(metricsOptions.Registry, metricsOptions.ConcurrencyLimit);
                var taskProcessingTimeHistogram = new HttpRequestsQueueTaskExecutionTimeHistogram(metricsOptions.Registry, metricsOptions.TaskExecutionTime);
                var queueItemsGauge = new HttpRequestsQueueItemsGauge(metricsOptions.Registry, metricsOptions.QueueItems);
                var queueLimitGauge = new HttpRequestsQueueLimitGauge(metricsOptions.Registry, metricsOptions.QueueLimit);
                var queueTimeHistogram = new HttpRequestsQueueTimeHistogram(metricsOptions.Registry, metricsOptions.QueueTime);
                var rejectedCounter = new HttpRequestsRejectedCounter(metricsOptions.Registry, metricsOptions.RequestRejected);

                var accessor = provider.GetRequiredService<IHttpContextAccessor>();

                events.SubscribeConcurrencyLimitChangedEvent(concurrencyLimitGauge);
                events.SubscribeQueueLimitChangedEvent(queueLimitGauge);
                events.SubscribeItemProcessingEvent(concurrencyItemsGauge, accessor);
                events.SubscribeItemProcessedEvent(concurrencyItemsGauge, taskProcessingTimeHistogram, accessor);
                events.SubscribeItemEnqueuedEvent(queueItemsGauge, accessor);
                events.SubscribeItemDequeuedEvent(queueItemsGauge, queueTimeHistogram, accessor);
                events.SubscribeRejectedEvent(rejectedCounter, accessor);
            });

            return options;
        }

        private static void SubscribeItemDequeuedEvent(
            this Events.ILoadSheddingEvents events,
            HttpRequestsQueueItemsGauge queueItemsGauge,
            HttpRequestsQueueTimeHistogram queueTimeHistogram,
            IHttpContextAccessor accessor)
        {
            if (!queueItemsGauge.IsEnabled && !queueTimeHistogram.IsEnabled)
            {
                return;
            }

            events.ItemDequeued.Subscribe(args =>
            {
                var method = accessor.GetMethod();

                queueItemsGauge.Decrement(method, args.Priority.FormatPriority());
                queueTimeHistogram.Observe(method, args.Priority.FormatPriority(), args.QueueTime.TotalSeconds);
            });
        }

        private static void SubscribeItemProcessedEvent(
            this Events.ILoadSheddingEvents events,
            HttpRequestsConcurrencyItemsGauge concurrencyItemsGauge,
            HttpRequestsQueueTaskExecutionTimeHistogram taskProcessingTimeHistogram,
            IHttpContextAccessor accessor)
        {
            if (!concurrencyItemsGauge.IsEnabled && !taskProcessingTimeHistogram.IsEnabled)
            {
                return;
            }

            events.ItemProcessed.Subscribe(args =>
            {
                var method = accessor.GetMethod();
                var priority = args.Priority.FormatPriority();

                concurrencyItemsGauge.Decrement(method, priority);
                taskProcessingTimeHistogram.Observe(method, priority, args.ProcessingTime.TotalSeconds);
            });
        }

        private static void SubscribeConcurrencyLimitChangedEvent(
            this Events.ILoadSheddingEvents events,
            HttpRequestsConcurrencyLimitGauge concurrencyLimitGauge)
        {
            if (concurrencyLimitGauge.IsEnabled)
            {
                events.ConcurrencyLimitChanged.Subscribe(args => concurrencyLimitGauge.Set(args.Limit));
            }
        }

        private static void SubscribeQueueLimitChangedEvent(this Events.ILoadSheddingEvents events, HttpRequestsQueueLimitGauge queueLimitGauge)
        {
            if (queueLimitGauge.IsEnabled)
            {
                events.QueueLimitChanged.Subscribe(args => queueLimitGauge.Set(args.Limit));
            }
        }

        private static void SubscribeItemProcessingEvent(
            this Events.ILoadSheddingEvents events,
            HttpRequestsConcurrencyItemsGauge concurrencyItemsGauge,
            IHttpContextAccessor accessor)
        {
            if (!concurrencyItemsGauge.IsEnabled)
            {
                return;
            }

            events.ItemProcessing.Subscribe(args =>
            {
                concurrencyItemsGauge.Increment(
                    accessor.GetMethod(),
                    args.Priority.FormatPriority());
            });
        }

        private static void SubscribeItemEnqueuedEvent(this Events.ILoadSheddingEvents events, HttpRequestsQueueItemsGauge queueItemsGauge, IHttpContextAccessor accessor)
        {
            if (!queueItemsGauge.IsEnabled)
            {
                return;
            }

            events.ItemEnqueued.Subscribe(args =>
            {
                queueItemsGauge.Increment(
                    accessor.GetMethod(),
                    args.Priority.FormatPriority());
            });
        }

        private static void SubscribeRejectedEvent(this Events.ILoadSheddingEvents events, HttpRequestsRejectedCounter rejectedCounter, IHttpContextAccessor accessor)
        {
            if (!rejectedCounter.IsEnabled)
            {
                return;
            }

            events.Rejected.Subscribe(args =>
            {
                rejectedCounter.Increment(
                    accessor.GetMethod(),
                    args.Priority.FormatPriority(),
                    args.Reason);
            });
        }

        private static string GetMethod(this IHttpContextAccessor accessor)
        {
            if (accessor?.HttpContext?.Request is null)
            {
                return MetricsConstants.Unknown;
            }

            return accessor.HttpContext.Request.Method.ToUpper();
        }

        private static string FormatPriority(this Priority priority)
        {
            return priority.ToString().ToLower();
        }
    }
}
