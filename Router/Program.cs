using Router.Business;
using RoutingAlgorithm;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RoutingTableStorage>();
builder.Services.AddSingleton<MessagePublisher>();
builder.Services.AddSingleton<SubscribeHandler>();
builder.Services.AddSingleton<PullHandler>();
builder.Services.AddSingleton<ClientNotifier>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();