namespace Broker;

public interface IBroker
{
    void PushMessage(Guid key, string value);
    Message? PullMessage(Guid key);
    void AddSubscriber(Guid key, string clientAddress);
}