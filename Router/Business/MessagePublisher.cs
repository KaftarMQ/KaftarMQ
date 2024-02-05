using Broker;
using Pathoschild.Http.Client;

namespace Router.Business;

public class MessagePublisher
{
    private readonly RoutingTableStorage _routingTableStorage;

    public MessagePublisher(RoutingTableStorage routingTableStorage)
    {
        _routingTableStorage = routingTableStorage ?? throw new ArgumentNullException(nameof(routingTableStorage));
    }

    public async Task Publish(string key, string value)
    {
        var masterUrl = _routingTableStorage.GetMaster(key).Url;
        var slaveUrl = _routingTableStorage.GetMaster(key).Url;

        var guid = Guid.NewGuid();
        await NotifyClient(masterUrl, key, value, guid);
        await NotifyClient(slaveUrl, key, value, guid);
    }
    
    private static async Task NotifyClient(string clientAddress, string key, string value, Guid id)
    {
        await new FluentClient(clientAddress)
            .PostAsync("message/push")
            .WithArgument("key", key)
            .WithArgument("value", value)
            .WithArgument("id", id);

        Console.WriteLine("broker Notified");
    }

    public async Task<Message?> Pull()
    {
        var n = _routingTableStorage.Brokers.Count;
        var i = new Random().Next(n);

        for (var j = (i+1)%n; j == i; j = (j + 1) % n)
        {
            var broker = _routingTableStorage.Brokers[j];
            
            try
            {
                var message = await new FluentClient(broker.Url)
                    .PostAsync("message/pull")
                    .As<Message>();
                
                
                 
                return message;
            }
            catch
            {
                // ignored
            }
        }

        return null;
    }

    public async Task Subscribe(string clientAddress)
    {
        var brokers = _routingTableStorage.(key);

        foreach (var broker in brokers)
        {
            await new FluentClient(broker)
                .PostAsync("message/subscribe")
                .WithArgument("key", key)
                .WithArgument("clientAddress", clientAddress);    
        }

        Console.WriteLine("broker Notified");
    }

    public async Task UpdatePointer(string key, Guid lastConsumedMessageId)
    {
        var brokers = _routingTableStorage.GetSlaves(key);

        foreach (var broker in brokers)
        {
            await new FluentClient(broker)
                .PostAsync("replication/updatePointer")
                .WithArgument("key", key)
                .WithArgument("lastConsumedMessageId", lastConsumedMessageId);    
        }

        Console.WriteLine("pointer updated");
    }
}