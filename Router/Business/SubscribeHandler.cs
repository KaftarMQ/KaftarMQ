namespace Router.Business;

public class SubscribeHandler
{
    private readonly PullHandler _pullHandler;
    private readonly ClientNotifier _clientNotifier;
    
    private readonly HashSet<string> _clients;
    private readonly Task _handleSubscribe;

    public SubscribeHandler(PullHandler pullHandler, ClientNotifier clientNotifier)
    {
        _pullHandler = pullHandler ?? throw new ArgumentNullException(nameof(pullHandler));
        _clientNotifier = clientNotifier ?? throw new ArgumentNullException(nameof(clientNotifier));
        _clients = new HashSet<string>();
        _handleSubscribe = CreateWorkingTask();
    }

    private Task CreateWorkingTask()
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                HandleSubscribe();
                Console.WriteLine("subscribed");
                await Task.Delay(1000);   
            }
        });
    }

    private void HandleSubscribe()
    {
        if (!_clients.Any()) return;

        var client = GetClient();
        var message = _pullHandler.Pull().GetAwaiter().GetResult();
        if (message == null) return;

        _clientNotifier.NotifyClient(client, message).GetAwaiter().GetResult();
    }

    private string GetClient()
    {
        lock (_clients)
        {
            return GetRandomElement(_clients.ToList());
        }
    }

    private static T GetRandomElement<T>(List<T> list)
    {
        var n = list.Count;
        var index =  new Random().Next(n);
        return list[index];
    }

    public void AddSubscriber(string clientAddress)
    {
        lock (_clients)
        {
            _clients.Add(clientAddress);
        }
    }
}