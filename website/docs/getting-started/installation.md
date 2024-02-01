---
sidebar_position: 1
---

# Installation

To start using the LoadShedding library, just install the following package to the Startup Project:

```bash
dotnet add package Farfetch.LoadShedding.AspNetCore
```

## How to Use

Add the LoadShedding services by calling the `AddLoadShedding` extension:

```csharp
services.AddLoadShedding();
```

Use the `UseLoadShedding` extension method by extending the `IApplicationBuilder` interface:

```csharp
app.UseLoadShedding();
```
