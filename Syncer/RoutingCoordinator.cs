namespace Syncer;

public class RoutingCoordinator
{
    private readonly RoutingTableCalc _routingTableCalc = new();
    private readonly RoutingTable _routingTable = new(new Dictionary<string, string>(), 
        new Dictionary<string, HashSet<string>>(),
        new HashSet<string>(),
        new HashSet<string>());
    private readonly RouterNotifier _routerNotifier = new();

    public void BrokerDied(string broker)
    {
        _routingTableCalc.BrokerDied(_routingTable, broker);
        _routerNotifier.NotifyRouters(_routingTable);
    }

    public void BrokerAdded(string broker)
    {
        _routingTableCalc.BrokerAdded(_routingTable, broker);
        _routerNotifier.NotifyRouters(_routingTable);
    }

    public void KeyAdded(string key)
    {
        _routingTableCalc.KeyAdded(_routingTable, key);
        _routerNotifier.NotifyRouters(_routingTable);
    }
}