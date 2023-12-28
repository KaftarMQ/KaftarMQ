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

    [HttpPost]
    public IActionResult Push(string key, byte[] value)
    {
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        _broker.PushMessage(key, value);
        return Ok();
    }

    [HttpGet]
    public ActionResult<Message?> Pull(string key)
    {
        return Ok(_broker.PullMessage(key));
    }

    [HttpPost]
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