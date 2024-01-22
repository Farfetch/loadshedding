# LoadShedding

This library provides a set of capabilities to enable the service to deal with requests overload and resource limits to avoid outages and to ensure the best usage of the service capacity.

## Documentation

<details open="open">
<summary>Getting Started</summary>

- [Installation](docs/getting_started/installation.md)
- Register Limits
  - [Concurrency Adaptative Limiter](docs/getting_started/register_concurrency_adaptative_limiter.md)
- [Samples](samples)

</details>

## Samples

For sample projects showcasing the application types, dependencies and features please check the [samples](samples/) folder.

## Contributing

Read our [contributing guidelines](CONTRIBUTING.md) to learn about our development process, how to propose bugfixes and improvements, and how to build and test your changes.

### Requirements

- Makefile
- .NET 6
- Docker
- Docker Compose

#### Makefile - Simplify Tests and Build commands

**Important: You must have the Make app installed and configured in your local environment.**

With the makefile present in the Load-Shedding, the teams or engineers who want to contribute to the development of the Load-Shedding library, have a simplified way to run the tests and build the project, the commands are shown below and will wrap and use the same commands that the ones used in the pipeline.

Note: You should be in the root folder of the repository locally, to allow you run the commands.

### Building

To build the solution file, you can simply use this command.

```bash
make build
```

#### Testing (Unit)

To run all the tests, you can simply use this command.

```bash
make unit-tests
```

The command will build the solution and then run all the tests present and marked as Unit tests.

#### Testing (Integration)

To run all the integration tests, you can simply use this command.

```bash
make integration-tests
```

The command will build the solution and then run all the integration tests.

### Benchmark

Below, it is possible to see a benchmark analysis of the concurrency control mechanism, for this test multiple scenarios were created:

- **Limiter_Default:** Directly tests the AdaptiveConcurrencyLimiter with default priority;
- **Limiter_RandomPriority:** Directly tests the AdaptiveConcurrencyLimiter with random priorities;
- **LimiterMiddleware_Default:** Tests the ConcurrencyLimiterMiddleware that uses the AdaptiveConcurrencyLimiter with Http Requests context with default priority.
- **LimiterMiddleware_RandomPriority:** Tests the ConcurrencyLimiterMiddleware that uses the AdaptiveConcurrencyLimiter with Http Requests context with random priorities.

- **TaskQueueWith1000Items_EnqueueFixedPriority:** Tests the TaskQueue.Enqueue pre-loaded with 1000 items and a default priority;
- **TaskQueueEmpty_EnqueueRandomPriority:** Tests the TaskQueue.Enqueue with no elements;
- **TaskQueueWith1000Items_EnqueueRandomPriority:** Tests the TaskQueue.Enqueue pre-loaded with 1000 items and random priorities;
- **TaskQueueWith1000Items_Dequeue:** Tests the TaskQueue.Dequeue pre-loaded with 1000 items;
- **TaskQueue_EnqueueNewItem_LimitReached:** Tests the TaskQueue.Enqueue pre-loaded with 1000 items and the queue limit reached;

#### Limiter

``` ini
BenchmarkDotNet=v0.13.4, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i7-10610U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.103
  [Host]     : .NET 6.0.14 (6.0.1423.7309), X64 RyuJIT AVX2
  Job-VLMTWN : .NET 6.0.14 (6.0.1423.7309), X64 RyuJIT AVX2

IterationCount=10  
```

|                           Method |     Mean |    Error |   StdDev |      Min |      Max | Rank | Completed Work Items | Lock Contentions |   Gen0 | Allocated |
|--------------------------------- |---------:|---------:|---------:|---------:|---------:|-----:|---------------------:|-----------------:|-------:|----------:|
|                  Limiter_Default | 354.4 ns |  6.22 ns |  3.25 ns | 349.2 ns | 358.4 ns |    1 |               0.0000 |                - | 0.1450 |     608 B |
|           Limiter_RandomPriority | 366.3 ns |  4.82 ns |  2.52 ns | 363.6 ns | 369.6 ns |    2 |               0.0000 |                - | 0.1450 |     608 B |
|        LimiterMiddleware_Default | 436.6 ns | 28.55 ns | 16.99 ns | 416.7 ns | 471.4 ns |    3 |               0.0000 |                - | 0.1855 |     776 B |
| LimiterMiddleware_RandomPriority | 468.7 ns |  6.55 ns |  3.90 ns | 463.7 ns | 475.3 ns |    4 |               0.0000 |                - | 0.2027 |     848 B |

##### TaskQueue

``` ini
BenchmarkDotNet=v0.13.4, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i7-10610U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.103
  [Host]     : .NET 6.0.14 (6.0.1423.7309), X64 RyuJIT AVX2
  Job-THBOTE : .NET 6.0.14 (6.0.1423.7309), X64 RyuJIT AVX2

InvocationCount=1  IterationCount=10  UnrollFactor=1  

```

|                                       Method |      Mean |     Error |    StdDev |      Min |       Max | Rank | Completed Work Items | Lock Contentions | Allocated |
|--------------------------------------------- |----------:|----------:|----------:|---------:|----------:|-----:|---------------------:|-----------------:|----------:|
|  TaskQueueWith1000Items_EnqueueFixedPriority | 11.311 μs | 3.8278 μs | 2.2779 μs | 6.800 μs | 14.800 μs |    4 |                    - |                - |     896 B |
|         TaskQueueEmpty_EnqueueRandomPriority |  6.700 μs | 3.8089 μs | 2.2666 μs | 4.800 μs | 11.800 μs |    2 |                    - |                - |     896 B |
| TaskQueueWith1000Items_EnqueueRandomPriority |  3.650 μs | 0.3540 μs | 0.1852 μs | 3.400 μs |  4.000 μs |    1 |                    - |                - |     896 B |
|               TaskQueueWith1000Items_Dequeue |  5.500 μs | 1.0912 μs | 0.5707 μs | 4.600 μs |  6.400 μs |    2 |                    - |                - |     704 B |
|        TaskQueue_EnqueueNewItem_LimitReached |  8.111 μs | 2.2848 μs | 1.3596 μs | 7.000 μs | 10.500 μs |    3 |                    - |                - |    1144 B |

#### Conclusion

In all the scenarios the time added to the execution pipeline is very small and the impact caused by the limiter and task queue can be ignored.
