namespace Syncer;

public class RoutingTableCalc
{
    public void BrokerDied(RoutingTable routingTable, string broker)
    {
        
    }

    public void BrokerAdded(RoutingTable routingTable, string broker)
    {
        routingTable.AllBrokers.Add(broker);
    }

    public void KeyAdded(RoutingTable routingTable, string newKey)
    {
        if (routingTable.AllKeys.Contains(newKey))
        {
            throw new Exception("Already Added Key");
        }

        var replications = Env.REPLICATION;

        var remainingBrokers = routingTable.AllBrokers.ToList();
        var master = GetRandomBroker(remainingBrokers);
        
        remainingBrokers.Remove(master);
        routingTable.MasterNodes.Add(newKey, master);

        for (var i = 0; i < replications - 1; i++)
        {
            var slave = GetRandomBroker(remainingBrokers);
        
            remainingBrokers.Remove(slave);
            if (!routingTable.SlaveNodes.ContainsKey(newKey))
            {
                routingTable.SlaveNodes.Add(newKey, new HashSet<string>());
            }
            routingTable.SlaveNodes[newKey].Add(slave);
        }

        routingTable.AllKeys.Add(newKey);
    }

    private static string GetRandomBroker(List<string> remainingBrokers)
    {
        var rnd = new Random();

        var brokerIndex = rnd.Next(remainingBrokers.Count);
        var broker = remainingBrokers[brokerIndex];

        return broker;
    }
}