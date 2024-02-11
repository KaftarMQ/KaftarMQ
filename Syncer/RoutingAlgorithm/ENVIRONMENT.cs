using System.Net;

namespace Syncer.RoutingAlgorithm;

public static class ENVIRONMENT
{
    public static List<string> ALL_BROKERS => GetAllReplications("http://broker");
    public static List<string> ALL_ROUTERS => GetAllReplications("http://router");

    private static List<string> GetAllReplications(string alias)
    {
        var res = new HashSet<string>();
        try
        {
            var address = Resolve(alias);
            while (!res.Contains(address))
            {
                res.Add(address);
                address = Resolve(alias);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to retrieve service urls for \"{alias}\": {e}");
        }

        Console.WriteLine($"Resolved for {alias}: {string.Join(", ", res)}");
        return res.ToList();
    }

    private static string Resolve(string alias)
    {
        var uri = new Uri(alias);
        return Dns.GetHostAddresses(uri.Host)[0].ToString();
    }
}