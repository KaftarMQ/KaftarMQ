using Broker;
using Pathoschild.Http.Client;

namespace Router.Business;

public class MessagePublisher
{
    private readonly RoutingTableStorage _routingTableStorage;
    private readonly SubscribeHandler _subscribeHandler;
    private readonly PullHandler _pullHandler;

    public MessagePublisher(RoutingTableStorage routingTableStorage, 
        SubscribeHandler subscribeHandler,
        PullHandler pullHandler)
    {
        _routingTableStorage = routingTableStorage ?? throw new ArgumentNullException(nameof(routingTableStorage));
        _subscribeHandler = subscribeHandler ?? throw new ArgumentNullException(nameof(subscribeHandler));
        _pullHandler = pullHandler ?? throw new ArgumentNullException(nameof(pullHandler));
    }

    public async Task Publish(string key, string value)
    {
        var masterUrl = _routingTableStorage.GetMaster(key).Url;
        var slaveUrl = _routingTableStorage.GetMaster(key).Url;

        var guid = Guid.NewGuid();
        await PublishMessageToBroker(slaveUrl, key, value, guid);
        await PublishMessageToBroker(masterUrl, key, value, guid);
    }
    
    private static async Task PublishMessageToBroker(string clientAddress, string key, string value, Guid id)
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
        return await _pullHandler.Pull();
    }

    public async Task Subscribe(string clientAddress)
    {
        _subscribeHandler.AddSubscriber(clientAddress);
    }
}