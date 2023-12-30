namespace Broker;

public interface IBroker
{
    void PushMessage(string key, string value);
    Message? PullMessage(string key);
    void AddSubscriber(string key, string clientAddress);
}