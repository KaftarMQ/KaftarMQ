using App.Metrics;
using App.Metrics.Counter;
using Broker;
using Microsoft.AspNetCore.Mvc;
using Router.Business;
using RoutingAlgorithm;
using Syncer;

namespace Router;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly RoutingTableStorage _routingTableStorage;
    private readonly MessagePublisher _messagePublisher;
    private readonly IMetrics _metrics;
    private static readonly CounterOptions PushCounter = new CounterOptions
    {
        Name = "router_push_requests_total",
        MeasurementUnit = Unit.Requests
    };


    public MessageController(RoutingTableStorage routingTableStorage, MessagePublisher messagePublisher, IMetrics metrics)
    {
        _routingTableStorage = routingTableStorage ?? throw new ArgumentNullException(nameof(routingTableStorage));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }

    [HttpPost("push")]
    public IActionResult Push(string key, string value)
    {
        _metrics.Measure.Counter.Increment(PushCounter);

        Console.WriteLine($"Pushing message with key: {key}, value: {value}");
        _messagePublisher.Publish(key, value).GetAwaiter().GetResult();
        return Ok();
    }

    [HttpGet("pull")]
    public ActionResult<Message?> Pull() // todo make PULL blocking
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
    [HttpPost("updateBrokers")]
    public ActionResult UpdateBrokers([FromBody] string[] brokers)
    {
        _routingTableStorage.UpdateBrokers(brokers.Select(u => new BrokerData(u, false)).ToList());
        return Ok();
    }    

    //syncer call this
    [HttpPost("UpdateBrokerFailure")]
    public IActionResult UpdateBrokerFailure(string brokerUrl)
    {
        _routingTableStorage.UpdateBrokerFailure(brokerUrl);
        return Ok();
    }
}