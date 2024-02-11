using Broker;
using Pathoschild.Http.Client;

namespace Router.Business;

public class ClientNotifier
{
    public async Task NotifyClient(string clientAddress, Message message)
    {
        await new FluentClient(clientAddress)
            .PostAsync("message/push")
            .WithArgument("key", message.Id)
            .WithArgument("value", message.Value);
        
        Console.WriteLine("ClientLibrary Notified");
    }
}