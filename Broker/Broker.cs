namespace Broker;

public class Broker : IBroker
{
    private readonly IMessageStore _messageStore;
    private readonly IClientNotifier _clientNotifier;
    private readonly Dictionary<Guid, HashSet<Subscriber>> _subscribers = new();
    private readonly Dictionary<Guid, Queue<Message>> _notSeenMessages = new();

    public Broker(IMessageStore messageStore, IClientNotifier clientNotifier)
    {
        _messageStore = messageStore;
        _clientNotifier = clientNotifier;
    }

    public void PushMessage(Guid key, string value)
    {
        var message = new Message(key, value);
        _messageStore.AddMessage(message);

        if (!_notSeenMessages.ContainsKey(key))
        {
            _notSeenMessages[key] = new Queue<Message>();
        }

        _notSeenMessages[key].Enqueue(message);
        if (!_subscribers.ContainsKey(key))
        {
            _subscribers[key] = new HashSet<Subscriber>();
        }

        NotifySubscribers(key);
    }

    private void NotifySubscribers(Guid key)
    {
        var notSeenMessages = _notSeenMessages[key];
        if (notSeenMessages.Count == 0 || (_subscribers.ContainsKey(key) && _subscribers[key].Count == 0))
        {
            return;
        }

        while (notSeenMessages.Count > 0)
        {
            var message = _notSeenMessages[key].Dequeue();
            var keySubscribers = _subscribers[key];
            foreach (var subscriber in keySubscribers)
            {
                _clientNotifier.NotifyClient(subscriber.ClientAddress, message);
            }
        }
    }

    public Message? PullMessage(Guid key)
    {
        return !_notSeenMessages.ContainsKey(key) ? null : _notSeenMessages[key].Dequeue();
    }

    public void AddSubscriber(Guid key, string clientAddress)
    {
        if (!_subscribers.ContainsKey(key))
        {
            _subscribers[key] = new HashSet<Subscriber>();
        }

        _subscribers[key].Add(new Subscriber(Guid.NewGuid(), clientAddress, key));
    }
}