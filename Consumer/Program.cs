using Pathoschild.Http.Client;
using RoutingAlgorithm;

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