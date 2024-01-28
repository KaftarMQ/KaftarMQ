using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Broker;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly IBroker _broker;

    public MessageController(IBroker broker)
    {
        _broker = broker;
    }

    [HttpPost("push")]
    public IActionResult Push(Guid key, string value)
    {
        Console.WriteLine($"Pushing message with key: {key}, value: {value}");

        _broker.PushMessage(key, value);
        return Ok();
    }

    [HttpGet]
    public ActionResult<Message?> Pull(Guid key)
    {
        return Ok(_broker.PullMessage(key));
    }

    [HttpPost("subscribe")]
    public IActionResult Subscribe(Guid key, string clientAddress)
    {
        _broker.AddSubscriber(key, clientAddress);
        return Ok();
    }
}