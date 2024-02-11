using Broker;
using Pathoschild.Http.Client;
using RoutingAlgorithm;

namespace Router.Business;

public class PullHandler
{
    private readonly RoutingTableStorage _routingTableStorage;

    public PullHandler(RoutingTableStorage routingTableStorage)
    {
        _routingTableStorage = routingTableStorage ?? throw new ArgumentNullException(nameof(routingTableStorage));
    }

    private static int I_R_N(int i, int n)
    {
        return (i) % n;
    }
    
    public async Task<Message?> Pull()
    {
        var n = _routingTableStorage.Brokers.Count;
        var i = new Random().Next(n);

        Console.WriteLine($"pull started");

        for (var j = I_R_N(i+1, n); j < i+1; j = I_R_N(j+1, n))
        {
            Console.WriteLine($"pull j:{j}, i+1:{i+1}");

            var masterBroker = _routingTableStorage.Brokers[j];
            if(masterBroker.IsFailed) continue;
            
            var slaveBroker = _routingTableStorage.GetNextHealthyBroker(I_R_N(j+1, n));

            try
            {
                var message = await new FluentClient(masterBroker.Url)
                    .GetAsync("Message/Pull")
                    .As<Message>();

                try
                {

                    await new FluentClient(slaveBroker.Url)
                        .GetAsync("Message/PullSlave")
                        .WithArgument("key", message.Key)
                        .WithArgument("id", message.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed pulling from slave {slaveBroker.Url}: \n{ex}");
                }
                
                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed pulling from {masterBroker.Url}: \n{ex}");
            }
        }
        
        Console.WriteLine($"pull finished");

        return null;
    }

}