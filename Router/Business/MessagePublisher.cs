﻿using Broker;
using Pathoschild.Http.Client;
using RoutingAlgorithm;

namespace Router.Business;

public class MessagePublisher
{
    private readonly RoutingTableStorage _routingTableStorage;
    private readonly PullHandler _pullHandler;

    public MessagePublisher(RoutingTableStorage routingTableStorage, 
        PullHandler pullHandler)
    {
        _routingTableStorage = routingTableStorage ?? throw new ArgumentNullException(nameof(routingTableStorage));
        _pullHandler = pullHandler ?? throw new ArgumentNullException(nameof(pullHandler));
    }

    public async Task Publish(string key, string value)
    {
        var masterUrl = _routingTableStorage.GetMaster(key).Url;
        var slaveUrl = _routingTableStorage.GetSlave(key).Url;

        var guid = Guid.NewGuid();
        await PublishMessageToBroker(slaveUrl, key, value, guid, true);
        await PublishMessageToBroker(masterUrl, key, value, guid, false);
    }
    
    private static async Task PublishMessageToBroker(string clientAddress, string key, string value, Guid id, bool isReplication)
    {
        await new FluentClient(clientAddress)
            .PostAsync("message/push")
            .WithArgument("key", key)
            .WithArgument("value", value)
            .WithArgument("id", id)
            .WithArgument("isReplication", isReplication);

        Console.WriteLine("broker Notified");
    }

    public async Task<Message?> Pull()
    {
        return await _pullHandler.Pull();
    }
}