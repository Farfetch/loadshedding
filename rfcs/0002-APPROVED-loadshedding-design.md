---
- Feature Name: loadshedding-design
- Start Date: 2022-12-12
---

# Summary
[summary]: #summary

LoadShedding is a set of techniques used by high performance web services to detect when there is a traffic congestion and identify the requests that should be served to prevent a service overload and cascading failures.

# Motivation
[motivation]: #motivation

Given the high-performance scenarios existent, where there are a lot of very important services in the critical flow, it is important to ensure resilience and avoid outages by detecting when there is an overload on the services and applying mechanisms to answer the maximum of requests without decrease the service quality, and also scale and redistribute the requests when the load is increasing.

For that, the LoadShedding library was created to serve as an observer the internal service performance, detect the service limit from inside and reject the requests when the limit is reached.

# Guide Implementation
[guide-level-explanation]: #guide-level-explanation

The idea is to create a layer to encapsulate the request and track the service performance, machine resource usage and reject when it's not possible to receive the request.

## How to Configure

[configuration]: #configuration

An extension will be provided to add a middleware to perform the logic to track, detect and control the request traffic.

```csharp
 app.UseLoadShedding((provider, builder) =>
    {
        builder
            .UseCPUUsageLimit(99)
            .UseMemoryUsageLimit(99)
            .UseAdaptativeLimiter(options =>
            {   
                options.PartitionResolver = (context) => context.Request.Path;             
                options.QueueSizeStrategy = QueueSizeStrategies.SquareRoot; 
                options.MinConcurrencyLimit = {{serviceSettings.Resilience.MaxInFlight.AdaptativeConcurrencyLimits.MinConcurrencyLimit}};
                options.MaxConcurrencyLimit = {{serviceSettings.Resilience.MaxInFlight.AdaptativeConcurrencyLimits.MaxConcurrencyLimit}};
            })
            .WithMetricsListener(listener =>
            {
                listener.OnChanged = metrics =>
                {
                    metricsHandler.MetricsChanged(metrics);
                };

                listener.OnReject = () =>
                {
                    metricsHandler.Reject();
                };
            });
    });
```

# Drawbacks
[drawbacks]: #drawbacks

It is a solution that needs to be maintained internally by a team even if it is open source.
On the other hand, making it open source will enable the contribution of the dotnet community.

# Rationale / Alternatives
[rationale-and-alternatives]: #rationale-and-alternatives

The rationale for creating a dotnet library is to assist, and have and easy and simple way of having these techniques in services.

Another alternative is to create load balancing or gateway plugins with this logic, it could be used in a layer above and it could be reused by any technology.