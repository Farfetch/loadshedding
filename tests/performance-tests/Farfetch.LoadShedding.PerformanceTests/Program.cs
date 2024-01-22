var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddLoadShedding((_, options) =>
{
    options.AdaptativeLimiter.ConcurrencyOptions.QueueTimeoutInMs = 1000;
    options.AdaptativeLimiter.ConcurrencyOptions.MinQueueSize = 1;
    options.AdaptativeLimiter.ConcurrencyOptions.InitialQueueSize = 2;
    options.AdaptativeLimiter.ConcurrencyOptions.MaxConcurrencyLimit = 10;
    options.AdaptativeLimiter.ConcurrencyOptions.MinConcurrencyLimit = 1;
    options.AdaptativeLimiter.ConcurrencyOptions.InitialConcurrencyLimit = 1;
});

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

if (app.Configuration.GetSection("UseLoadShedding").Get<bool>())
{
    app.UseLoadShedding();
}

app.UseStaticFiles();

app.MapControllers();

app.UseRouting();

app.Run();
