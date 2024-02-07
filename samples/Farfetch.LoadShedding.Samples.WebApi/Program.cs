using Farfetch.LoadShedding;
using Farfetch.LoadShedding.Samples.WebApi;
using MongoDB.Driver;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddLoadShedding((provider, options) =>
{
    options.AdaptativeLimiter.ConcurrencyOptions.MinQueueSize = 10;
    options.AdaptativeLimiter.UseHeaderPriorityResolver();
    options.AddMetrics();

    options.SubscribeEvents(events =>
    {
        events.ItemEnqueued.Subscribe(args => Console.WriteLine($"QueueLimit: {args.QueueLimit}, QueueCount: {args.QueueCount}"));
        events.ItemDequeued.Subscribe(args => Console.WriteLine($"QueueLimit: {args.QueueLimit}, QueueCount: {args.QueueCount}"));
        events.ItemProcessing.Subscribe(args => Console.WriteLine($"ConcurrencyLimit: {args.ConcurrencyLimit}, ConcurrencyItems: {args.ConcurrencyCount}"));
        events.ItemProcessed.Subscribe(args => Console.WriteLine($"ConcurrencyLimit: {args.ConcurrencyLimit}, ConcurrencyItems: {args.ConcurrencyCount}"));
        events.Rejected.Subscribe(args => Console.Error.WriteLine($"Item rejected with Priority: {args.Priority}"));
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(_ => MongoUrl.Create(builder.Configuration["MongoConnectionString"]));

builder.Services.AddSingleton<IMongoClient>(provider => new MongoClient(provider.GetRequiredService<MongoUrl>()));
builder.Services.AddSingleton<IMongoDatabase>(provider =>
{
    var url = provider.GetRequiredService<MongoUrl>();
    var client = provider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(url.DatabaseName);
});

builder.Services.AddSingleton<IMongoCollection<WeatherForecast>>(provider => provider
    .GetRequiredService<IMongoDatabase>()
    .GetCollection<WeatherForecast>(nameof(WeatherForecast)));

var app = builder.Build();

app.UseMetricServer();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseLoadShedding();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var collection = app.Services.GetRequiredService<IMongoCollection<WeatherForecast>>();

    if (collection.Find(FilterDefinition<WeatherForecast>.Empty).Any())
    {
        return;
    }

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    };

    var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Id = Guid.NewGuid(),
        Date = DateTime.Now.AddDays(index),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = summaries[Random.Shared.Next(summaries.Length)],
    });

    collection.InsertMany(forecasts);
});

app.Run();
