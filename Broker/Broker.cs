namespace Broker;

public class Broker
{
    private readonly LinkedList<Message> _masterMessages = new();
    private readonly LinkedList<Message> _slaveMessages = new();

    public void PushMessage(string key, string value, Guid id, bool isReplication)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "null";
        }

        if (isReplication)
        {
            lock (_slaveMessages)
            {
                _slaveMessages.AddLast(new Message(key, value, id));
            }
        }
        else
        {
            lock (_masterMessages)
            {
                _masterMessages.AddLast(new Message(key, value, id));
            }
        }
    }

    public Message? PullMessage()
    {
        lock (_masterMessages)
        {
            var message = _masterMessages.FirstOrDefault();
            _masterMessages.RemoveFirst();
            return message;
        }
    }

    public void DropSlave(Guid messageId)
    {
        lock (_slaveMessages)
        {
            var message = _slaveMessages.FirstOrDefault(m => m.Id == messageId);
            if (message is not null)
            {
                _slaveMessages.Remove(message);
            }
        }
    }
}