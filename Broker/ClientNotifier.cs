using Pathoschild.Http.Client;

namespace Broker;

public class ClientNotifier : IClientNotifier
{
    public async Task NotifyClient(string clientAddress, Message message)
    {
        await new FluentClient(clientAddress)
            .PostAsync("message/push")
            .WithArgument("key", message.Id)
            .WithArgument("value", message.Value);
        
        Console.WriteLine("Client Notified");
    }
}