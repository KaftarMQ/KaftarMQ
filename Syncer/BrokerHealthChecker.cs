using Pathoschild.Http.Client;
using RoutingAlgorithm;

namespace Syncer;

public class BrokerHealthChecker
{
    public async Task CheckHealthOfBrokers()
    {
        var routerNotifier = new RouterNotifier();
        foreach (var broker in ENVIRONMENT.ALL_BROKERS)
        {
            if (!await IsHealthy(broker))
            {
                await routerNotifier.NotifyRoutersFailedBroker(broker);
            }
        }
    }

    private static async Task<bool> IsHealthy(string broker)
    {
        try
        {
            await new FluentClient(broker)
                .PostAsync("health");

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}