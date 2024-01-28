namespace Broker;

public class MemoryMessageStore : IMessageStore
{
    private readonly Dictionary<Guid, LinkedList<MessageRecord>> _messages = new();

    public void AddMessage(Message message)
    {
        var key = message.Id;
        var messageRecord = new MessageRecord(Guid.NewGuid(), DateTime.Now, message);

        if (!_messages.ContainsKey(key))
        {
            _messages[key] = new LinkedList<MessageRecord>();
        }

        _messages[key].AddLast(messageRecord);
    }
}