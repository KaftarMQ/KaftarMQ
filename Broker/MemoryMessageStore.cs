namespace Broker;

public class MemoryMessageStore : IMessageStore
{
    private readonly Dictionary<string, LinkedList<MessageRecord>> _messages = new();

    public void AddMessage(Message message)
    {
        var key = message.Key;
        var messageRecord = new MessageRecord(Guid.NewGuid(), DateTime.Now, message);
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }
        if (!_messages.ContainsKey(key))
        {
            _messages[key] = new LinkedList<MessageRecord>();
        }

        _messages[key].AddLast(messageRecord);
    }
}