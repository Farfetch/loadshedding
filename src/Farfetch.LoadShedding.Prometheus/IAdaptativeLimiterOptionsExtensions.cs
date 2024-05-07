using System;
using System.Collections.Generic;
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
                var method = EnsureMethodLabel(args.Labels, accessor);
                var priority = args.Priority.FormatPriority();

                //queueItemsGauge.Set(method, priority, args.QueueCount);
                queueItemsGauge.Decrement(method, priority);
                queueTimeHistogram.Observe(method, priority, args.QueueTime.TotalSeconds);
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
                var method = EnsureMethodLabel(args.Labels, accessor);
                var priority = args.Priority.FormatPriority();

                //concurrencyItemsGauge.Set(method, priority, args.ConcurrencyCount);
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
                var method = EnsureMethodLabel(args.Labels, accessor);
                var priority = args.Priority.FormatPriority();

                //concurrencyItemsGauge.Set(method, priority, args.ConcurrencyCount);
                concurrencyItemsGauge.Increment(method, priority);
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
                var method = EnsureMethodLabel(args.Labels, accessor);
                var priority = args.Priority.FormatPriority();

                //queueItemsGauge.Set(method, priority, args.QueueCount);
                queueItemsGauge.Increment(method, priority);
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
                var method = EnsureMethodLabel(args.Labels, accessor);

                rejectedCounter.Increment(
                    method,
                    args.Priority.FormatPriority(),
                    args.Reason);
            });
        }

        private static string EnsureMethodLabel(IDictionary<string, string> labels, IHttpContextAccessor accessor)
        {
            return "GET";

            if (labels.TryGetValue(MetricsConstants.MethodLabel, out var method))
            {
                return method;
            }

            method = GetMethod(accessor);
            labels.TryAdd(MetricsConstants.MethodLabel, method);
            return method;
        }

        private static string GetMethod(this IHttpContextAccessor accessor)
        {
            if (accessor?.HttpContext?.Request is null)
            {
                return MetricsConstants.Unknown;
            }

            return accessor.HttpContext.Request.Method.ToUpper();
        }
    }
}
