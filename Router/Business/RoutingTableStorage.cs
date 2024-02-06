using Broker.Classes;

namespace Router.Business;

public class RoutingTableStorage
{
    public List<BrokerData> Brokers { get; private set; } = new();

    public void UpdateBrokers(List<BrokerData> brokers)
    {
        brokers.Sort();
        Brokers = brokers;
    }

    public BrokerData GetMaster(string key)
    {
        return Brokers[GetMasterIndex(key)];
    }

    public BrokerData GetSlave(string key)
    {
        return GetNextHealthyBroker(GetMasterIndex(key));
    }

    public BrokerData GetNextHealthyBroker(int index)
    {
        var mod = Brokers.Count;
        index = (index+1) % mod;

        var brokerData = Brokers[index];
        while (brokerData.IsFailed)
        {
            index = (index+1) % mod;
            brokerData = Brokers[index];
        }

        return brokerData;
    }
    
    
    private int GetMasterIndex(string key)
    {
        var mod = Brokers.Count;
        var hash = key.GetHashCode() % mod;
        
        var brokerData = Brokers[hash];
        while (brokerData.IsFailed)
        {
            hash = (hash+1) % mod;
            brokerData = Brokers[hash];
        }

        return hash;
    }
}