using Pathoschild.Http.Client;

namespace Broker.Business;

public class MessagePublisher
{
    private readonly RoutingTableStorage _routingTableStorage;

    public MessagePublisher(RoutingTableStorage routingTableStorage)
    {
        _routingTableStorage = routingTableStorage ?? throw new ArgumentNullException(nameof(routingTableStorage));
    }

    public async Task Publish(string key, string value)
    {
        var brokerUrls = _routingTableStorage.GetBrokers(key);

        foreach (var brokerUrl in brokerUrls)
        {
            await NotifyClient(brokerUrl, key, value);
        }
    }
    
    public async Task<Message?> Pull(string key)
    {
        var masterUrl = _routingTableStorage.GetMaster(key);

        return await new FluentClient(masterUrl)
            .PostAsync("message/pull")
            .WithArgument("key", key).As<Message>();
    }

    
    private static async Task NotifyClient(string clientAddress, string key, string value)
    {
        await new FluentClient(clientAddress)
            .PostAsync("message/push")
            .WithArgument("key", key)
            .WithArgument("value", value);
        
        Console.WriteLine("broker Notified");
    }

    public async Task Subscribe(string key, string clientAddress)
    {
        var brokers = _routingTableStorage.GetBrokers(key);

        foreach (var broker in brokers)
        {
            await new FluentClient(broker)
                .PostAsync("message/subscribe")
                .WithArgument("key", key)
                .WithArgument("clientAddress", clientAddress);    
        }

        Console.WriteLine("broker Notified");
    }

    public async Task UpdatePointer(string key, string lastConsumedMessageId)
    {
        var brokers = _routingTableStorage.GetBrokers(key);

        foreach (var broker in brokers)
        {
            await new FluentClient(broker)
                .PostAsync("message/UpdatePointer")
                .WithArgument("key", key)
                .WithArgument("lastConsumedMessageId", lastConsumedMessageId);    
        }

        Console.WriteLine("pointer updated");
    }
}