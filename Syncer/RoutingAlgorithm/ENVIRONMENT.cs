using Pathoschild.Http.Client;

namespace Syncer.RoutingAlgorithm;

public static class ENVIRONMENT
{
    private static string Router = "http://router";
    private static string Broker = "http://broker";

    public static List<string> ALL_BROKERS => GetAllReplications(Broker);
    public static List<string> ALL_ROUTERS => GetAllReplications(Router);

    private static List<string> GetAllReplications(string alias)
    {
        var res = new HashSet<string>();
        try
        {
            var confidenceRange = 5;
            while (confidenceRange > 0)
            {
                var address = Resolve(alias);
                if (res.Contains(address))
                {
                    confidenceRange--;
                }
                else
                {
                    res.Add(address);
                }
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
        var response = new FluentClient(alias)
            .GetAsync("Message/ip")
            .As<string>()
            .GetAwaiter().GetResult();

        return $"http://{response}";
    }
}