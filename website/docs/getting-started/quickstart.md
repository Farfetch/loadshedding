---
sidebar_position: 2
sidebar_label: Quickstart
---

# Quickstart: Create your first application with LoadShedding

In this guide, you will use C# and the .NET CLI to create a WebApi that will have LoadShedding with the default configurations.

By the end of the guide, you will know how to use LoadShedding on your application.

## Prerequisites

- One of the following .NET versions:
  - .NET Core 2.0 or above.

## Overview

You will create a WebApi using LoadShedding.

## Steps
### 1. Create a folder for your applications

Create a new folder with the name _LoadSheddingQuickstart_.

### 2. Create WebApi Project

Run the following command to create a WebApi Project named _LoadSheddingQuickstart_:

```bash
dotnet new webapi -controllers -n LoadSheddingQuickstart
```

### 3. Install LoadShedding package

Inside the _LoadSheddingQuickstart_ project directory, run the following command to install the required package:

```bash
dotnet add package Farfetch.LoadShedding.AspNetCore
```

### 4 Add Metrics

Inside the _LoadSheddingQuickstart_ project directory, run the following command to install Prometheus:

```bash
dotnet add package prometheus-net.AspNetCore
dotnet add package Farfetch.LoadShedding.Prometheus
```

:::info
This step is optional. With this you will be able to confirm that LoadShedding is configured on the [Run!](#7-run) step.
:::

### 5. Add LoadShedding on the WebApi

Add the LoadShedding services by calling the AddLoadShedding extension:

```csharp
services.AddLoadShedding();
```

Optionally you can configure `SubscribeEvents()` and you will be able to confirm that LoadShedding is configured on the [Run!](#7-run) step:

```csharp
services.AddLoadShedding((provider, options) =>
{
    options.SubscribeEvents(events =>
    {
        events.ItemEnqueued.Subscribe(args => Console.WriteLine($"QueueLimit: {args.QueueLimit}, QueueCount: {args.QueueCount}"));
        events.ItemDequeued.Subscribe(args => Console.WriteLine($"QueueLimit: {args.QueueLimit}, QueueCount: {args.QueueCount}"));
        events.ItemProcessing.Subscribe(args => Console.WriteLine($"ConcurrencyLimit: {args.ConcurrencyLimit}, ConcurrencyItems: {args.ConcurrencyCount}"));
        events.ItemProcessed.Subscribe(args => Console.WriteLine($"ConcurrencyLimit: {args.ConcurrencyLimit}, ConcurrencyItems: {args.ConcurrencyCount}"));
        events.Rejected.Subscribe(args => Console.Error.WriteLine($"Item rejected with Priority: {args.Priority}"));
    });
});
```

:::note
For more information about de default configurations and possible customizations see [here.](../guides/adaptative-concurreny-limiter/configuration.md#options-configuration)
:::

### 6. Use LoadShedding on the WebApi

Use the `UseLoadShedding` extension method by extending the `IApplicationBuilder` interface:

```csharp
app.UseLoadShedding();
```

### 7. Run!

From the `LoadSheddingQuickstart` directory run the project:

```bash
dotnet run --project LoadSheddingQuickstart/LoadSheddingQuickstart.csproj 
```

And you should be able to call the available endpoint with Swagger Web Interface: https://localhost:7231/swagger/index.html

Now, you can confirm that LoadShedding is correctly configured on your WebApi by calling the following endpoint: https://localhost:7231/metrics

You should see the LoadShedding metrics referenced [here](../guides/adaptative-concurreny-limiter/configuration.md#reference-documentation).

Additionally, you can also confirm if LoadShedding is correctly configured by checking the following console output:

```
ConcurrencyLimit: 5, ConcurrencyItems: 1
ConcurrencyLimit: 5, ConcurrencyItems: 0
```
