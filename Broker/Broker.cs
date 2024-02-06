namespace Broker;

public class Broker
{
    private LinkedList<Message> _masterMessages = new();
    private LinkedList<Message> _slaveMessages = new();

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

    public void MoveSlaveContentToMaster()
    {
        lock (_masterMessages)
        {
            lock (_slaveMessages)
            {
                _masterMessages = new LinkedList<Message>(_masterMessages.Concat(_slaveMessages));
                _slaveMessages = new LinkedList<Message>();
            }
        }
    }
}