using Syncer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

await Task.Delay(TimeSpan.FromSeconds(20));
await new RouterNotifier().NotifyRoutersTheBrokers();

var brokerHealthChecker = new BrokerHealthChecker();
Task.Run(async () =>
{
    while (true)
    {
        await brokerHealthChecker.CheckHealthOfBrokers();
        await Task.Delay(TimeSpan.FromSeconds(10));
    }
});

app.Run();