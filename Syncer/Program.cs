using RoutingAlgorithm;
using Syncer;

var routingTableStorage = new RoutingTableStorage();
var allBrokers = ENVIRONMENT.ALL_BROKERS.Select(u => new BrokerData(u, false)).ToList();
routingTableStorage.UpdateBrokers(allBrokers);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(routingTableStorage);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

//await Task.Delay(TimeSpan.FromSeconds(20)); // todo if needed  
await new RouterNotifier(routingTableStorage).NotifyRoutersTheBrokers();

var brokerHealthChecker = new BrokerHealthChecker(routingTableStorage);
Task.Run(async () =>
{
    while (true)
    {
        await brokerHealthChecker.CheckHealthOfBrokers();
        await Task.Delay(TimeSpan.FromSeconds(10));
    }
});

app.Run();