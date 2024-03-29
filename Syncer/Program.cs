using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using RoutingAlgorithm;
using Syncer;
using Syncer.RoutingAlgorithm;

await Task.Delay(TimeSpan.FromSeconds(60)); // todo if needed


var routingTableStorage = new RoutingTableStorage();
var allBrokers = ENVIRONMENT.ALL_BROKERS.Select(u => new BrokerData(u, false)).ToList();
routingTableStorage.UpdateBrokers(allBrokers);

var builder = WebApplication.CreateBuilder(args);

// Configure App.Metrics
var metrics = new MetricsBuilder()
    .OutputMetrics.AsPrometheusPlainText()
    .Build();

builder.Services.AddMetrics(metrics);
builder.Services.AddMetricsEndpoints(options =>
{
    options.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
});
builder.Services.AddMetricsTrackingMiddleware();

// Existing services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(routingTableStorage);
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMetricsAllMiddleware();
app.UseMetricsAllEndpoints();

app.MapHealthChecks("/health");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();


var routerNotifier = new RouterNotifier(routingTableStorage);
await routerNotifier.NotifyRoutersTheBrokers();

var brokerHealthChecker = new BrokerHealthChecker(routingTableStorage);
var ctSource = new CancellationTokenSource();

var healthCheckMethod = async (CancellationToken ct) =>
{
    while (true)
    {
        await Task.Delay(TimeSpan.FromSeconds(10));
        ct.ThrowIfCancellationRequested();
        await brokerHealthChecker.CheckHealthOfBrokers();
    }
};

var healthCheckTask = Task.Run(async () => await healthCheckMethod(ctSource.Token));

var brokersScaleChecker = new BrokersScaleUpChecker(routerNotifier, routingTableStorage, ctSource, healthCheckMethod, healthCheckTask);
Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(TimeSpan.FromSeconds(10));
        await brokersScaleChecker.Check();
    }
});


app.Run();