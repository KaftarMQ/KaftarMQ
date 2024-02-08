namespace RoutingAlgorithm;

public static class ENVIRONMENT
{
    public static List<string> ALL_BROKERS = new() { "http://localhost:5154"};
    public static List<string> ALL_ROUTERS = new() { "http://localhost:5274"};
    public static string NGINX = ALL_ROUTERS[0];//"http://localhost:5274";
}