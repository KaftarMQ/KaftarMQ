using Pathoschild.Http.Client;

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

await new FluentClient("http://localhost:5154")
    .PostAsync("message/subscribe")
    .WithArgument("key", keyToSubscribe)
    .WithArgument("clientAddress", "http://localhost:5000");

app.MapControllers();
app.UseHttpsRedirection();

app.Run();