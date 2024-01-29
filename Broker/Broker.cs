namespace Broker;

public class Broker : IBroker
{
    private readonly IMessageStore _messageStore;
    private readonly IClientNotifier _clientNotifier;
    private readonly ReplicationMetadata _replicationMetadata;
    private readonly Dictionary<string, HashSet<Subscriber>> _subscribers = new();
    private readonly Dictionary<string, Queue<Message>> _notSeenMessages = new();

    public Broker(IMessageStore messageStore, IClientNotifier clientNotifier, ReplicationMetadata replicationMetadata)
    {
        _messageStore = messageStore;
        _clientNotifier = clientNotifier;
        _replicationMetadata = replicationMetadata;
    }

    public void PushMessage(string key, string value, Guid id)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        var message = new Message(key, value, id);
        _messageStore.AddMessage(message);

        if (!_notSeenMessages.ContainsKey(key))
        {
            _notSeenMessages[key] = new Queue<Message>();
        }

        var notSeenMessage = _notSeenMessages[key];
        lock (notSeenMessage)
        {
            notSeenMessage.Enqueue(message);
        }
        if (!_subscribers.ContainsKey(key))
        {
            _subscribers[key] = new HashSet<Subscriber>();
        }

        NotifySubscribers(key);
    }

    private void NotifySubscribers(string key)
    {
        if (!_replicationMetadata.IsMaster(key))
        {
            return;
        }
        var notSeenMessages = _notSeenMessages[key];
        lock (notSeenMessages)
        {
            if (notSeenMessages.Count == 0 || (_subscribers.ContainsKey(key) && _subscribers[key].Count == 0))
            {
                return;
            }

            while (notSeenMessages.Count > 0)
            {
                var message = notSeenMessages.Dequeue();

                var keySubscribers = _subscribers[key].ToArray();
                
                var randomSubscriber = keySubscribers[new Random().Next(keySubscribers.Length)];
                
                _clientNotifier.NotifyClient(randomSubscriber.Address, message)
                    .GetAwaiter()
                    .GetResult();

                new FluentClient(ADDRESS.RouterAddress)
                    .PostAsync("replication/updatePointer")
                    .WithArgument("key", message.Key)
                    .WithArgument("lastConsumedMessageId", message.Id).GetAwaiter().GetResult();
            }
        }
    }

    public Message? PullMessage(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        if (!_notSeenMessages.ContainsKey(key))
            return null;

        var notSeenMessage = _notSeenMessages[key];
        lock (notSeenMessage)
        {
            return notSeenMessage.Dequeue();
        }
    }

    public void AddSubscriber(string key, string clientAddress)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        if (!_subscribers.ContainsKey(key))
        {
            _subscribers[key] = new HashSet<Subscriber>();
        }

        _subscribers[key].Add(new Subscriber(clientAddress));

        NotifySubscribers(key);
    }

    public void UpdatePointer(string key, Guid lastConsumedMessageId)
    {
        var notSeenMessage = _notSeenMessages[key];
        lock (notSeenMessage)
        {
            while (notSeenMessage.Dequeue().Id != lastConsumedMessageId)
            {
            }   
        }
    }
}

public record Subscriber(string Address);