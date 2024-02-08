using System.Collections.Generic;

namespace RoutingAlgorithm;

public static class ENVIRONMENT
{
    public static List<string> ALL_BROKERS = new() { "http://broker"};
    public static List<string> ALL_ROUTERS = new() { "http://router"};
    public static string NGINX = "http://nginx";
}