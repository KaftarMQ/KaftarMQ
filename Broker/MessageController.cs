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
    public IActionResult Push(string key, string value, Guid id)
    {
        Console.WriteLine($"Pushing message with key: {key}, value: {value}");
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        _broker.PushMessage(key, value, id);
        return Ok();
    }

    [HttpGet]
    public ActionResult<Message?> Pull(string key)
    {
        return Ok(_broker.PullMessage(key));
    }

    [HttpPost("subscribe")]
    public IActionResult Subscribe(string key, string clientAddress)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        _broker.AddSubscriber(key, clientAddress);
        return Ok();
    }
}