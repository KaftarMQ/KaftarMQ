namespace Broker;

public class Broker : IBroker
{
    private readonly IMessageStore _messageStore;
    private readonly IClientNotifier _clientNotifier;
    private readonly Dictionary<string, HashSet<string>> _subscribers = new();
    private readonly Dictionary<string, Queue<Message>> _notSeenMessages = new();

    public Broker(IMessageStore messageStore, IClientNotifier clientNotifier)
    {
        _messageStore = messageStore;
        _clientNotifier = clientNotifier;
    }

    public void PushMessage(string key, byte[] value)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        var message = new Message(key, value);
        _messageStore.AddMessage(message);

        if (!_notSeenMessages.ContainsKey(key))
        {
            _notSeenMessages[key] = new Queue<Message>();
        }

        _notSeenMessages[key].Enqueue(message);

        NotifySubscribers(key);
    }

    private void NotifySubscribers(string key)
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
                _clientNotifier.NotifyClient(subscriber, message);
            }
        }
    }

    public Message? PullMessage(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        return !_notSeenMessages.ContainsKey(key) ? null : _notSeenMessages[key].Dequeue();
    }

    public void AddSubscriber(string key, string clientAddress)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        _subscribers[key].Add(clientAddress);
    }
}