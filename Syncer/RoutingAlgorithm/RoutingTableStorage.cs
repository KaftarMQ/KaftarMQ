using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Syncer;

namespace RoutingAlgorithm;

public class RoutingTableStorage
{
    public List<BrokerData> Brokers { get; private set; } = new();

    private bool _brokersInitialized;

    public void UpdateBrokers(List<BrokerData> brokers)
    {
        // if (_brokersInitialized)
        // {
        //     throw new Exception("Brokers are already initialized");
        // }

        brokers.Sort(BrokerData.UrlComparer);
        Brokers = brokers;
        _brokersInitialized = true;
    }

    public void UpdateBrokerFailure(string brokerUrl)
    {
        var broker = Brokers.FirstOrDefault(u => u.Url == brokerUrl);
        if (broker is null)
        {
            throw new Exception("Broker not found!");
        }

        broker.IsFailed = true;
    }

    public BrokerData GetMaster(string key)
    {
        return Brokers[GetMasterIndex(key)];
    }

    public BrokerData GetSlave(string key)
    {
        return GetNextHealthyBroker(GetMasterIndex(key));
    }

    public BrokerData GetMasterSlave(string masterUrl)
    {
        return GetNextHealthyBroker(GetBrokerIndex(masterUrl));
    }

    public BrokerData GetNextHealthyBroker(int index)
    {
        var mod = Brokers.Count;
        index = (index + 1) % mod;

        var brokerData = Brokers[index];
        while (brokerData.IsFailed)
        {
            index = (index + 1) % mod;
            brokerData = Brokers[index];
        }

        return brokerData;
    }

    private int GetMasterIndex(string key)
    {
        var mod = Brokers.Count;
        var hashByte = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
        var hash = BitConverter.ToInt32(hashByte, 0);
        hash = Math.Abs(hash);
        hash %= mod;
        Console.WriteLine($"GetMasterIndex :: key:{key}, hash:{hash}");

        var brokerData = Brokers[hash];
        while (brokerData.IsFailed)
        {
            hash = (hash + 1) % mod;
            brokerData = Brokers[hash];
        }

        return hash;
    }

    private int GetBrokerIndex(string url)
    {
        return Brokers.FindIndex(b => b.Url == url);
    }

    public IEnumerable<string> GetNotFailedBrokers()
    {
        return Brokers.Where(b => !b.IsFailed).Select(b => b.Url);
    }
}