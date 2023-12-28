namespace Broker;

public interface IBroker
{
    void PushMessage(string key, byte[] value);
    Message? PullMessage(string key);
    void AddSubscriber(string key, string clientAddress);
}