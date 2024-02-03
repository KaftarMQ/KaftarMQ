namespace Syncer;

public class RoutingCoordinator
{
    private readonly RoutingTableCalc _routingTableCalc = new();
    private readonly RoutingTable _routingTable;

    public RoutingCoordinator()
    {
        _routingTable = _routingTableCalc.CalcRoutingTable();
    }

    public void BrokerDied(string broker)
    {
        
    }

    public void KeyAdded(string key)
    {
        
    }
}