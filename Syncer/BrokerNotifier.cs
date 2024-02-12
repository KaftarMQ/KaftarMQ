using Pathoschild.Http.Client;

namespace Syncer;

public class BrokerNotifier
{
    public async Task MoveSlaveContentToMaster(string broker)
    {
        await new FluentClient(broker)
            .GetAsync("message/MoveSlaveContentToMaster");
    }
}