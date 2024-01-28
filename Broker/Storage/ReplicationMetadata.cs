namespace Broker.Storage;

public class ReplicationMetadata
{
    private readonly Dictionary<string, bool> _isMasterByKey = new ();
    
    public bool IsMaster(string key)
    {
        return _isMasterByKey.ContainsKey(key) && _isMasterByKey[key];
    }

    public void SetMaster(string key)
    {
        _isMasterByKey[key] = true;
    }
    
    public void SetSlave(string key)
    {
        _isMasterByKey[key] = false;
    }
}