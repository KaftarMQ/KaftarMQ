namespace Broker;

public interface IBroker
{
    void PushMessage(string key, string value, Guid id);
    Message? PullMessage(string key);
    void AddSubscriber(string key, string clientAddress);
    void UpdatePointer(string key, Guid lastConsumedMessageId);
}