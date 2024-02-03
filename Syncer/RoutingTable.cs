namespace Syncer;

public record RoutingTable(Dictionary<string, string> MasterNodes, Dictionary<string, List<string>> SlaveNodes);
