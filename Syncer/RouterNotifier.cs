using Pathoschild.Http.Client;
using RoutingAlgorithm;

namespace Syncer;

public class RouterNotifier
{
    public async Task NotifyRoutersTheBrokers()
    {
        var brokers = ENVIRONMENT.ALL_BROKERS;

        foreach (var router in ENVIRONMENT.ALL_ROUTERS)
        {
            await new FluentClient(router)
                .PostAsync("message/update_brokers")
                .WithArgument("brokers", brokers);
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