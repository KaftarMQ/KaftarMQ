using Pathoschild.Http.Client;
using RoutingAlgorithm;
using Syncer.RoutingAlgorithm;

namespace Syncer;

public class RouterNotifier
{
    private readonly RoutingTableStorage _routingTableStorage;

    public RouterNotifier(RoutingTableStorage routingTableStorage)
    {
        _routingTableStorage = routingTableStorage;
    }

    public async Task NotifyRoutersTheBrokers()
    {
        var brokers = _routingTableStorage.GetNotFailedBrokers();

        foreach (var router in ENVIRONMENT.ALL_ROUTERS)
        {
            await new FluentClient(router)
                .PostAsync("message/updateBrokers")
                .WithBody(brokers.ToArray());
        }
    }

    public async Task NotifyRoutersFailedBroker(string broker)
    {
        foreach (var router in ENVIRONMENT.ALL_ROUTERS)
        {
            await new FluentClient(router)
                .PostAsync("message/UpdateBrokerFailure")
                .WithArgument("brokerUrl", broker);
        }
    }
}