using Broker.Classes;

namespace Router.Business;

public class RoutingTableStorage
{
    private Dictionary<string, string> masterNodes = new Dictionary<string, string>();
    private Dictionary<string, List<string>> slaveNodes = new Dictionary<string, List<string>>();

    public void UpdateBrokers(List<BrokerData> brokers)
    {
        masterNodes = brokers.Where(x => x.IsMaster).ToDictionary(x => x.Key, x => x.Url);

        slaveNodes = brokers.Where(x => !x.IsMaster)
            .GroupBy(x => x.Key)
            .ToDictionary(x => x.Key,
                x => x.Select(y => y.Url).ToList());
    }

    public List<string> GetBrokers(string key)
    {
        return Enumerable.Repeat(masterNodes[key], 1)
            .Concat(slaveNodes.ContainsKey(key) ? slaveNodes[key] : Enumerable.Empty<string>())
            .ToList();
    }

    public List<string> GetSlaves(string key)
    {
        return slaveNodes.ContainsKey(key) ? slaveNodes[key] : new List<string>();
    }

    public string GetMaster(string key)
    {
        if (!masterNodes.ContainsKey(key))
        {
            throw new Exception($"fucked up key : {key}");
        }

        return masterNodes[key];
    }
}