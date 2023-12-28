namespace Broker;

public interface IClientNotifier
{
    void NotifyClient(string clientAddress, Message message);
}