using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Router.Business;
using RoutingAlgorithm;

var clientNotifier = new ClientNotifier();
var routingTableStorage = new RoutingTableStorage();
var pullHandler = new PullHandler(routingTableStorage);
var subscribeHandler = new SubscribeHandler(pullHandler, clientNotifier);

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
builder.Services.AddSingleton<MessagePublisher>();
builder.Services.AddSingleton(routingTableStorage);
builder.Services.AddSingleton(subscribeHandler);
builder.Services.AddSingleton(pullHandler);
builder.Services.AddSingleton(clientNotifier);
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IMetrics>(metrics);


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

app.Run();