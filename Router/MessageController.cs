using System.Text;
using Broker.Business;
using Broker.Classes;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<Message?> Pull(string key)
    {
        Console.WriteLine($"Pulling message with key: {key}");
        return _messagePublisher.Pull(key).GetAwaiter().GetResult();
    }

    [HttpPost("subscribe")]
    public IActionResult Subscribe(string key, string clientAddress)
    {
        Console.WriteLine($"Subscribing message with key: {key} and clientAddress : {clientAddress}");
        _messagePublisher.Subscribe(key, clientAddress).GetAwaiter().GetResult();
        return Ok();
    }
    
    //syncer call this
    [HttpPost("update_brokers")]
    public IActionResult UpdateBrokers(List<BrokerData> brokers)
    {
        _routingTableStorage.UpdateBrokers(brokers);
        return Ok();
    }
    
    [HttpPost("subscribe")]
    public IActionResult UpdatePointer(string key, string lastConsumedMessageId)
    {
        Console.WriteLine($"Pointer of key {key} updated to : {lastConsumedMessageId}");
        _messagePublisher.UpdatePointer(key, lastConsumedMessageId).GetAwaiter().GetResult();
        return Ok();
    }

}