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

        for (var j = I_R_N(i+1, n); j == i+1; j = I_R_N(j+1, n))
        {
            var masterBroker = _routingTableStorage.Brokers[j];
            if(masterBroker.IsFailed) continue;
            
            var slaveBroker = _routingTableStorage.GetNextHealthyBroker(I_R_N(j+1, n));

            try
            {
                var message = await new FluentClient(masterBroker.Url)
                    .PostAsync("message/pull")
                    .As<Message>();

                try
                {

                    await new FluentClient(slaveBroker.Url)
                        .PostAsync("message/pullSlave")
                        .WithArgument("key", message.Key)
                        .WithArgument("id", message.Id);
                }
                catch
                {
                    
                }
                
                return message;
            }
            catch
            {
                // ignored
            }
        }

        return null;
    }

}