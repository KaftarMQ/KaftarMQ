using Pathoschild.Http.Client;
using RoutingAlgorithm;
using Syncer.RoutingAlgorithm;

namespace Syncer;

public class BrokersScaleUpChecker
{
    private readonly RouterNotifier _routerNotifier;
    private readonly RoutingTableStorage _routingTableStorage;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly Func<CancellationToken, Task> _healthCheckMethod;
    private Task _healthCheckTask;
    private List<string> _brokers = new();

    public BrokersScaleUpChecker(RouterNotifier routerNotifier,
        RoutingTableStorage routingTableStorage,
        CancellationTokenSource cancellationTokenSource,
        Func<CancellationToken, Task> healthCheckMethod,
        Task healthCheckTask)
    {
        _routerNotifier = routerNotifier ?? throw new ArgumentNullException(nameof(routerNotifier));
        _routingTableStorage = routingTableStorage ?? throw new ArgumentNullException(nameof(routingTableStorage));
        _cancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));
        _healthCheckMethod = healthCheckMethod ?? throw new ArgumentNullException(nameof(healthCheckMethod));
        _healthCheckTask = healthCheckTask ?? throw new ArgumentNullException(nameof(healthCheckTask));
    }

    public async Task Check()
    {
        Console.WriteLine("check started");
        var newBrokers = GetNewBrokers();
        var newBrokersCount = newBrokers.Count;
        if (_brokers.Count >= newBrokersCount)
        {
            return;
        }
        
        Console.WriteLine("scale up started");

        await CancelHealthCheck();
        
        await Task.Delay(20000);
        newBrokers = GetNewBrokers();

        var messagesStorage = new MessagesStorage();
        await PullAllMessages(messagesStorage);
        await UpdateRouters(newBrokers);
        await PushAllMessages(messagesStorage);
        await Task.Delay(10000);
        InitBusinessForNextCall(newBrokers);
    }

    private async Task PushAllMessages(MessagesStorage messagesStorage)
    {
        while (true)
        {
            var message = messagesStorage.Dequeue();
            if (message is null) break;
            await Push(message);
        }
    }

    private const string RandomRouter = "http://router";

    private async Task Push(Message message)
    {
        await new FluentClient(RandomRouter)
            .PostAsync("Message/push")
            .WithArgument("key", message.Key)
            .WithArgument("value", message.Value);
    }

    private async Task UpdateRouters(List<string> newBrokers)
    {
        _routingTableStorage.UpdateBrokers(
            newBrokers.Select(u => new BrokerData(u, false)).ToList());
        await _routerNotifier.NotifyRoutersTheBrokers();
    }

    private void InitBusinessForNextCall(List<string> newBrokers)
    {
        _brokers = newBrokers;
        _cancellationTokenSource = new CancellationTokenSource();
        _healthCheckTask = Task.Run(async () => await _healthCheckMethod(_cancellationTokenSource.Token));
    }

    private async Task CancelHealthCheck()
    {
        try
        {
            _cancellationTokenSource.Cancel();
            await _healthCheckTask;
        }
        catch
        {
        }
    }

    private async Task PullAllMessages(MessagesStorage messagesStorage)
    {
        while (true)
        {
            var message = await InternalPull();
            if (message is null) break;
            messagesStorage.Enqueue(message);
        }
    }
    
    private static async Task<Message?> InternalPull()
    {
        var message = await new FluentClient(RandomRouter)
            .GetAsync("Message/pull")
            .As<Message?>();
        
        return message;
    }


    private static List<string> GetNewBrokers()
    {
        //todo abol replace
        var newBrokers = ENVIRONMENT.ALL_BROKERS;
        return newBrokers;
    }
}

public class MessagesStorage
{
    private readonly Queue<Message> _messages = new();
    public void Enqueue(Message message)
    {
        _messages.Enqueue(message);
    }
    
    public Message? Dequeue()
    {
        try
        {
            return _messages.Dequeue();
        }
        catch
        {
            return null!;
        }
    }
}