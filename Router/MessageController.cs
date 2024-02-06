using Broker.Classes;
using Microsoft.AspNetCore.Mvc;
using Router.Business;

namespace Broker;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly RoutingTableStorage _routingTableStorage;
    private readonly MessagePublisher _messagePublisher;

    public MessageController(RoutingTableStorage routingTableStorage, MessagePublisher messagePublisher)
    {
        _routingTableStorage = routingTableStorage ?? throw new ArgumentNullException(nameof(routingTableStorage));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
    }

    [HttpPost("push")]
    public IActionResult Push(string key, string value)
    {
        Console.WriteLine($"Pushing message with key: {key}, value: {value}");
        _messagePublisher.Publish(key, value).GetAwaiter().GetResult();
        return Ok();
    }

    [HttpGet]
    public ActionResult<Message?> Pull()
    {
        Console.WriteLine($"Pulling message");
        return _messagePublisher.Pull().GetAwaiter().GetResult();
    }

    [HttpPost("subscribe")]
    public IActionResult Subscribe(string clientAddress)
    {
        Console.WriteLine($"Subscribing message with clientAddress : {clientAddress}");
        _messagePublisher.Subscribe(clientAddress).GetAwaiter().GetResult();
        return Ok();
    }
    
    //syncer call this
    [HttpPost("update_brokers")]
    public IActionResult UpdateBrokers(List<BrokerData> brokers)
    {
        _routingTableStorage.UpdateBrokers(brokers);
        return Ok();
    }
}