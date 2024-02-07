using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Pathoschild.Http.Client;
using RoutingAlgorithm;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMetricsAllMiddleware();
app.UseMetricsAllEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var keyToSubscribe = Console.ReadLine();
if (string.IsNullOrEmpty(keyToSubscribe))
{
    keyToSubscribe = "default";
}

const string CLIENT_ADDRESS = "http://localhost:5000";

await new FluentClient(ENVIRONMENT.NGINX)
    .PostAsync("message/subscribe")
    .WithArgument("key", keyToSubscribe)
    .WithArgument("clientAddress", CLIENT_ADDRESS);

app.MapControllers();
app.UseHttpsRedirection();

app.Run();