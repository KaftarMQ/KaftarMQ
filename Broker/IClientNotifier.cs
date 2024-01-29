namespace Broker;

public interface IClientNotifier
{
    Task NotifyClient(string clientAddress, Message message);
}