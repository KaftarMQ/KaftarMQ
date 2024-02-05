using Broker.Classes;

namespace Router.Business;

public class RoutingTableStorage
{
    private List<BrokerData> _brokers = new List<BrokerData>();

    public List<BrokerData> Brokers => _brokers;

    public void UpdateBrokers(List<BrokerData> brokers)
    {
        brokers.Sort();
        _brokers = brokers;
    }

    public BrokerData GetMaster(string key)
    {
        var mod = _brokers.Count;
        return _brokers[key.GetHashCode() % mod];
    }
    
    public BrokerData GetSlave(string key)
    {
        var mod = _brokers.Count;
        return _brokers[(key.GetHashCode()+1) % mod];
    }
}