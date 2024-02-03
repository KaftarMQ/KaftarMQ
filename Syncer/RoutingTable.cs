namespace Syncer;

public record RoutingTable(Dictionary<string, string> MasterNodes, 
    Dictionary<string, HashSet<string>> SlaveNodes,
    HashSet<string> AllBrokers,
    HashSet<string> AllKeys);
