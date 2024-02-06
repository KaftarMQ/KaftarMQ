using Pathoschild.Http.Client;
using RoutingAlgorithm;

namespace Syncer;

public class BrokerHealthChecker
{
    private readonly RoutingTableStorage _routingTableStorage;
    private readonly RouterNotifier _routerNotifier;
    private readonly BrokerNotifier _brokerNotifier;

    public BrokerHealthChecker(RoutingTableStorage routingTableStorage)
    {
        _routingTableStorage = routingTableStorage;
        _routerNotifier = new RouterNotifier(_routingTableStorage);
        _brokerNotifier = new BrokerNotifier();
    }

    public async Task CheckHealthOfBrokers()
    {
        foreach (var broker in _routingTableStorage.GetNotFailedBrokers())
        {
            if (await IsHealthy(broker)) continue;
            await _routerNotifier.NotifyRoutersFailedBroker(broker);
            await _brokerNotifier.MoveSlaveContentToMaster(_routingTableStorage.GetMasterSlave(broker).Url);
            _routingTableStorage.UpdateBrokerFailure(broker);
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