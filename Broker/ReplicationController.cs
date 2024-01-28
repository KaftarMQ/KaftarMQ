using System.Text;
using Broker.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Broker;

[ApiController]
[Route("[controller]")]
public class ReplicationController : ControllerBase
{
    private readonly ReplicationMetadata _replicationMetadata;

    public ReplicationController(ReplicationMetadata replicationMetadata)
    {
        _replicationMetadata = replicationMetadata ?? throw new ArgumentNullException(nameof(replicationMetadata));
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
    public IActionResult UpdatePointer(string key, string lastConsumedMessageId)
    {
        return Ok();
    }
}