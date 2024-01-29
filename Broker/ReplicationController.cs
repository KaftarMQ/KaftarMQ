using Broker.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Broker;

[ApiController]
[Route("[controller]")]
public class ReplicationController : ControllerBase
{
    private readonly ReplicationMetadata _replicationMetadata;
    private readonly IBroker _broker;

    public ReplicationController(ReplicationMetadata replicationMetadata, IBroker broker)
    {
        _replicationMetadata = replicationMetadata ?? throw new ArgumentNullException(nameof(replicationMetadata));
        _broker = broker;
    }

    [HttpPost("SetMaster")]
    public IActionResult SetMaster(string key)
    {
        Console.WriteLine($"Broker set as master for key : {key}");
        _replicationMetadata.SetMaster(key);
        return Ok();
    }
    
    [HttpPost("SetSlave")]
    public IActionResult SetSlave(string key)
    {
        Console.WriteLine($"Broker set as slave for key : {key}");
        _replicationMetadata.SetSlave(key);
        return Ok();
    }

    [HttpPost("UpdatePointer")]
    public IActionResult UpdatePointer(string key, Guid lastConsumedMessageId)
    {
        _broker.UpdatePointer(key, lastConsumedMessageId);
        return Ok();
    }
}