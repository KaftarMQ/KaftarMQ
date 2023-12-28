namespace Broker;

public interface IMessageStore
{
    void AddMessage(Message message);
}