namespace Broker;

public class ClientNotifier : IClientNotifier
{
    public void NotifyClient(string clientAddress, Message message)
    {
        Console.WriteLine("Client Notified");
    }
}