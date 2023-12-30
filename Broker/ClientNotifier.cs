using Pathoschild.Http.Client;

namespace Broker;

public class ClientNotifier : IClientNotifier
{
    public async void NotifyClient(string clientAddress, Message message)
    {
        await new FluentClient(clientAddress)
            .PostAsync("message/push")
            .WithArgument("key", message.Key)
            .WithArgument("value", message.Value);
        
        Console.WriteLine("Client Notified");
    }
}