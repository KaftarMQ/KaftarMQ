using Broker;
using Broker.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IBroker, Broker.Broker>();
builder.Services.AddSingleton<IMessageStore, MemoryMessageStore>();
builder.Services.AddSingleton<IClientNotifier, ClientNotifier>();
builder.Services.AddSingleton<ReplicationMetadata>();

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