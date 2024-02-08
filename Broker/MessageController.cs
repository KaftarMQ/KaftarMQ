using Microsoft.AspNetCore.Mvc;

namespace Broker;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly Broker _broker;

    public MessageController(Broker broker)
    {
        _broker = broker;
    }

    [HttpPost("push")]
    public IActionResult Push(string key, string value, Guid id, bool isReplication)
    {
        Console.WriteLine($"Received message with key: {key}, value: {value}, isReplication: {isReplication}");
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        _broker.PushMessage(key, value, id, isReplication);
        return Ok();
    }

    [HttpGet("Pull")]
    public ActionResult<Message?> Pull()
    {
        return Ok(_broker.PullMessage());
    }
    
    [HttpGet("PullSlave")]
    public ActionResult<Message?> PullSlave(Guid messageId)
    {
        _broker.DropSlave(messageId);
        return Ok();
    }   

    [HttpGet("MoveSlaveContentToMaster")]
    public ActionResult<Message?> MoveSlaveContentToMaster()
    {
        _broker.MoveSlaveContentToMaster();
        return Ok();
    }
}